using System;
using System.Data.SqlClient;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace UberDeployer.Core.Management.Db
{
  public class MsSqlDbScriptRunner : IDbScriptRunner
  {
    private const string _ConnectionStringPattern = "Server={0};Integrated Security=SSPI";

    private readonly string _databaseServer;

    #region Constructor(s)

    public MsSqlDbScriptRunner(string databaseServer)
    {
      if (string.IsNullOrEmpty(databaseServer))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "databaseServer");
      }

      _databaseServer = databaseServer;
    }

    #endregion

    #region IDbScriptRunner Members

    public void Execute(string script)
    {
      if (string.IsNullOrEmpty(script))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "script");
      }

      try
      {
        using (SqlConnection connection = new SqlConnection(GetConnectionString()))
        {
          var errors = new StringBuilder();
          Server server = new Server(new ServerConnection(connection));

          server.ConnectionContext.ServerMessage += (o, eventArgs) => errors.AppendLine(eventArgs.ToString());
          server.ConnectionContext.InfoMessage += (o, eventArgs) => errors.AppendLine(eventArgs.ToString());
          server.ConnectionContext.ExecuteNonQuery(script);
        }
      }
      catch (Exception exc)
      {
        throw new DbScriptRunnerException(script, exc);
      }
    }

    #endregion

    #region Private methnods

    private string GetConnectionString()
    {
      return string.Format(_ConnectionStringPattern, _databaseServer);
    }

    #endregion
  }
}
