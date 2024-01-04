using System;
using System.Data.SqlClient;

namespace DB_Entity
{
    public class Manager
    {
        public int ManagerID { get; set; }
        public string ManagerName { get; set; }

        public Manager(int managerID, string managerName)
        {
            ManagerID = managerID;
            ManagerName = managerName;
        }
    }

    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public int Quantity { get; set; }
        public decimal CostPrice { get; set; }

        public Product(int productID, string productName, string productType, int quantity, decimal costPrice)
        {
            ProductID = productID;
            ProductName = productName;
            ProductType = productType;
            Quantity = quantity;
            CostPrice = costPrice;
        }
    }

    public class Sale
    {
        public int SaleID { get; set; }
        public int ProductID { get; set; }
        public int ManagerID { get; set; }
        public string CustomerCompany { get; set; }
        public int QuantitySold { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime SaleDate { get; set; }

        public Sale (int saleID, int productID, int managerID, string customerCompany, int quantitySold, decimal unitPrice, DateTime saleDate)
        {
            SaleID = saleID;
            ProductID = productID;
            ManagerID = managerID;
            CustomerCompany = customerCompany;
            QuantitySold = quantitySold;
            UnitPrice = unitPrice;
            SaleDate = saleDate;
        }
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
    }
}
