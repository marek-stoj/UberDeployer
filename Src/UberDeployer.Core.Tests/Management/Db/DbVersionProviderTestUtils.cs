using System.Configuration;
using System.Data.SqlClient;

namespace UberDeployer.Core.Tests.Management.Db
{
  // TODO IMM HI: use in-memory sqlite
  public class DbVersionProviderTestUtils
  {    
    public static void DropTableIfExists(string tableName)
    {
      using (var connection = new SqlConnection(GetUberDeployerConnectionString()))
      {
        connection.Open();

        DropTableIfExists(tableName, connection);
      }
    }

    private static void DropTableIfExists(string tableName, SqlConnection connection)
    {
      if (TableExists(tableName, connection))
      {
        using (var transaction = connection.BeginTransaction())
        {
          using (var command = connection.CreateCommand())
          {
            command.CommandText = string.Format("DROP TABLE {0}", tableName);
            command.Transaction = transaction;

            command.ExecuteNonQuery();
          }

          transaction.Commit();
        }
      }
    }

    public static void CreateAndFillTable(string tableName, string columnName, params string[] values)
    {
      DropTableIfExists(tableName);

      using (var connection = new SqlConnection(GetUberDeployerConnectionString()))
      {
        connection.Open();        

        DropTableIfExists(tableName, connection);
        CreateTable(tableName, columnName, connection);
        InsertValues(tableName, columnName, values, connection);
      }
    }

    private static void CreateTable(string tableName, string columnName, SqlConnection connection)
    {
      using (var command = connection.CreateCommand())
      {
        command.CommandText = string.Format(
          "CREATE TABLE {0} ( {1} nvarchar(20) )",          
          tableName,
          columnName);

        command.ExecuteNonQuery();
      }
    }

    private static void InsertValues(string tableName, string columnName, string[] values, SqlConnection connection)
    {
      const string insertStatementPattern = "INSERT INTO [{0}] ({1}) VALUES ('{2}'); ";

      string query = string.Empty;

      foreach (var version in values)
      {
        query += string.Format(insertStatementPattern, tableName, columnName, version);
      }

      if (string.IsNullOrEmpty(query)) return;

      using (var transaction = connection.BeginTransaction())
      {
        using (var command = connection.CreateCommand())
        {
          command.CommandText = query;
          command.Transaction = transaction;
          command.ExecuteNonQuery();
        }

        transaction.Commit();
      }
    }
  
    private static bool TableExists(string tableName, SqlConnection connection)
    {
      using (var command = connection.CreateCommand())
      {
        command.CommandText = string.Format("SELECT COUNT(*) FROM sys.tables where name = '{0}'", tableName);

        int count = (int)command.ExecuteScalar();

        return count > 0;
      }
    }

    private static string GetUberDeployerConnectionString()
    {
      // TODO IMM HI: use in-memory sqlite
      return ConfigurationManager.ConnectionStrings["UberDeployer"].ConnectionString;
    }
  }
}
