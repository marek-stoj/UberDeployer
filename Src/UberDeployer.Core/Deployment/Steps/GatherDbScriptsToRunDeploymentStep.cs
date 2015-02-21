using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.DbDiff;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Db;

namespace UberDeployer.Core.Deployment.Steps
{
  public class GatherDbScriptsToRunDeploymentStep : DeploymentStep
  {
    private const string _NoTransactionTail = ".notrans";

    private readonly string _dbName;
    private readonly Lazy<string> _scriptsDirectoryPathProvider;
    private readonly string _sqlServerName;
    private readonly string _environmentName;
    private readonly DeploymentInfo _deploymentInfo;
    private readonly IScriptsToRunWebSelector _scriptsToRunWebSelector;
    private readonly IDbVersionProvider _dbVersionProvider;

    private IEnumerable<DbScriptToRun> _scriptsToRun = new List<DbScriptToRun>();
    private IScriptsToRunWebSelector _scriptToRunWebSelector;

    public GatherDbScriptsToRunDeploymentStep(string dbName, Lazy<string> scriptsDirectoryPathProvider, string databaseServerMachineName, string environmentName, DeploymentInfo deploymentInfo, IDbVersionProvider scriptToRunWebSelector, IScriptsToRunWebSelector scriptsToRunWebSelector)
    {
      Guard.NotNullNorEmpty(dbName, "dbName");
      Guard.NotNull(scriptsDirectoryPathProvider, "scriptsDirectoryPathProvider");
      Guard.NotNullNorEmpty(databaseServerMachineName, "databaseServerMachineName");
      Guard.NotNullNorEmpty(environmentName, "environmentName");
      Guard.NotNull(deploymentInfo, "deploymentInfo");
      Guard.NotNull(scriptToRunWebSelector, "scriptToRunWebSelector");

      _dbName = dbName;
      _scriptsDirectoryPathProvider = scriptsDirectoryPathProvider;
      _sqlServerName = databaseServerMachineName;
      _environmentName = environmentName;
      _deploymentInfo = deploymentInfo;
      _scriptsToRunWebSelector = scriptsToRunWebSelector;

      _scriptsToRun = Enumerable.Empty<DbScriptToRun>();
    }

    protected override void DoExecute()
    {
      _scriptsToRun = GetScriptsToRun();
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Gather db scripts from '{0}' to run on database '{1}'.",
            _scriptsDirectoryPathProvider,
            _dbName);
      }
    }

    private static bool IsScriptSupported(DbVersion scriptVersion)
    {
      return string.IsNullOrEmpty(scriptVersion.Tail)
        || IsNoTransactionScript(scriptVersion);
    }

    private static bool IsNoTransactionScript(DbVersion dbVersion)
    {
      return string.Equals(dbVersion.Tail, _NoTransactionTail, StringComparison.OrdinalIgnoreCase);
    }

    private IEnumerable<DbScriptToRun> GetScriptsToRun()
    {
      // get db versions
      IEnumerable<string> versions =
        _dbVersionProvider.GetVersions(_dbName, _sqlServerName);

      var dbVersionsModel = new DbVersionsModel();

      dbVersionsModel.AddDatabase(_environmentName, _dbName, versions);

      // sort db versions
      List<DbVersion> dbVersionsList =
        dbVersionsModel.GetAllSortedDbVersions(_dbName)
          .Select(DbVersion.FromString)
          .ToList();

      DbVersion currentDbVersion = dbVersionsList.LastOrDefault();

      var dbVersionsSet = new HashSet<DbVersion>(dbVersionsList);

      // collect scripts that weren't executed on database
      string[] scriptFilePaths =
        Directory.GetFiles(
          _scriptsDirectoryPathProvider.Value,
          "*.sql",
          SearchOption.AllDirectories);

      Dictionary<DbVersion, string> scriptsToRunDict =
        (from filePath in scriptFilePaths
         let dbVersion = DbVersion.FromString(Path.GetFileNameWithoutExtension(filePath))
         where !dbVersionsSet.Contains(dbVersion)
         select new { dbVersion, filePath })
          .ToDictionary(x => x.dbVersion, x => x.filePath);

      Dictionary<DbVersion, string> scriptsNewerThanCurrentVersion =
        scriptsToRunDict
          .Where(kvp => currentDbVersion == null || kvp.Key.IsGreatherThan(currentDbVersion))
          .OrderBy(kvp => kvp.Key)
          .Select(x => x)
          .ToDictionary(x => x.Key, x => x.Value);

      IEnumerable<DbVersion> scriptsToRunOlderThanCurrentVersion =
        scriptsToRunDict.Keys.Except(scriptsNewerThanCurrentVersion.Keys)
          .OrderBy(v => v);

      foreach (DbVersion dbVersion in scriptsToRunOlderThanCurrentVersion)
      {
        if (!IsScriptSupported(dbVersion) || IsNoTransactionScript(dbVersion))
        {
          continue;
        }

        PostDiagnosticMessage(string.Format("This script should be run but it's older than the current version so we won't run it: '{0}'.", dbVersion), DiagnosticMessageType.Warn);
      }

      RemoveNotSupportedScripts(scriptsNewerThanCurrentVersion);

      List<DbScriptToRun> scriptsToRun =
        scriptsNewerThanCurrentVersion
          .Select(x => new DbScriptToRun(x.Key, x.Value))
          .ToList();

      string[] selectedScripts = _scriptToRunWebSelector.GetSelectedScriptsToRun(_deploymentInfo.DeploymentId, scriptsToRun.Select(s => Path.GetFileNameWithoutExtension(s.ScriptPath)).ToArray());

      return FilterScripts(scriptsToRun, selectedScripts);
    }

    private static IEnumerable<DbScriptToRun> FilterScripts(List<DbScriptToRun> scriptsToRun, string[] selectedScripts)
    {
      // TODO LK: we have to choose the way we want to filter our scripts (we could get all scripts below selected script for example)

      return scriptsToRun.Where(w => selectedScripts.Any(a => a == Path.GetFileNameWithoutExtension(w.ScriptPath)));
    }

    /// <summary>
    /// Removes script versions with tail - hotfixes etc.
    /// </summary>
    /// <param name="scripts"></param>
    private void RemoveNotSupportedScripts(IDictionary<DbVersion, string> scripts)
    {
      List<DbVersion> keysToRemove =
        (from scriptToRun in scripts
         where !IsScriptSupported(scriptToRun.Key)
         select scriptToRun.Key)
          .ToList();

      foreach (DbVersion keyToRemove in keysToRemove)
      {
        scripts.Remove(keyToRemove);

        PostDiagnosticMessage(string.Format("The following script is not supported and won't be run: '{0}'.", keyToRemove), DiagnosticMessageType.Warn);
      }
    }

    public IEnumerable<DbScriptToRun> ScriptsToRun
    {
      get { return _scriptsToRun; }
    }
  }
}