using System;
using Npgsql;

namespace TourPlanner.Persistence.Utils;

public class DatabaseManager
{
    private const string DefaultConnectionString = "Host=127.0.0.1;Port=5432;Username=postgres;Password=postgres;Database=tour_planner_db";
    private static NpgsqlConnection? _dbConnection;

    // Public property to access the database connection string
    public static string ConnectionString { get; } = DefaultConnectionString;
    
    // Property to access the database connection instance
    public static NpgsqlConnection DbConnection
    {
        get
        {
            if (_dbConnection == null)
            {
                throw new InvalidOperationException("Database connection has not been opened.");
            }

            return _dbConnection;
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