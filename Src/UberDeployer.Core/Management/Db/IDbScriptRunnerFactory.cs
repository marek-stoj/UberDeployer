using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Management.Db
{
  public interface IDbScriptRunnerFactory
  {
    IDbScriptRunner CreateDbScriptRunner(bool usesSqlCmdUpgradeScripts, string databaseServerName,
      string databaseName);
  }
}