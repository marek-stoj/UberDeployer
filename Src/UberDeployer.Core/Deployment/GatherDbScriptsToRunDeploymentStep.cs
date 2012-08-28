using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UberDeployer.Core.DbDiff;
using UberDeployer.Core.Management.Db;

namespace UberDeployer.Core.Deployment
{
  public class GatherDbScriptsToRunDeploymentStep : DeploymentStep
  {
    private readonly string _scriptsDirectoryPath;
    private readonly string _databaseName;
    private readonly string _sqlServerName;
    private readonly string _environmentName;
    private readonly IDbVersionProvider _dbVersionProvider;

    private IEnumerable<string> _scriptsToRun = new List<string>();

    #region Constructor(s)

    public GatherDbScriptsToRunDeploymentStep(string scriptsDirectoryPath, string databaseName, string sqlServerName, string environmentName, IDbVersionProvider dbVersionProvider)
    {
      if (string.IsNullOrEmpty(scriptsDirectoryPath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "scriptsDirectoryPath");
      }

      if (string.IsNullOrEmpty(databaseName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "databaseName");
      }

      if (string.IsNullOrEmpty(sqlServerName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "sqlServerName");
      }

      if (string.IsNullOrEmpty(environmentName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "environmentName");
      }

      if (dbVersionProvider == null)
      {
        throw new ArgumentNullException("dbVersionProvider");
      }

      _scriptsDirectoryPath = scriptsDirectoryPath;
      _databaseName = databaseName;
      _sqlServerName = sqlServerName;
      _environmentName = environmentName;
      _dbVersionProvider = dbVersionProvider;

      _scriptsToRun = Enumerable.Empty<string>();
    }

    #endregion

    #region Overrides of DeploymentStep

    protected override void DoExecute()
    {
      // get db versions
      var versions = _dbVersionProvider.GetVersions(_databaseName, _sqlServerName);

      var dbVersionsModel = new DbVersionsModel();
      dbVersionsModel.AddDatabase(_environmentName, _databaseName, versions);

      // sort db versions
      List<DbVersion> dbVersionsList =
        dbVersionsModel.GetAllSortedDbVersions(_databaseName)
          .Select(DbVersion.FromString)
          .ToList();

      DbVersion currentDbVersion = dbVersionsList.LastOrDefault();

      var dbVersionsSet = new HashSet<DbVersion>(dbVersionsList);

      // collect scripts that weren't executed on database
      Dictionary<DbVersion, string> scriptsToRunDict =
        (from filePath in Directory.GetFiles(_scriptsDirectoryPath, "*.sql")
         let dbVersion = DbVersion.FromString(Path.GetFileNameWithoutExtension(filePath))
         where !dbVersionsSet.Contains(dbVersion)
         select new { dbVersion, filePath })
         .ToDictionary(unknown => unknown.dbVersion, unknown => unknown.filePath);
      
      var scriptsNewerThanCurrentVersion =
        scriptsToRunDict
          .Where(kvp => currentDbVersion == null || kvp.Key.IsGreatherThan(currentDbVersion))
          .OrderBy(kvp => kvp.Key)
          .Select(x => x)
          .ToDictionary(x => x.Key, x => x.Value);
      
      RemoveNotSupportedScripts(scriptsNewerThanCurrentVersion);

      _scriptsToRun = 
        scriptsNewerThanCurrentVersion
        .Select(x => x.Value)
        .ToList();
    }

    /// <summary>
    /// Removes script versions with tail - hotfixes etc.
    /// </summary>
    /// <param name="scriptsToRun"></param>
    private void RemoveNotSupportedScripts(Dictionary<DbVersion, string> scriptsToRun)
    {
      var keysToRemove =
        (from scriptToRun in scriptsToRun
         where !string.IsNullOrEmpty(scriptToRun.Key.Tail)
         select scriptToRun.Key)
          .ToList();

      foreach (var keyToRemove in keysToRemove)
      {
        scriptsToRun.Remove(keyToRemove);
        PostDiagnosticMessage("The following script has not been run yet: " + keyToRemove, DiagnosticMessageType.Warning);
      }
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Gather db scripts from '{0}' to run on database '{1}'.",
            _scriptsDirectoryPath,
            _databaseName);
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
