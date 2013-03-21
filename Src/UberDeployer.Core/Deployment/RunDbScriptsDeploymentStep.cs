using System;
using System.Collections.Generic;
using System.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Db;

namespace UberDeployer.Core.Deployment
{
  public class RunDbScriptsDeploymentStep : DeploymentStep
  {
    private readonly string _databaseServerMachineName;
    private readonly IEnumerable<string> _scriptPathsToRunEnumerable;
    private readonly IDbScriptRunner _dbScriptRunner;

    #region Constructor(s)

    public RunDbScriptsDeploymentStep(ProjectInfo projectInfo, IDbScriptRunner dbScriptRunner, string databaseServerMachineName, IEnumerable<string> scriptPathsToRunEnumerable)
      : base(projectInfo)
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
        foreach (string scriptPathToRun in _scriptPathsToRunEnumerable)
        {
          executedScriptName = Path.GetFileName(scriptPathToRun);

          using (var sr = new StreamReader(scriptPathToRun))
          {
            string script = sr.ReadToEnd();

            _dbScriptRunner.Execute(script);

            PostDiagnosticMessage("Script executed successfully: " + executedScriptName, DiagnosticMessageType.Info);
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
        return
          string.Format(
            "Run db scripts on server '{0}'.",
            _databaseServerMachineName);
      }
    }

    #endregion
  }
}
