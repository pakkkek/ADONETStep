using DB_Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;

namespace Stock_ADONET
{
    public partial class MainWindow : Window
    {
        const string connectionString = @"Data Source=pakkkek\SQLEXPRESS;Initial Catalog=Stock;Integrated Security=True;Encrypt=False";
        DatabaseManager databaseManager;

        public MainWindow()
        {
            InitializeComponent();
            databaseManager = new DatabaseManager(connectionString);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await databaseManager.OpenConnection();

                if (databaseManager.sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    MessageBox.Show("Connection successful!", "System Message", MessageBoxButton.OK, MessageBoxImage.Information);

                    List<Student> students = await LoadStudents();
                    StockUsers.ItemsSource = students;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failed: {ex.Message}", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                await databaseManager.CloseConnection();
            }
        }

        private async Task<List<Student>> LoadStudents()
        {
            List<Student> students = new List<Student>();
            string query = "SELECT * FROM Students";

            using (SqlCommand sqlCommand = new SqlCommand(query, databaseManager.sqlConnection))
            using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
            {
                while (await sqlDataReader.ReadAsync())
                {
                    students.Add(new Student(
                        (int)sqlDataReader[0],
                        sqlDataReader[1].ToString(),
                        sqlDataReader[2].ToString(),
                        (decimal)sqlDataReader[3],
                        sqlDataReader[4].ToString(),
                        sqlDataReader[5].ToString()));
                }
            }
            return students;
        }
    }
}
