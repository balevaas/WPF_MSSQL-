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
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        DataBase db = new DataBase();
        Random random = new Random();
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTB.Text; //сохранили значение логина
            string password = PasswordTB.Password; // и пароля
            db.OpenConnection();
            string checkUserQuery = "Select count(*) from users where Username = @Login"; //проверка существует ли такой пользователь
            SqlCommand cmd = new SqlCommand(checkUserQuery, db.GetConnection());
            cmd.Parameters.AddWithValue("@Login", login); // добавляем параметр для поиска
            int userExists = (int)cmd.ExecuteScalar(); // сохраняем результат запроса (кол-во найденных по логину пользователей)
            if(userExists > 0) // если их больше 0, значит такой логин уже занят
            {
                MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка");
                return;
            }
            else // иначе процесс регистрации
            {
                int userId = random.Next(10,200); // рандомом определяем id пользователя
                string insertUserQuery = "insert into Users (UserID, Username, PasswordHash, Role) " +
                    "values (@userId, @login, @password, @role)"; // запрос на добавление нового пользователя с параметрами
                SqlCommand inserCmd = new SqlCommand(insertUserQuery, db.GetConnection()); // создаем команду
                // добавляем параметры для добавления
                inserCmd.Parameters.AddWithValue("@userId", userId);
                inserCmd.Parameters.AddWithValue("@login", login);
                inserCmd.Parameters.AddWithValue("@password", password);
                inserCmd.Parameters.AddWithValue("@role", SelectedRole);

                int rowsAffected = inserCmd.ExecuteNonQuery(); // сам запрос работает, здесь сохраняем количество новых строк
                if (rowsAffected > 0) MessageBox.Show("Регистрация прошла успешно!", "Успех"); // строк больше 0 - добавили
                else MessageBox.Show("Произошла ошибка при регистрации", "Ошибка"); // иначе ошибка
            }
            db.CloseConnection();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        public string SelectedRole { get; set; }
        private void TypeAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // по выбранной роли в combobox получаем индекс
            switch(TypeAccount.SelectedIndex)
            {
                case 0: //если выбран первый элемент из списка 
                    SelectedRole = "admin"; // значит админ
                    break;
                case 1:
                    SelectedRole = "user"; // иначе пользователь
                    break;
                default:
                    break;
            }
        }
    }
}
