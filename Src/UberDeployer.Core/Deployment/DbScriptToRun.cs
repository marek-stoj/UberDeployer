namespace UberDeployer.Core.Deployment
{
  using UberDeployer.Common.SyntaxSugar;
  using UberDeployer.Core.DbDiff;

  public class DbScriptToRun
  {
    public DbVersion DbVersion { get; set; }
    public string ScriptPath { get; set; }

    public DbScriptToRun(DbVersion dbVersion, string scriptPath)
    {
      Guard.NotNull(dbVersion);
      Guard.NotNullNorEmpty(scriptPath);

      this.DbVersion = dbVersion;
      this.ScriptPath = scriptPath;
    }
  }
}