using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using UberDeployer.Core.DataAccess.Dapper;

namespace UberDeployer.Core.Management.Db
{
  public class DbVersionProvider : IDbVersionProvider
  {
    private const string _ConnectionStringPattern = "Server={0};Integrated Security=SSPI";

    private readonly List<DbVersionTableInfo> _versionTableInfos;    

    public DbVersionProvider(IEnumerable<DbVersionTableInfo> versionTableInfos)
    {
      if (versionTableInfos == null)
      {
        throw new ArgumentNullException("versionTableInfos");
      }      

      _versionTableInfos = new List<DbVersionTableInfo>(versionTableInfos);
    }

    public IEnumerable<string> GetVersions(string dbName, string sqlServerName)
    {
      if (string.IsNullOrEmpty(dbName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "dbName");
      }

      if (string.IsNullOrEmpty(sqlServerName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "sqlServerName");
      }
      
      string connectionString = string.Format(_ConnectionStringPattern, sqlServerName);

      using (var connection = new SqlConnection(connectionString))
      {
        connection.Open();

        DbVersionTableInfo versionTableInfo =
          GetVersionTableInfo(dbName, connection);

        if (versionTableInfo == null)
        {
          return new List<string>();
        }

        string versionQuery = string.Format(
          "use [{0}]" + "\r\n" +
          "select [{1}] from [{2}]",
          dbName,
          versionTableInfo.ColumnName,
          versionTableInfo.TableName);

        IEnumerable<dynamic> dbVersions = connection.Query(versionQuery);

        return dbVersions
          .Select(
            dbVersion =>
              {
                object value = ((IDictionary<string, object>)dbVersion)[versionTableInfo.ColumnName];

                return value != null ? (string)value : null;
              })
          .ToList();
      }
    }

    private DbVersionTableInfo GetVersionTableInfo(string dbName, SqlConnection connection)
    {
      IEnumerable<dynamic> tables = connection.Query(string.Format(
        "use [{0}]" + "\r\n" +
        "select * from sys.tables",
        dbName));
      
      HashSet<string> tableNames = new HashSet<string>(tables.Select(t => ((string)t.name).ToUpper()));

      foreach (var versionTableInfo in _versionTableInfos)
      {
        if (tableNames.Contains(versionTableInfo.TableName.ToUpper())
         && TableContainsColumn(connection, dbName, versionTableInfo.TableName, versionTableInfo.ColumnName))
        {
          return versionTableInfo;
        }
      }

      return null;
    }

    private static bool TableContainsColumn(SqlConnection dbConnection, string databaseName, string tableName, string columnName)
    {
      IEnumerable<dynamic> result =
        dbConnection.Query(
          string.Format(
            "use [{0}]" + "\r\n" +
            "select * from sys.columns c" + "\r\n" +
            "join sys.tables t on t.[object_id] = c.[object_id]" + "\r\n" +
            "where t.name = '{1}' and c.name = '{2}'",
            databaseName,
            tableName,
            columnName));

      return result.Any();
    }
  }
}
