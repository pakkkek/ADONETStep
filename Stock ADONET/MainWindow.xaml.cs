using DB_Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Stock_ADONET
{
    public partial class MainWindow : Window
    {
        private SqlConnection connection;
        private string connectionString = @"Data Source=WIN-0I7PB3TGH35\SQLEXPRESS;Initial Catalog=StationeryCompany;Integrated Security=True;Encrypt=False";

        public MainWindow()
        {
            InitializeComponent();
        }
        private void ClearDataGrid()
        {
            DG_Users.ItemsSource = null;
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                MessageBox.Show("Connection is successful!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connect: {ex.Message}");
            }
        }
        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    MessageBox.Show("Database was disconnected.");
                }
                else
                {
                    MessageBox.Show("There is no active database.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error disconnect: {ex.Message}");
            }
        }

        private void LoadDataButton_Click1(object sender, RoutedEventArgs e)
        {
            ClearDataGrid();
            try
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    Button clickedButton = sender as Button;

                    string query = GetQueryByButtonId(clickedButton.Name);
                    if (query != null)
                    {
                        DataTable dataTable = new DataTable();
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                            {
                                adapter.Fill(dataTable);
                            }
                        }

                        DG_Users.ItemsSource = dataTable.DefaultView;
                    }
                    else
                    {
                        MessageBox.Show("Unknown operation.");
                    }
                }
                else
                {
                    MessageBox.Show("First, connect to the database.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
        }

        private string GetQueryByButtonId(string buttonId)
        {
            switch (buttonId)
            {
                case "LoadAllProductsButton":
                    return "SELECT * FROM Products";
                case "LoadProductTypesButton":
                    return "SELECT DISTINCT ProductType FROM Products";
                case "LoadSalesManagersButton":
                    return "SELECT * FROM SalesManagers";
                case "LoadMaxQuantityProductsButton":
                    return "SELECT * FROM Products WHERE QuantityInStock = (SELECT MAX(QuantityInStock) FROM Products)";
                case "LoadMinQuantityProductsButton":
                    return "SELECT * FROM Products WHERE QuantityInStock = (SELECT MIN(QuantityInStock) FROM Products)";
                case "LoadMinCostProductsButton":
                    return "SELECT * FROM Products WHERE CostPrice = (SELECT MIN(CostPrice) FROM Products)";
                case "LoadMaxCostProductsButton":
                    return "SELECT * FROM Products WHERE CostPrice = (SELECT MAX(CostPrice) FROM Products)";
                default:
                    return null;
            }
        }


        private void LoadDataButton_Click(object sender, RoutedEventArgs e)
        {
            ClearDataGrid();
            try
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    DataTable dataTable = new DataTable();
                    using (SqlCommand command = new SqlCommand("SELECT * FROM Products", connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }

                    DG_Users.ItemsSource = dataTable.DefaultView;
                }
                else
                {
                    MessageBox.Show("First, connect to the database.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
        }
    }
}
