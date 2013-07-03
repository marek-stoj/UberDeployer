using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.DbDiff;
using UberDeployer.Core.Management.Db;

namespace UberDeployer.Core.Deployment
{
  public class GatherDbScriptsToRunDeploymentStep : DeploymentStep
  {
    private readonly string _dbName;
    private readonly Lazy<string> _scriptsDirectoryPathProvider;
    private readonly string _sqlServerName;
    private readonly string _environmentName;
    private readonly IDbVersionProvider _dbVersionProvider;

    private IEnumerable<string> _scriptsToRun = new List<string>();

    #region Constructor(s)

    public GatherDbScriptsToRunDeploymentStep(string dbName, Lazy<string> scriptsDirectoryPathProvider, string sqlServerName, string environmentName, IDbVersionProvider dbVersionProvider)
    {
      Guard.NotNullNorEmpty(dbName, "dbName");
      Guard.NotNull(scriptsDirectoryPathProvider, "scriptsDirectoryPathProvider");
      Guard.NotNullNorEmpty(sqlServerName, "sqlServerName");
      Guard.NotNullNorEmpty(environmentName, "environmentName");
      Guard.NotNull(dbVersionProvider, "dbVersionProvider");
      
      _dbName = dbName;
      _scriptsDirectoryPathProvider = scriptsDirectoryPathProvider;
      _sqlServerName = sqlServerName;
      _environmentName = environmentName;
      _dbVersionProvider = dbVersionProvider;

      _scriptsToRun = Enumerable.Empty<string>();
    }

    #endregion

    #region Overrides of DeploymentStep

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

    #endregion

    #region Private methods

    private static bool IsScriptSupported(DbVersion scriptVersion)
    {
      return string.IsNullOrEmpty(scriptVersion.Tail);
    }

    private IEnumerable<string> GetScriptsToRun()
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
          SearchOption.TopDirectoryOnly);

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
        scriptsToRunDict.Keys.Except(scriptsNewerThanCurrentVersion.Keys);

      foreach (DbVersion dbVersion in scriptsToRunOlderThanCurrentVersion)
      {
        if (!IsScriptSupported(dbVersion))
        {
          continue;
        }

        PostDiagnosticMessage(string.Format("This script should be run but it's older than the current version so we won't run it: '{0}'.", dbVersion), DiagnosticMessageType.Warn);
      }

      RemoveNotSupportedScripts(scriptsNewerThanCurrentVersion);

      List<string> scriptsToRun =
        scriptsNewerThanCurrentVersion
          .Select(x => x.Value)
          .ToList();

      return scriptsToRun;
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

    #endregion

    #region Properties

    public IEnumerable<string> ScriptsToRun
    {
      get { return _scriptsToRun; }
    }

    #endregion
  }
}
