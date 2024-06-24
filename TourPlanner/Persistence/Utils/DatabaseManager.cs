using System;
using Npgsql;
using System.Configuration;
using System.Data;
using System.Reflection;
using System.Windows;

namespace TourPlanner.Persistence.Utils;

public static class DatabaseManager
{
    private static readonly string _connectionString = ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString;

    public static string ConnectionString 
    {  
        get 
        {
            if(CheckDbConnection())
            {
                return _connectionString;
            }

            return string.Empty;
        }
    }


    public static bool CheckDbConnection()
    {
        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            {
                conn.Open();
                return conn.State == ConnectionState.Open;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

    /*
        /// <summary>
        /// Initializes the database by creating the necessary tables and inserting the initial data.
        /// </summary>
        public static void Initialize()
        {
            if (IsDatabaseSetup())
            {
                return;
            }
            try
            {
                _dbConnection = new NpgsqlConnection(ConnectionString);
                _dbConnection.Open();
                string filePath = "../../../../DataAccess/Db/sql/init.sql";
                var sql = File.ReadAllText(filePath);
                var cmd = new NpgsqlCommand(sql, _dbConnection);
                cmd.ExecuteNonQuery();
                _dbConnection.Close();
            }
            catch (Exception e)
            {
                Log.Error($"An error occured while setting up database", e);
            }
            Log.Info("Database setup complete");
        } 
    */
}