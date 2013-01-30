using System;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Db;

namespace UberDeployer.Core.Deployment
{
  public class MigrateDbDeploymentTask : DeploymentTask
  {
    protected readonly string _projectConfigurationName;
    protected readonly string _projectConfigurationBuildId;

    protected readonly DbProjectInfo _projectInfo;
    protected readonly IArtifactsRepository _artifactsRepository;
    protected readonly IDbScriptRunnerFactory _dbScriptRunnerFactory;
    protected readonly IDbVersionProvider _dbVersionProvider;

    #region Constructor(s)

    public MigrateDbDeploymentTask(
      IEnvironmentInfoRepository environmentInfoRepository,
      IArtifactsRepository artifactsRepository,
      IDbScriptRunnerFactory dbScriptRunnerFactory,
      IDbVersionProvider dbVersionProvider,
      DbProjectInfo projectInfo,
      string projectConfigurationName,
      string projectConfigurationBuildId,
      string targetEnvironmentName)
      : base(environmentInfoRepository, targetEnvironmentName)
    {

      if (artifactsRepository == null)
      {
        throw new ArgumentNullException("artifactsRepository");
      }

      if (projectInfo == null)
      {
        throw new ArgumentNullException("projectInfo");
      }

      if (string.IsNullOrEmpty(projectConfigurationName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "projectConfigurationName");
      }

      if (string.IsNullOrEmpty(projectConfigurationBuildId))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "projectConfigurationBuildId");
      }

      if (dbVersionProvider == null)
      {
        throw new ArgumentNullException("dbVersionProvider");
      }

      if (dbScriptRunnerFactory == null)
      {
        throw new ArgumentNullException("dbScriptRunnerFactory");
      }

      _artifactsRepository = artifactsRepository;
      _dbScriptRunnerFactory = dbScriptRunnerFactory;
      _dbVersionProvider = dbVersionProvider;
      _projectInfo = projectInfo;
      _projectConfigurationName = projectConfigurationName;
      _projectConfigurationBuildId = projectConfigurationBuildId;
    }

    #endregion

    #region Overrides of DeploymentTaskBase

    protected override void DoPrepare()
    {
      EnvironmentInfo environmentInfo = GetEnvironmentInfo();

      // create a step for downloading the artifacts
      var downloadArtifactsDeploymentStep =
        new DownloadArtifactsDeploymentStep(
          _artifactsRepository,
          _projectInfo,
          _projectConfigurationName,
          _projectConfigurationBuildId,
          GetTempDirPath());

      AddSubTask(downloadArtifactsDeploymentStep);

      // create a step for extracting the artifacts
      var extractArtifactsDeploymentStep =
        new ExtractArtifactsDeploymentStep(
          environmentInfo,
          _projectInfo,
          _projectConfigurationName,
          _projectConfigurationBuildId,
          downloadArtifactsDeploymentStep.ArtifactsFilePath,
          GetTempDirPath());

      AddSubTask(extractArtifactsDeploymentStep);

      // create a step for gathering scripts to run
      var gatherDbScriptsToRunDeploymentStep =
        new GatherDbScriptsToRunDeploymentStep(
          extractArtifactsDeploymentStep.BinariesDirPath,
          _projectInfo.DbName,
          environmentInfo.DatabaseServerMachineName,
          environmentInfo.Name,
          _dbVersionProvider);

      AddSubTask(gatherDbScriptsToRunDeploymentStep);

      // create a step for running scripts
      var runDbScriptsDeploymentStep =
        new RunDbScriptsDeploymentStep(
          GetScriptRunner(environmentInfo.DatabaseServerMachineName),
          environmentInfo.DatabaseServerMachineName,
          new DeferredEnumerable<string>(() => gatherDbScriptsToRunDeploymentStep.ScriptsToRun));

      AddSubTask(runDbScriptsDeploymentStep);
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Migrate db '{0} ({1}:{2})' on '{3}'.",
            _projectInfo.Name,
            _projectConfigurationName,
            _projectConfigurationBuildId,
            _targetEnvironmentName);
      }
    }

    #endregion

    #region Overrides of DeploymentTask

    public override string ProjectName
    {
      get { return _projectInfo.Name; }
    }

    public override string ProjectConfigurationName
    {
      get { return _projectConfigurationName; }
    }

    public override string ProjectConfigurationBuildId
    {
      get { return _projectConfigurationBuildId; }
    }
    #endregion

    #region Private helper methods

    private IDbScriptRunner GetScriptRunner(string databaseServerMachineName)
    {
      IDbScriptRunner scriptRunner =
        _dbScriptRunnerFactory.CreateDbScriptRunner(databaseServerMachineName);

      if (scriptRunner == null)
      {
        throw new DeploymentTaskException(string.Format("Can not create script runner for specified database server machine: '{0}'.", databaseServerMachineName));
      }

      return scriptRunner;
    }

    #endregion
  }
}
