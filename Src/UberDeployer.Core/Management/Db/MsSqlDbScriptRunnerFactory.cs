using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Management.Db
{
  public class MsSqlDbScriptRunnerFactory : IDbScriptRunnerFactory
  {
    public IDbScriptRunner CreateDbScriptRunner(bool usesSqlCmdUpgradeScripts, string databaseServerName, string databaseName)
    {
      if (usesSqlCmdUpgradeScripts)
      {
        return new MsSqlSqlCmdScriptRunner(databaseServerName, databaseName);
      }

      return new MsSqlDbScriptRunner(databaseServerName);
    }
  }
}