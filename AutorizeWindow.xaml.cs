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
    /// Логика взаимодействия для AutorizeWindow.xaml
    /// </summary>
    public partial class AutorizeWindow : Window
    {
        DataBase database = new DataBase(); // экземпляр класса DataBase
        public AutorizeWindow()
        {
            InitializeComponent();
        }
        public string Username { get; set; }
        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            Username = UsernameTextBox.Text; //сохраняем значение логина
            string password = PasswordBox.Password; // и пароля

            var(isAuthenticated, role) = AuthenticateUser(Username, password); // кортеж значений

            if (isAuthenticated) // если вернуло true
            {
                OpenRoleBaseWindow(role); //открываем окно соотв. роли
                MessageBox.Show("Вход выполнен успешно!", "Уведомление");
                this.Close(); // Закрыть окно авторизации
            }
            else
            {
                MessageBox.Show("Неверное имя пользователя или пароль.", "Ошибка");
            }
        }

        private void OpenRoleBaseWindow(string role)
        {
            Window windowToOpen; // экземпляр окна
            switch(role) // идем по роли
            {
                case "admin": 
                    windowToOpen = new AdminWindow(Username); // сохраняем админское окно, передаем логин для отображения                     
                    break;
                case "user":
                    windowToOpen = new MainWindow(); // или пользовательское
                    break;
                default:
                    MessageBox.Show("Неизвестный пользователь", "Предупреждение");
                    return;
            }
            windowToOpen.Show(); // открываем соотв. окно
        }

        /// <summary>
        /// метод возвращает два параметра и принимает логин и пароль
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private (bool isAuthenticated, string role) AuthenticateUser(string username, string password)
        {          
            database.OpenConnection();
            string query = "SELECT Role FROM Users WHERE Username = @Username AND PasswordHash = @Password"; // запрос ищет роль по совпадению логина и пароля

            using (SqlCommand command = new SqlCommand(query, database.GetConnection()))
            {
                // добавляем параметры для поиска
                command.Parameters.AddWithValue("@Username", username); 
                command.Parameters.AddWithValue("@Password", password); 

                var role = command.ExecuteScalar() as string; // роль сохраняем строкой
                return (role != null, role); // если не null возвращаем ее
            }            
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow rg = new RegisterWindow();   // открывает окно регистрации
            rg.Show();
            this.Close();
        }
    }
}
