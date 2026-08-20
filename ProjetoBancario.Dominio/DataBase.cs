using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoBancario
{
    public static class DataBase
    {
        private const string ConnectionString = "Data Source=dados.db";
        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(ConnectionString);
        }
    }
}
