using Microsoft.Data.SqlClient;
using System.Data;

namespace WPF_MSSQL
{
    public class DataBase
    {
        SqlConnection sqlConnection = new SqlConnection(@"Server=Anastasia-ПК\SQLEXPRESS;Database=Biblio;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true;encrypt=false");
        public void OpenConnection()
        {
            // Если состояние строки закрыто, открываем
            if (sqlConnection.State == ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
        }

        public void CloseConnection()
        {
            // Если состояние строки открыто, закрываем
            if (sqlConnection.State == ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }

        public SqlConnection GetConnection()
        {
            return sqlConnection;
        }

        public DataTable SelectData(string query)
        {
            SqlCommand sqlCommand = new SqlCommand(query, GetConnection());
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            return dataTable;
        }
    }
}
