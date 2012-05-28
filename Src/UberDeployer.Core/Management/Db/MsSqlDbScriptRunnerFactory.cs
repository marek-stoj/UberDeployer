namespace UberDeployer.Core.Management.Db
{
  public class MsSqlDbScriptRunnerFactory : IDbScriptRunnerFactory
  {
    #region IDbScriptRunnerFactory Members

    public IDbScriptRunner CreateDbScriptRunner(string databaseServerMachineName)
    {
      return new MsSqlDbScriptRunner(databaseServerMachineName);
    }

    #endregion
  }
}
