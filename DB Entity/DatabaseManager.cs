using System;
using System.Data.SqlClient;

namespace DB_Entity
{
    public class Student
    {
        public int StudentID { get; set; }
        public string Fullname { get; set; }
        public string GroupName { get; set; }
        public decimal AverageGrade { get; set; }
        public string MinSubject { get; set; }
        public string MaxSubject { get; set; }

        public Student(int studentID, string fullname, string groupname, decimal averageGrade, string minSubject, string maxSubject)
        {
            StudentID = studentID;
            Fullname = fullname;
            GroupName = groupname;
            AverageGrade = averageGrade;
            MinSubject = minSubject;
            MaxSubject = maxSubject;
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

