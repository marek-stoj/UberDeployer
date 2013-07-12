using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Management.Db;

namespace UberDeployer.Core.Deployment
{
  using System.Text.RegularExpressions;

  using UberDeployer.Core.DbDiff;

  public class RunDbScriptsDeploymentStep : DeploymentStep
  {
    private readonly string _databaseServerMachineName;

    private readonly IEnumerable<DbScriptToRun> _scriptPathsToRunEnumerable;

    private readonly IDbScriptRunner _dbScriptRunner;

    #region Constructor(s)

    public RunDbScriptsDeploymentStep(
      IDbScriptRunner dbScriptRunner, string databaseServerMachineName, IEnumerable<DbScriptToRun> scriptPathsToRunEnumerable)
    {
      Guard.NotNull(dbScriptRunner, "dbScriptRunner");
      Guard.NotNullNorEmpty(databaseServerMachineName, "databaseServerMachineName");

      if (scriptPathsToRunEnumerable == null)
      {
        throw new ArgumentNullException("scriptPathsToRunEnumerable");
      }

      _databaseServerMachineName = databaseServerMachineName;
      _scriptPathsToRunEnumerable = scriptPathsToRunEnumerable;
      _dbScriptRunner = dbScriptRunner;
    }

    #endregion

    #region Overrides of DeploymentStep

    protected override void DoExecute()
    {
      string executedScriptName = null;

      try
      {
        List<DbScriptToRun> scriptPathsToRunList = _scriptPathsToRunEnumerable.ToList();

        if (scriptPathsToRunList.Count <= 0)
        {
          PostDiagnosticMessage("There are no scripts to run.", DiagnosticMessageType.Info);
        }
        else
        {
          PostDiagnosticMessage(
            string.Format("Will run '{0}' scripts.", scriptPathsToRunList.Count), DiagnosticMessageType.Info);

          foreach (DbScriptToRun scriptPathToRun in scriptPathsToRunList)
          {
            executedScriptName = Path.GetFileName(scriptPathToRun.ScriptPath);

            using (var sr = new StreamReader(scriptPathToRun.ScriptPath))
            {
              string script = sr.ReadToEnd();

              EnsureVersionInsertIsPresent(scriptPathToRun.DbVersion, script);

              _dbScriptRunner.Execute(script);

              PostDiagnosticMessage("Script executed successfully: " + executedScriptName, DiagnosticMessageType.Info);
            }
          }
        }
      }
      catch (DbScriptRunnerException exc)
      {
        string message = string.Format("Script execution failed, script name: '{0}'.", executedScriptName);

        throw new DeploymentTaskException(message, exc);
      }
    }

    public override string Description
    {
      get
      {
        return string.Format("Run db scripts on server '{0}'.", _databaseServerMachineName);
      }
    }

    #endregion

    #region Private methods

    // ReSharper disable UnusedParameter.Local

    /// <summary>
    /// Checks if script makes insert into version history table
    /// </summary>
    private static void EnsureVersionInsertIsPresent(DbVersion dbVersion, string script)
    {
      var versionInsertRegexes = 
        new[] {
          string.Format("insert (into)? version(history)?(.*?){0}\\.{1}", dbVersion.Major, dbVersion.Minor),
          string.Format("insert into #temp (.*?)VALUES(.*?){0}\\.{1}", dbVersion.Major, dbVersion.Minor),
        }
      .Select(pattern => new Regex(pattern, RegexOptions.IgnoreCase));

      if (!versionInsertRegexes.Any(r => r.IsMatch(script)))
      {
        throw new DeploymentTaskException(string.Format("Script {0} that should be run does not have necessary version insert.", dbVersion));
      }
    }
    
    // ReSharper restore UnusedParameter.Local

    #endregion
  }
}
