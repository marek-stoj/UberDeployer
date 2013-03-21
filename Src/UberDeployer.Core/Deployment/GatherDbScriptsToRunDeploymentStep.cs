using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.DbDiff;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Db;

namespace UberDeployer.Core.Deployment
{
  public class GatherDbScriptsToRunDeploymentStep : DeploymentStep
  {
    private readonly Lazy<string> _scriptsDirectoryPathProvider;
    private readonly string _sqlServerName;
    private readonly string _environmentName;
    private readonly IDbVersionProvider _dbVersionProvider;

    private IEnumerable<string> _scriptsToRun = new List<string>();
    private DbProjectInfo _dbProjectInfo;

    #region Constructor(s)

    public GatherDbScriptsToRunDeploymentStep(ProjectInfo projectInfo, Lazy<string> scriptsDirectoryPathProvider, string sqlServerName, string environmentName, IDbVersionProvider dbVersionProvider)
      : base(projectInfo)
    {
      Guard.NotNull(scriptsDirectoryPathProvider, "scriptsDirectoryPathProvider");
      Guard.NotNullNorEmpty(sqlServerName, "sqlServerName");
      Guard.NotNullNorEmpty(environmentName, "environmentName");
      Guard.NotNull(dbVersionProvider, "dbVersionProvider");
      
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
      _dbProjectInfo = (DbProjectInfo)ProjectInfo;

      // get db versions
      var versions = _dbVersionProvider.GetVersions(_dbProjectInfo.DbName, _sqlServerName);

      var dbVersionsModel = new DbVersionsModel();
      dbVersionsModel.AddDatabase(_environmentName, _dbProjectInfo.DbName, versions);

      // sort db versions
      List<DbVersion> dbVersionsList =
        dbVersionsModel.GetAllSortedDbVersions(_dbProjectInfo.DbName)
          .Select(DbVersion.FromString)
          .ToList();

      DbVersion currentDbVersion = dbVersionsList.LastOrDefault();

      var dbVersionsSet = new HashSet<DbVersion>(dbVersionsList);

      // collect scripts that weren't executed on database
      Dictionary<DbVersion, string> scriptsToRunDict =
        (from filePath in Directory.GetFiles(_scriptsDirectoryPathProvider.Value, "*.sql")
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
        PostDiagnosticMessage("The following script has not been run yet: " + keyToRemove, DiagnosticMessageType.Warn);
      }
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Gather db scripts from '{0}' to run on database '{1}'.",
            _scriptsDirectoryPathProvider,
            ((DbProjectInfo)ProjectInfo).DbName);
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
