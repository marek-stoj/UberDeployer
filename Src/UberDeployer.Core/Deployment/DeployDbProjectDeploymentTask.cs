using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UberDeployer.Common.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Db;

namespace UberDeployer.Core.Deployment
{
  public class DeployDbProjectDeploymentTask : DeploymentTask
  {
    private readonly IArtifactsRepository _artifactsRepository;
    private readonly IDbScriptRunnerFactory _dbScriptRunnerFactory;
    private readonly IDbVersionProvider _dbVersionProvider;
    private readonly IFileAdapter _fileAdapter;
    private readonly IZipFileAdapter _zipFileAdapter;

    #region Constructor(s)

    public DeployDbProjectDeploymentTask(
      IProjectInfoRepository projectInfoRepository,
      IEnvironmentInfoRepository environmentInfoRepository,
      IArtifactsRepository artifactsRepository,
      IDbScriptRunnerFactory dbScriptRunnerFactory,
      IDbVersionProvider dbVersionProvider,
      IFileAdapter fileAdapter,
      IZipFileAdapter zipFileAdapter)
      : base(projectInfoRepository, environmentInfoRepository)
    {
      Guard.NotNull(artifactsRepository, "artifactsRepository");
      Guard.NotNull(dbVersionProvider, "dbVersionProvider");
      Guard.NotNull(dbScriptRunnerFactory, "dbScriptRunnerFactory");
      Guard.NotNull(fileAdapter, "fileAdapter");
      Guard.NotNull(zipFileAdapter, "zipFileAdapter");

      _artifactsRepository = artifactsRepository;
      _dbScriptRunnerFactory = dbScriptRunnerFactory;
      _dbVersionProvider = dbVersionProvider;
      _fileAdapter = fileAdapter;
      _zipFileAdapter = zipFileAdapter;
    }

    #endregion

    #region Overrides of DeploymentTask

    protected override void DoPrepare()
    {
      EnvironmentInfo environmentInfo = GetEnvironmentInfo();
      DbProjectInfo projectInfo = GetProjectInfo<DbProjectInfo>();

      DatabaseServer databaseServer =
        environmentInfo.GetDatabaseServer(projectInfo);

      string databaseServerMachineName = databaseServer.MachineName;

      // create a step for downloading the artifacts
      var downloadArtifactsDeploymentStep =
        new DownloadArtifactsDeploymentStep(
          projectInfo,
          DeploymentInfo,
          GetTempDirPath(),
          _artifactsRepository);

      AddSubTask(downloadArtifactsDeploymentStep);

      // create a step for extracting the artifacts
      var extractArtifactsDeploymentStep =
        new ExtractArtifactsDeploymentStep(
          projectInfo,
          environmentInfo,
          DeploymentInfo,
          downloadArtifactsDeploymentStep.ArtifactsFilePath,
          GetTempDirPath(),
          _fileAdapter,
          _zipFileAdapter);
      
      AddSubTask(extractArtifactsDeploymentStep);

      // create a step for gathering scripts to run
      var gatherDbScriptsToRunDeploymentStep =
        new GatherDbScriptsToRunDeploymentStep(
          projectInfo.DbName,
          new Lazy<string>(() => extractArtifactsDeploymentStep.BinariesDirPath),
          databaseServerMachineName,
          environmentInfo.Name,
          _dbVersionProvider);

      AddSubTask(gatherDbScriptsToRunDeploymentStep);

      // create a step for running scripts
      var runDbScriptsDeploymentStep =
        new RunDbScriptsDeploymentStep(
          GetScriptRunner(databaseServerMachineName),
          databaseServerMachineName,
          new DeferredEnumerable<DbScriptToRun>(() => gatherDbScriptsToRunDeploymentStep.ScriptsToRun));

      AddSubTask(runDbScriptsDeploymentStep);
    }

    protected override void Simulate()
    {
      foreach (DeploymentTaskBase subTask in SubTasks)
      {
        subTask.Execute();

        if (subTask is GatherDbScriptsToRunDeploymentStep)
        {
          var gatherDbScriptsToRunDeploymentStep =
            (GatherDbScriptsToRunDeploymentStep)subTask;

          List<string> scriptsToRun =
            gatherDbScriptsToRunDeploymentStep.ScriptsToRun
              .Select(str => Path.GetFileNameWithoutExtension(str.ScriptPath))
              .ToList();

          string diagnosticMessage =
            string.Format(
              "Will run {0} script(s): {1}.",
              scriptsToRun.Count,
              scriptsToRun.Count > 0 ? string.Join(", ", scriptsToRun) : "(none)");

          PostDiagnosticMessage(
            diagnosticMessage,
            DiagnosticMessageType.Info);

          break;
        }
      }
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Deploy db project '{0} ({1}:{2})' on '{3}'.",
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
