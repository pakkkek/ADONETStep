using System;
using System.Data.SqlClient;

namespace DB_Entity
{
    public class FruitsVegetables
    {
        public int ItemID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public int Calories { get; set; }

        public FruitsVegetables(int itemID, string name, string type, string color, int calories)
        {
            ItemID = itemID;
            Name = name;
            Type = type;
            Color = color;
            Calories = calories;
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
