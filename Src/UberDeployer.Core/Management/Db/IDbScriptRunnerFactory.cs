namespace UberDeployer.Core.Management.Db
{
  public interface IDbScriptRunnerFactory
  {
    IDbScriptRunner CreateDbScriptRunner(string databaseServerMachineName);
  }
}
