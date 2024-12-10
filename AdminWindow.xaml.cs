using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
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
        public AdminWindow(string username)
        {
            InitializeComponent();
            AdminName.Text = $"Добро пожаловать, {username}!";
            booksgrid.ItemsSource = database.SelectData("select * from Books").DefaultView;
            authorsgrid.ItemsSource = database.SelectData("select * from Authors").DefaultView;
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void AddBooks_Click(object sender, RoutedEventArgs e)
        {
            var header_name = EditingDates.SelectedItem as TabItem;
            string header = header_name.Header.ToString();
            switch(header)
            {
                case "Книги":
                    AddDataToBooks();
                    break;
                case "Авторы":
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

        private void DeleteBooks_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateBooks_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
