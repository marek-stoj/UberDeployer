namespace UberDeployer.Core.Deployment
{
  using System.Linq;
  using System.Text.RegularExpressions;

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

    // ReSharper disable UnusedParameter.Local

    /// <summary>
    /// Checks if script makes insert into version history table
    /// </summary>
    public static bool IsVersionInsertPresent(DbVersion dbVersion, string script)
    {
      var versionInsertRegexes =
        new[] {
          string.Format("insert\\s+(into)?\\s+\\[?version(history)?\\]?(.*?){0}\\.{1}", dbVersion.Major, dbVersion.Minor),
          string.Format("insert\\s+into\\s+#temp\\s+(.*?)VALUES(.*?){0}\\.{1}", dbVersion.Major, dbVersion.Minor),
        }
      .Select(pattern => new Regex(pattern, RegexOptions.IgnoreCase));

      return versionInsertRegexes.Any(r => r.IsMatch(script));
    }

    // ReSharper restore UnusedParameter.Local

  }
}