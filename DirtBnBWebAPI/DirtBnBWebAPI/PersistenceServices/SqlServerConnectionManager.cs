﻿using System;

namespace DirtBnBWebAPI.PersistenceServices
{
    public class SqlServerConnectionManager
    {
        private static SqlServerConnectionManager _instance;
        private MySql.Data.MySqlClient.MySqlConnection sqlConnection;

        private SqlServerConnectionManager()
        {
            Initialize();
        }

        public static SqlServerConnectionManager Instance => _instance ?? (_instance = new SqlServerConnectionManager());

        public bool Initialized { get; private set; }

        public void Initialize()
        {
            var connectionString = "server=127.0.0.1;uid=root;pwd=personalproject;database=personalproject";
            try
            {
                sqlConnection = new MySql.Data.MySqlClient.MySqlConnection();
                sqlConnection.ConnectionString = connectionString;
                sqlConnection.Open();
                Initialized = true;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw new Exception("An error has occurred: " + ex);
            }
        }

        public MySql.Data.MySqlClient.MySqlConnection GetSqlConnection()
        {
            if (Initialized)
            {
                return sqlConnection;
            }
            else
            {
                throw new Exception("Unable to get SQL Connection");
            }
        }

        public void CloseSQLConnection()
        {
            sqlConnection.Close();
        }
    }
}
