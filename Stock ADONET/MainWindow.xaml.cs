using DB_Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;

namespace Stock_ADONET
{
    public partial class MainWindow : Window
    {
        const string connectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=Stock;Integrated Security=True;Encrypt=False";
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

                    List<Suppliers> suppliers = await LoadSuppliers();
                    Suppliers.ItemsSource = suppliers;
                    List<Products> products = await LoadProducts();
                    Products.ItemsSource = products;
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
                Suppliers.ItemsSource = null;
                Products.ItemsSource = null;
            }
        }

        private async Task<List<Suppliers>> LoadSuppliers()
        {
            List<Suppliers> suppliers = new List<Suppliers>();
            string query = "SELECT * FROM Suppliers";

            try
            {
                if (databaseManager.sqlConnection.State == ConnectionState.Closed)
                {
                    await databaseManager.OpenConnection();
                }

                using (SqlCommand sqlCommand = new SqlCommand(query, databaseManager.sqlConnection))
                using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                {
                    while (await sqlDataReader.ReadAsync())
                    {
                        suppliers.Add(new Suppliers(
                            (int)sqlDataReader[0],
                            sqlDataReader[1].ToString(),
                            sqlDataReader[2].ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading suppliers: {ex.Message}", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                await databaseManager.CloseConnection();
            }

            return suppliers;
        }


        private async Task<List<Products>> LoadProducts()
        {
            List<Products> products = new List<Products>();
            string query = "SELECT * FROM Products";

            using (SqlCommand sqlCommand = new SqlCommand(query, databaseManager.sqlConnection))
            using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
            {
                while (await sqlDataReader.ReadAsync())
                {
                    products.Add(new Products(
                        (int)sqlDataReader[0],
                        sqlDataReader[1].ToString(),
                        sqlDataReader[2].ToString(),
                        (int)sqlDataReader[3],
                        (int)sqlDataReader[4],
                        (decimal)sqlDataReader[5],
                        sqlDataReader.GetDateTime(6)));
                }
            }
            return products;
        }

        private List<Products> ConvertDataTableToProductsList(DataTable dataTable)
        {
            List<Products> list = new List<Products>();

            foreach (DataRow row in dataTable.Rows)
            {
                list.Add(new Products(
                    Convert.ToInt32(row["ProductId"]),
                    row["ProductName"].ToString(),
                    row["ProductType"].ToString(),
                    Convert.ToInt32(row["SupplierId"]),
                    Convert.ToInt32(row["Quantity"]),
                    Convert.ToDecimal(row["Cost"]),
                    Convert.ToDateTime(row["SupplyDate"])
                ));
            }

            return list;
        }

        private async void InsertProductButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Products newProduct = new Products
                {
                    ProductName = ProductNameTextBox.Text,
                    ProductType = ProductTypeTextBox.Text,
                    SupplierID = Convert.ToInt32(SupplierIDProductTextBox.Text),
                    Quantity = Convert.ToInt32(QuantityTextBox.Text),
                    Cost = Convert.ToDecimal(CostTextBox.Text),
                    SupplyDate = Convert.ToDateTime(SupplyDateTextBox.Text)
                };

                await databaseManager.InsertProductAsync(newProduct);
                MessageBox.Show("Product inserted successfully!", "System Message", MessageBoxButton.OK, MessageBoxImage.Information);

                // Refresh the DataGrid
                List<Products> products = await LoadProducts();
                Products.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting product: {ex.Message}", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void InsertSupplierButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Suppliers newSupplier = new Suppliers
                {
                    SupplierName = SupplierNameTextBox.Text,
                    ContactInfo = ContactInfoTextBox.Text
                };

                await databaseManager.InsertSupplierAsync(newSupplier);
                MessageBox.Show("Supplier inserted successfully!", "System Message", MessageBoxButton.OK, MessageBoxImage.Information);

                // Refresh the DataGrid
                List<Suppliers> suppliers = await LoadSuppliers();
                Suppliers.ItemsSource = suppliers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting supplier: {ex.Message}", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UpdateProductButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Products selectedProduct = (Products)Products.SelectedItem;

                if (selectedProduct != null)
                {
                    selectedProduct.ProductName = ProductNameTextBox.Text;
                    selectedProduct.ProductType = ProductTypeTextBox.Text;
                    selectedProduct.SupplierID = Convert.ToInt32(SupplierIDTextBox.Text);
                    selectedProduct.Quantity = Convert.ToInt32(QuantityTextBox.Text);
                    selectedProduct.Cost = Convert.ToDecimal(CostTextBox.Text);
                    selectedProduct.SupplyDate = Convert.ToDateTime(SupplyDateTextBox.Text);

                    await databaseManager.UpdateProductAsync(selectedProduct);
                    MessageBox.Show("Product updated successfully!", "System Message", MessageBoxButton.OK, MessageBoxImage.Information);

                    List<Products> products = await LoadProducts();
                    Products.ItemsSource = products;
                }
                else
                {
                    MessageBox.Show("Please select a product to update.", "System Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating product: {ex.Message}", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UpdateSupplierButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Suppliers selectedSupplier = (Suppliers)Suppliers.SelectedItem;

                if (selectedSupplier != null)
                {
                    selectedSupplier.SupplierName = SupplierNameTextBox.Text;
                    selectedSupplier.ContactInfo = ContactInfoTextBox.Text;

                    await databaseManager.UpdateSupplierAsync(selectedSupplier);
                    MessageBox.Show("Supplier updated successfully!", "System Message", MessageBoxButton.OK, MessageBoxImage.Information);

                    List<Suppliers> suppliers = await LoadSuppliers();
                    Suppliers.ItemsSource = suppliers;
                }
                else
                {
                    MessageBox.Show("Please select a supplier to update.", "System Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating supplier: {ex.Message}", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Products selectedProduct = (Products)Products.SelectedItem;

                if (selectedProduct != null)
                {
                    await databaseManager.DeleteProductAsync(selectedProduct.ProductID);
                    MessageBox.Show("Product deleted successfully!", "System Message", MessageBoxButton.OK, MessageBoxImage.Information);

                    List<Products> products = await LoadProducts();
                    Products.ItemsSource = products;
                }
                else
                {
                    MessageBox.Show("Please select a product to delete.", "System Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting product: {ex.Message}", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteSupplierButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Suppliers selectedSupplier = (Suppliers)Suppliers.SelectedItem;

                if (selectedSupplier != null)
                {
                    await databaseManager.DeleteSupplierAsync(selectedSupplier.SupplierID);
                    MessageBox.Show("Supplier deleted successfully!", "System Message", MessageBoxButton.OK, MessageBoxImage.Information);

                    List<Suppliers> suppliers = await LoadSuppliers();
                    Suppliers.ItemsSource = suppliers;
                }
                else
                {
                    MessageBox.Show("Please select a supplier to delete.", "System Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting supplier: {ex.Message}", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}