using Microsoft.Data.SqlClient;
using System.Data;
using System.Windows;
using static System.Reflection.Metadata.BlobBuilder;

namespace WPF_MSSQL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataBase database = new DataBase();
        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        // метод для вывода запроса из бд
        private void LoadData()
        {
            database.OpenConnection();
          //booksgrid.ItemsSource =
          //      database.SelectData("select * from Books")
          //      .DefaultView;
            //LoadDataHeaders();
            LoadDataHeadersNames();
            database.CloseConnection();
        }

        private void LoadDataHeaders()
        {
            List<Book> books = new List<Book>();
            try
            {
                database.OpenConnection();
                string query = "select BookID, Title, AuthorID, Images from Books";
                SqlCommand cmd = new SqlCommand(query, database.GetConnection());
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    books.Add(new Book
                    {
                        BookID = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        AuthorID = reader.GetInt32(2),
                        ImagePath = reader.GetString(3)
                    });
                }
                booksgrid.ItemsSource = books;
                database.CloseConnection();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void LoadDataHeadersNames()
        {
            List<Book> books = new List<Book>();
            HashSet<string> authors = new HashSet<string>();
            try
            {
                database.OpenConnection();
                string query = "select BookID, Title, YearPublished, a.Names, b.Images as Author from Books b join Authors a on b.AuthorID = a.AuthorID";
                SqlCommand cmd = new SqlCommand(query, database.GetConnection());
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    books.Add(new Book
                    {
                        BookID = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        YearPublish = reader.GetInt32(2),
                        Author = reader.GetString(3),
                        ImagePath = reader.GetString(4)
                    });
                    authors.Add(reader.GetString(3));
                }
                authors.Add("Сбросить выбор");
                booksgrid.ItemsSource = books;
                AuthorCB.ItemsSource = authors;
                database.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void SearchTb_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            database.OpenConnection();
            SqlDataAdapter dataAdapter = new SqlDataAdapter("select * from Books where concat " +
                "(BookID, Title) like '%" + SearchTb.Text + "%'", database.GetConnection());
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            booksgrid.ItemsSource = dataTable.DefaultView;
            database.CloseConnection();
        }

        private void AuthorCB_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(AuthorCB.SelectedItem != null)
            {
                string selectedAuthor = AuthorCB.SelectedItem.ToString();
                LoadBooksByAuthor(selectedAuthor);
            }
            if(AuthorCB.SelectedItem == "Сбросить выбор")
            {
                LoadDataHeadersNames();
            }
        }
        private void LoadBooksByAuthor(string author)
        {
            string query = "SELECT BookID, Title, YearPublished, a.Names, b.Images FROM Books b JOIN Authors a ON b.AuthorID = a.AuthorID WHERE a.Names = @Author"; // SQL-запрос с параметром

            List<Book> books = new List<Book>(); // Предположим, у вас есть класс Book

            SqlCommand command = new SqlCommand(query, database.GetConnection());
            command.Parameters.AddWithValue("@Author", author); // Добавляем параметр запроса
            database.OpenConnection();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Book book = new Book
                    {
                        BookID = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        YearPublish = reader.GetInt32(2),
                        Author = reader.GetString(3),
                        ImagePath = reader.GetString(4)
                    };
                    books.Add(book);
                }
            }
            booksgrid.ItemsSource = books;
        }

        private void FilterCB(string query, List<Book> books)
        {
            SqlCommand cmd = new SqlCommand(query, database.GetConnection());
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                books.Add(new Book
                {
                    BookID = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    YearPublish = reader.GetInt32(2),
                    Author = reader.GetString(3),
                    ImagePath = reader.GetString(4)
                });
            }
            booksgrid.ItemsSource = books;
        }
        private void YearPublishCB_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            database.OpenConnection();
            SqlDataAdapter dataAdapter;
            DataTable dataTable = new DataTable();
            List<Book> books = new List<Book>();
            switch (YearPublishCB.SelectedIndex)
            {
                case 0:
                    string filtrString1 = "SELECT BookID, Title, YearPublished, a.Names, b.Images FROM Books b JOIN Authors a ON b.AuthorID = a.AuthorID WHERE YearPublished <= 1799 and YearPublished >= 1700";
                    FilterCB(filtrString1, books);
                    break;
                case 1:
                    string filtrString2 = "SELECT BookID, Title, YearPublished, a.Names, b.Images FROM Books b JOIN Authors a ON b.AuthorID = a.AuthorID WHERE YearPublished <= 1899 and YearPublished >= 1800";
                    FilterCB(filtrString2, books);
                    break;
                case 2:
                    string filtrString3 = "SELECT BookID, Title, YearPublished, a.Names, b.Images FROM Books b JOIN Authors a ON b.AuthorID = a.AuthorID WHERE YearPublished <= 1999 and YearPublished >= 1900";
                    FilterCB(filtrString3, books);
                    break;
                case 3:
                    string filtrString4 = "SELECT BookID, Title, YearPublished, a.Names, b.Images FROM Books b JOIN Authors a ON b.AuthorID = a.AuthorID WHERE YearPublished <= 2099 and YearPublished >= 2000";
                    FilterCB(filtrString4, books);
                    break;
                case 4:
                    string filtrString5 = "SELECT BookID, Title, YearPublished, a.Names, b.Images FROM Books b JOIN Authors a ON b.AuthorID = a.AuthorID";
                    FilterCB(filtrString5 , books);
                    break;
            }
            database.CloseConnection();
        }

        private void AutorizeBtn_Click(object sender, RoutedEventArgs e)
        {
            AutorizeWindow auto = new AutorizeWindow(); // просто открываем окно авторизации
            auto.Show();
            this.Close();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class Book
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public int AuthorID { get; set; }
        //public int GenreID { get; set; }
        public int YearPublish {  get; set; }
        //public int PageCount { get; set; }
        public string Author { get; set; }
        public string ImagePath { get; set; }
    }

}