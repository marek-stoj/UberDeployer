﻿using System;
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
      IEnvironmentInfoRepository environmentInfoRepository,
      IArtifactsRepository artifactsRepository,
      IDbScriptRunnerFactory dbScriptRunnerFactory,
      IDbVersionProvider dbVersionProvider)
      : base(environmentInfoRepository)
    {
      if (artifactsRepository == null)
      {
        throw new ArgumentNullException("artifactsRepository");
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
    }

    #endregion Constructor(s)

    #region Overrides of DeploymentTaskBase

    protected override void DoPrepare()
    {
      EnvironmentInfo environmentInfo = GetEnvironmentInfo();

      // create a step for downloading the artifacts
      var downloadArtifactsDeploymentStep =
        new DownloadArtifactsDeploymentStep(
          _artifactsRepository,
          GetTempDirPath());

      AddSubTask(downloadArtifactsDeploymentStep);

      // create a step for extracting the artifacts
      var extractArtifactsDeploymentStep =
        new ExtractArtifactsDeploymentStep(
          environmentInfo,
          downloadArtifactsDeploymentStep.ArtifactsFilePath,
          GetTempDirPath());
      
      AddSubTask(extractArtifactsDeploymentStep);

      // create a step for gathering scripts to run
      var gatherDbScriptsToRunDeploymentStep =
        new GatherDbScriptsToRunDeploymentStep(
          extractArtifactsDeploymentStep.BinariesDirPath,
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
            DeploymentInfo.ProjectName,
            DeploymentInfo.ProjectConfigurationName,
            DeploymentInfo.ProjectConfigurationBuildId,
            DeploymentInfo.TargetEnvironmentName);
      }
    }

    #endregion Overrides of DeploymentTaskBase

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