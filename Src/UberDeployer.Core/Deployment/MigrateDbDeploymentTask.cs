using System;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Db;

namespace UberDeployer.Core.Deployment
{
  public class MigrateDbDeploymentTask : DeploymentTask
  {
    protected readonly IArtifactsRepository _artifactsRepository;
    protected readonly IDbScriptRunnerFactory _dbScriptRunnerFactory;
    protected readonly IDbVersionProvider _dbVersionProvider;

    #region Constructor(s)

    public MigrateDbDeploymentTask(
      IProjectInfoRepository projectInfoRepository,
      IEnvironmentInfoRepository environmentInfoRepository,
      IArtifactsRepository artifactsRepository,
      IDbScriptRunnerFactory dbScriptRunnerFactory,
      IDbVersionProvider dbVersionProvider)
      : base(projectInfoRepository, environmentInfoRepository)
    {
      Guard.NotNull(artifactsRepository, "artifactsRepository");
      Guard.NotNull(dbVersionProvider, "dbVersionProvider");
      Guard.NotNull(dbScriptRunnerFactory, "dbScriptRunnerFactory");

      _artifactsRepository = artifactsRepository;
      _dbScriptRunnerFactory = dbScriptRunnerFactory;
      _dbVersionProvider = dbVersionProvider;
    }

    #endregion

    #region Overrides of DeploymentTaskBase

    protected override void DoPrepare()
    {
      EnvironmentInfo environmentInfo = GetEnvironmentInfo();
      DbProjectInfo projectInfo = GetProjectInfo<DbProjectInfo>();

      // create a step for downloading the artifacts
      var downloadArtifactsDeploymentStep =
        new DownloadArtifactsDeploymentStep(
          projectInfo,
          _artifactsRepository,
          GetTempDirPath());

      AddSubTask(downloadArtifactsDeploymentStep);

      // create a step for extracting the artifacts
      var extractArtifactsDeploymentStep =
        new ExtractArtifactsDeploymentStep(
          projectInfo,
          environmentInfo,
          downloadArtifactsDeploymentStep.ArtifactsFilePath,
          GetTempDirPath());
      
      AddSubTask(extractArtifactsDeploymentStep);

      //var bin = new Func<ExtractArtifactsDeploymentStep, string>(e => e.BinariesDirPath);

      // create a step for gathering scripts to run
      var gatherDbScriptsToRunDeploymentStep =
        new GatherDbScriptsToRunDeploymentStep(
          projectInfo,
          new Lazy<string>(() => extractArtifactsDeploymentStep.BinariesDirPath),
          environmentInfo.DatabaseServerMachineName,
          environmentInfo.Name,
          _dbVersionProvider);

      AddSubTask(gatherDbScriptsToRunDeploymentStep);

      // create a step for running scripts
      var runDbScriptsDeploymentStep =
        new RunDbScriptsDeploymentStep(
          projectInfo,
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
            DeploymentInfo.ProjectName,
            DeploymentInfo.ProjectConfigurationName,
            DeploymentInfo.ProjectConfigurationBuildId,
            DeploymentInfo.TargetEnvironmentName);
      }
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

    #endregion Private helper methods
  }
}