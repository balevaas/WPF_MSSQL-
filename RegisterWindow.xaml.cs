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
            string login = LoginTB.Text;
            string password = PasswordTB.Password;
            db.OpenConnection();
            string checkUserQuery = "Select count(*) from users where Username = @Login";
            SqlCommand cmd = new SqlCommand(checkUserQuery, db.GetConnection());
            cmd.Parameters.AddWithValue("@Login", login);
            int userExists = (int)cmd.ExecuteScalar();
            if(userExists > 0)
            {
                MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка");
                return;
            }
            else
            {
                int userId = random.Next(10,200);
                string insertUserQuery = "insert into Users ("
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        public string selectedRole;
        private void TypeAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(TypeAccount.SelectedItem != null)
            {
                selectedRole = TypeAccount.SelectedItem.ToString();
            }
        }
    }
}
