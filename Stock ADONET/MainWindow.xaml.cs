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
        const string connectionString = @"Data Source=pakkkek\SQLEXPRESS;Initial Catalog=Market;Integrated Security=True;Encrypt=False";
        DatabaseManager databaseManager;

        public MainWindow()
        {
            InitializeComponent();
            databaseManager = new DatabaseManager(connectionString);
        }

        private async void ConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await databaseManager.OpenConnection();

                if (databaseManager.sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    MessageBox.Show("Connection successful!", "System Message", MessageBoxButton.OK, MessageBoxImage.Information);

                    List<FruitsVegetables> fruitsVegetables = await LoadFruitsVegetables();
                    FruitsVegetables.ItemsSource = fruitsVegetables;
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

        private async void DisconnectionButton_Click(object sender, RoutedEventArgs e)
        {
            await databaseManager.CloseConnection();

            if (databaseManager.sqlConnection.State == System.Data.ConnectionState.Closed)
            {
                MessageBox.Show("Disconnection successful!", "System Message", MessageBoxButton.OK, MessageBoxImage.Information);
                FruitsVegetables.ItemsSource = null;
            }
        }

        private async Task<List<FruitsVegetables>> LoadFruitsVegetables()
        {
            List<FruitsVegetables> fruitsVegetables = new List<FruitsVegetables>();
            string query = "SELECT * FROM FruitsVegetables";

            using (SqlCommand sqlCommand = new SqlCommand(query, databaseManager.sqlConnection))
            using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
            {
                while (await sqlDataReader.ReadAsync())
                {
                    fruitsVegetables.Add(new FruitsVegetables(
                        (int)sqlDataReader[0],
                        sqlDataReader[1].ToString(),
                        sqlDataReader[2].ToString(),
                        sqlDataReader[3].ToString(),
                        (int)sqlDataReader[4]));
                }
            }
            return fruitsVegetables;
        }
    }
}
