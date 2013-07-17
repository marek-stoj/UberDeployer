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
          string.Format("insert\\s+(into)?\\s+\\[?version(history)?\\]?(.+?){0}\\.{1}{2}{3}", dbVersion.Major, dbVersion.Minor, RevisionRegex(dbVersion), BuildRegex(dbVersion)),
          string.Format("insert\\s+into\\s+#temp(.+?)VALUES(.*?){0}\\.{1}{2}{3}", dbVersion.Major, dbVersion.Minor, RevisionRegex(dbVersion), BuildRegex(dbVersion)),
        }
      .Select(pattern => new Regex(pattern, RegexOptions.IgnoreCase));

      return versionInsertRegexes.Any(r => r.IsMatch(script));
    }

    private static object RevisionRegex(DbVersion dbVersion)
    {
      if (dbVersion.Revision == 0 && dbVersion.Build == 0)
      {
        return "(\\.0)?";
      }

      return "\\." + dbVersion.Revision;
    }

    private static object BuildRegex(DbVersion dbVersion)
    {
      if (dbVersion.Build == 0)
      {
        return "(\\.0)?";
      }

      return "\\." + dbVersion.Build;
    }

    // ReSharper restore UnusedParameter.Local

  }
}