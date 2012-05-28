namespace UberDeployer.Core.Management.Db
{
  public interface IDbScriptRunner
  {
    void Execute(string script);
  }
}
