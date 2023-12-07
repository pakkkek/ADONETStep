using System;
using System.Data;
using System.Data.SqlClient;

namespace DB_Entity
{
    public class Suppliers
    {
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string ContactInfo { get; set; }

        public Suppliers(int supplierID, string supplierName, string contactInfo)
        {
            SupplierID = supplierID;
            SupplierName = supplierName;
            ContactInfo = contactInfo;
        }

        public Suppliers() { }
    }

    public class Products
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public int SupplierID { get; set; }
        public int Quantity { get; set; }
        public decimal Cost { get; set; }
        public DateTime SupplyDate { get; set; }

        public Products(int productID, string productName, string productType, int supplierID, int quantity, decimal cost, DateTime supplyDate)
        {
            ProductID = productID;
            ProductName = productName;
            ProductType = productType;
            SupplierID = supplierID;
            Quantity = quantity;
            Cost = cost;
            SupplyDate = supplyDate;
        }

        public Products() { }
    }


    public class DatabaseManager
    {
        public string ConnectionString { get; set; }

        public SqlConnection sqlConnection { get; set; }

        public DatabaseManager(string connectionString)
        {
            ConnectionString = connectionString;
            sqlConnection = new SqlConnection(ConnectionString);
        }

        public async Task<bool> OpenConnection()
        {
            try
            {
                await sqlConnection.OpenAsync();
                return true;
            }
            catch (SqlException sql_ev)
            {
                throw new Exception(sql_ev.Message);
            }
        }

        public async Task<bool> CloseConnection()
        {
            try
            {
                await sqlConnection.CloseAsync();
                return true;
            }
            catch (SqlException sql_ev)
            {
                throw new Exception(sql_ev.Message);
            }
        }

        public async Task ExecuteNonQuery(string query)
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
            {
                await OpenConnection();
            }
            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
            sqlCommand.ExecuteNonQuery();
            await CloseConnection();
        }

        public async Task<DataTable> ExecuteQueryAsync(string query)
        {
            if (sqlConnection.State == ConnectionState.Closed)
            {
                await OpenConnection();
            }

            DataTable dataTable = new DataTable();
            using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
            using (SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand))
            {
                adapter.Fill(dataTable);
            }

            await CloseConnection();

            return dataTable;
        }

        public async Task<List<string>> GetAllTypesAsync()
        {
            string query = "SELECT DISTINCT ProductType FROM Products";
            DataTable dataTable = await ExecuteQueryAsync(query);

            List<string> types = new List<string>();
            foreach (DataRow row in dataTable.Rows)
            {
                types.Add(row["ProductType"].ToString());
            }

            return types;
        }

        public async Task<List<string>> GetAllSuppliersAsync()
        {
            string query = "SELECT DISTINCT SupplierName FROM Suppliers";
            DataTable dataTable = await ExecuteQueryAsync(query);

            List<string> suppliers = new List<string>();
            foreach (DataRow row in dataTable.Rows)
            {
                suppliers.Add(row["SupplierName"].ToString());
            }

            return suppliers;
        }

        public async Task<Products> GetProductWithMaxQuantityAsync()
        {
            string query = "SELECT TOP 1 * FROM Products ORDER BY Quantity DESC";
            DataTable dataTable = await ExecuteQueryAsync(query);

            if (dataTable.Rows.Count > 0)
            {
                DataRow row = dataTable.Rows[0];
                return new Products(
                    Convert.ToInt32(row["ProductID"]),
                    row["ProductName"].ToString(),
                    row["ProductType"].ToString(),
                    Convert.ToInt32(row["SupplierID"]),
                    Convert.ToInt32(row["Quantity"]),
                    Convert.ToDecimal(row["Cost"]),
                    Convert.ToDateTime(row["SupplyDate"])
                );
            }

            return null;
        }

        public async Task<Products> GetProductWithMinQuantityAsync()
        {
            string query = "SELECT TOP 1 * FROM Products ORDER BY Quantity ASC";
            DataTable dataTable = await ExecuteQueryAsync(query);

            if (dataTable.Rows.Count > 0)
            {
                DataRow row = dataTable.Rows[0];
                return new Products(
                    Convert.ToInt32(row["ProductID"]),
                    row["ProductName"].ToString(),
                    row["ProductType"].ToString(),
                    Convert.ToInt32(row["SupplierID"]),
                    Convert.ToInt32(row["Quantity"]),
                    Convert.ToDecimal(row["Cost"]),
                    Convert.ToDateTime(row["SupplyDate"])
                );
            }

            return null;
        }

        public async Task<Products> GetProductWithMinCostAsync()
        {
            string query = "SELECT TOP 1 * FROM Products ORDER BY Cost ASC";
            DataTable dataTable = await ExecuteQueryAsync(query);

            if (dataTable.Rows.Count > 0)
            {
                DataRow row = dataTable.Rows[0];
                return new Products(
                    Convert.ToInt32(row["ProductID"]),
                    row["ProductName"].ToString(),
                    row["ProductType"].ToString(),
                    Convert.ToInt32(row["SupplierID"]),
                    Convert.ToInt32(row["Quantity"]),
                    Convert.ToDecimal(row["Cost"]),
                    Convert.ToDateTime(row["SupplyDate"])
                );
            }

            return null;
        }

        public async Task<Products> GetProductWithMaxCostAsync()
        {
            string query = "SELECT TOP 1 * FROM Products ORDER BY Cost DESC";
            DataTable dataTable = await ExecuteQueryAsync(query);

            if (dataTable.Rows.Count > 0)
            {
                DataRow row = dataTable.Rows[0];
                return new Products(
                    Convert.ToInt32(row["ProductID"]),
                    row["ProductName"].ToString(),
                    row["ProductType"].ToString(),
                    Convert.ToInt32(row["SupplierID"]),
                    Convert.ToInt32(row["Quantity"]),
                    Convert.ToDecimal(row["Cost"]),
                    Convert.ToDateTime(row["SupplyDate"])
                );
            }

            return null;
        }

        public async Task InsertProductAsync(Products product)
        {
            string query = $"INSERT INTO Products (ProductName, ProductType, SupplierID, Quantity, Cost, SupplyDate) " +
                           $"VALUES ('{product.ProductName}', '{product.ProductType}', {product.SupplierID}, " +
                           $"{product.Quantity}, {product.Cost}, '{product.SupplyDate.ToString("yyyy-MM-dd HH:mm:ss")}')";
            await ExecuteNonQuery(query);
        }

        public async Task InsertTypeAsync(string productType)
        {
            string query = $"INSERT INTO ProductTypes (ProductType) VALUES ('{productType}')";
            await ExecuteNonQuery(query);
        }

        public async Task InsertSupplierAsync(Suppliers supplier)
        {
            string query = $"INSERT INTO Suppliers (SupplierName, ContactInfo) VALUES ('{supplier.SupplierName}', '{supplier.ContactInfo}')";
            await ExecuteNonQuery(query);
        }

        public async Task UpdateProductAsync(Products product)
        {
            string query = $"UPDATE Products SET ProductName = '{product.ProductName}', ProductType = '{product.ProductType}', " +
                           $"SupplierID = {product.SupplierID}, Quantity = {product.Quantity}, Cost = {product.Cost}, " +
                           $"SupplyDate = '{product.SupplyDate.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE ProductID = {product.ProductID}";
            await ExecuteNonQuery(query);
        }

        public async Task UpdateSupplierAsync(Suppliers supplier)
        {
            string query = $"UPDATE Suppliers SET SupplierName = '{supplier.SupplierName}', ContactInfo = '{supplier.ContactInfo}' " +
                           $"WHERE SupplierID = {supplier.SupplierID}";
            await ExecuteNonQuery(query);
        }

        public async Task UpdateTypeAsync(string oldType, string newType)
        {
            string query = $"UPDATE ProductTypes SET ProductType = '{newType}' WHERE ProductType = '{oldType}'";
            await ExecuteNonQuery(query);
        }

        public async Task DeleteProductAsync(int productId)
        {
            string query = $"DELETE FROM Products WHERE ProductID = {productId}";
            await ExecuteNonQuery(query);
        }

        public async Task DeleteSupplierAsync(int supplierId)
        {
            string query = $"DELETE FROM Suppliers WHERE SupplierID = {supplierId}";
            await ExecuteNonQuery(query);
        }

        public async Task DeleteTypeAsync(string productType)
        {
            string query = $"DELETE FROM ProductTypes WHERE ProductType = '{productType}'";
            await ExecuteNonQuery(query);
        }
    }
}