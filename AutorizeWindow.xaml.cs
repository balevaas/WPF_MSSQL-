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
        DataBase database = new DataBase();
        public AutorizeWindow()
        {
            InitializeComponent();
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            var(isAuthenticated, role) = AuthenticateUser(username, password); // кортеж значений

            if (isAuthenticated)
            {
                OpenRoleBaseWindow(role);
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
            Window windowToOpen;
            switch(role)
            {
                case "admin":
                    windowToOpen = new AdminWindow();
                    break;
                case "user":
                    windowToOpen = new UserWindow();
                    break;
                default:
                    MessageBox.Show("Неизвестный пользователь", "Предупреждение");
                    return;
            }
            windowToOpen.Show();
        }

        private (bool isAuthenticated, string role) AuthenticateUser(string username, string password)
        {          
            database.OpenConnection();
            string query = "SELECT Role FROM Users WHERE Username = @Username AND PasswordHash = @Password";

            using (SqlCommand command = new SqlCommand(query, database.GetConnection()))
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password); // Не забудьте использовать хеширование пароля!

                var role = command.ExecuteScalar() as string;
                return (role != null, role);
            }            
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            RegisterWindow rg = new RegisterWindow();   
            rg.Show();
        }
    }
}
