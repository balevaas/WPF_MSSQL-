using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF_MSSQL
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        DataBase database = new DataBase();
        /// <summary>
        /// Передаем параметр username в конструктор окна, чтобы передать значения логина из окна авторизации
        /// </summary>
        /// <param name="username"></param>
        public AdminWindow(string username)
        {
            InitializeComponent();
            AdminName.Text = $"Добро пожаловать, {username}!"; // в текст бокс выводится текст с логином конкретного пользователя
            booksgrid.ItemsSource = database.SelectData("select * from Books").DefaultView; // заполнение таблицы booksgrid во вкладке Книги
            authorsgrid.ItemsSource = database.SelectData("select * from Authors").DefaultView; // заполнение таблицы authorsgrid во вкладке Авторы
            LoadIDBook();
        }

        private void LoadIDBook()
        {
            List<int> books = new List<int>();
            string query = "select BookID from Books";
            SqlCommand cmd = new SqlCommand(query, database.GetConnection());
            database.OpenConnection();
            SqlDataReader reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                books.Add(reader.GetInt32(0));
            }
            bookCB.ItemsSource = books;
            database.CloseConnection();
        }

        /// <summary>
        /// Кнопка "Выход"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            // Вовзращаемся назад в начальное окно
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        /// <summary>
        /// Кнопка Добавить
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var header_name = EditingDates.SelectedItem as TabItem; // сохраняем значение выбронной вкладки
            string header = header_name.Header.ToString(); // сохраняем наименование заголовка открытой вкладки TabControl

            switch(header) // идем по кейсам с названием вкладки
            {
                case "Книги":
                    AddDataToBooks(); // вызываем метод для добавления книг
                    break;
                case "Авторы": // аналогичная работа с вкладкой авторы
                    break;
            }
        }

        private void AddDataToBooks()
        {
            int bookId = Convert.ToInt32(numberbooks.Text.ToString());
            string namebook = namebooks.Text.ToString();
            int authorId = Convert.ToInt32(numberauthors.Text.ToString());
            int genreId = Convert.ToInt32(numbergenre.Text.ToString());
            int yearpublished = Convert.ToInt32(yearpublish.Text.ToString()); 
            int countPage = Convert.ToInt32(pagecount.Text.ToString());
            string link = linkimage.Text.ToString();

            string query = "INSERT INTO Books (BookID, Title, AuthorID, GenreID, YearPublished, PageCount, Images) VALUES (@bookId, @namebook, @authorId, @genreId, @yearpublished, @countPage, @link)";
            try
            {
                database.OpenConnection(); // Открываем соединение с базой данных
                SqlCommand command = new SqlCommand(query, database.GetConnection());

                // Добавляем параметры
                command.Parameters.AddWithValue("@bookId", bookId);
                command.Parameters.AddWithValue("@namebook", namebook);
                command.Parameters.AddWithValue("@authorId", authorId);
                command.Parameters.AddWithValue("@genreId", genreId);
                command.Parameters.AddWithValue("@yearpublished", yearpublished);
                command.Parameters.AddWithValue("@countPage", countPage);
                command.Parameters.AddWithValue("@link", link);

                // Выполняем команду
                int rowsAffected = command.ExecuteNonQuery();

                // Проверяем количество затронутых строк
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Книга успешно добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Не удалось добавить книгу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                database.CloseConnection(); // Закрываем соединение в блоке finally, чтобы гарантировать его закрытие
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            string query = "delete from Books where BookID = @booksid";
            SqlCommand cmd = new SqlCommand(query, database.GetConnection());
            database.OpenConnection();
            cmd.Parameters.AddWithValue("@booksid", SelectedBook);
            int rowsdelete =  cmd.ExecuteNonQuery();
            if(rowsdelete > 0)
            {
                MessageBox.Show($"Данные книги #{SelectedBook} удалены", "Уведомление");
            }
            else MessageBox.Show("Ошибка удаления данных", "Уведомление");
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            string query = "update Books set Title = @title, AuthorID = @authorid, GenreID = @genreid, YearPublished = @year, PageCount = @page, Images = @images where BookID = @booksid";
            SqlCommand command = new SqlCommand(query, database.GetConnection());
            database.OpenConnection();
            command.Parameters.AddWithValue("@booksID", SelectedBook);
            command.Parameters.AddWithValue("@title", namebooks.Text);
            command.Parameters.AddWithValue("@authorid", numberauthors.Text);
            command.Parameters.AddWithValue("@genreid", numbergenre.Text);
            command.Parameters.AddWithValue("@year", yearpublish.Text);
            command.Parameters.AddWithValue("@page", pagecount.Text);
            command.Parameters.AddWithValue("@images", linkimage.Text);
            int rowsupdate = command.ExecuteNonQuery();
            if(rowsupdate > 0)
            {
                MessageBox.Show($"Данные книги #{SelectedBook} обновлены", "Уведомление");
            }
            else MessageBox.Show("Ошибка обновления данных", "Уведомление");
        }

        public int SelectedBook { get; set; }
        private void bookCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bookCB.SelectedItem != null)
            {
                SelectedBook = Convert.ToInt32(bookCB.SelectedItem);
                string query = "select * from Books where BookID = @booksID";
                SqlCommand command = new SqlCommand(query, database.GetConnection());
                database.OpenConnection();
                command.Parameters.AddWithValue("@booksID", SelectedBook);
                SqlDataReader sqlDataReader = command.ExecuteReader();
                while(sqlDataReader.Read())
                {
                    numberbooks.Text = sqlDataReader.GetInt32(0).ToString();
                    namebooks.Text = sqlDataReader.GetString(1).ToString();
                    numberauthors.Text = sqlDataReader.GetInt32(2).ToString();
                    numbergenre.Text = sqlDataReader.GetInt32(3).ToString();
                    yearpublish.Text = sqlDataReader.GetInt32(4).ToString();
                    pagecount.Text = sqlDataReader.GetInt32(5).ToString();
                    linkimage.Text = sqlDataReader.GetString(6).ToString();
                }
                database.CloseConnection();                
            }
        }
    }
}
