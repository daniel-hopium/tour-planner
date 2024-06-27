using System;
using System.Configuration;
using System.Data;
using Npgsql;

namespace TourPlanner.Persistence.Utils;

public static class DatabaseManager
{
  private static readonly string _connectionString =
    ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString;

  public static string ConnectionString
  {
    get
    {
      if (CheckDbConnection()) return _connectionString;

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
}