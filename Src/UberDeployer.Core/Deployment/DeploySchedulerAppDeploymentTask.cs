using System;
using System.IO;
using UberDeployer.Common.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.ScheduledTasks;

namespace UberDeployer.Core.Deployment
{
  // TODO IMM HI: move common code up
  public class DeploySchedulerAppDeploymentTask : DeploymentTask
  {
    private readonly IArtifactsRepository _artifactsRepository;
    private readonly ITaskScheduler _taskScheduler;
    private readonly IPasswordCollector _passwordCollector;
    private readonly IDirectoryAdapter _directoryAdapter;

    private SchedulerAppProjectInfo _projectInfo
    {
      get { return (SchedulerAppProjectInfo)DeploymentInfo.ProjectInfo; }
    }

    #region Constructor(s)

    public DeploySchedulerAppDeploymentTask(
      IEnvironmentInfoRepository environmentInfoRepository,
      IArtifactsRepository artifactsRepository,
      ITaskScheduler taskScheduler,
      IPasswordCollector passwordCollector,
      IDirectoryAdapter directoryAdapter)
      : base(environmentInfoRepository)
    {
      Guard.NotNull(artifactsRepository, "artifactsRepository");
      Guard.NotNull(taskScheduler, "taskScheduler");
      Guard.NotNull(passwordCollector, "passwordCollector");
      Guard.NotNull(directoryAdapter, "directoryAdapter");      

      _artifactsRepository = artifactsRepository;
      _taskScheduler = taskScheduler;
      _passwordCollector = passwordCollector;
      _directoryAdapter = directoryAdapter;
    }

    #endregion Constructor(s)

    #region Overrides of DeploymentTaskBase

    protected override void DoPrepare()
    {
      EnvironmentInfo environmentInfo = GetEnvironmentInfo();

      string machineName = environmentInfo.AppServerMachineName;
      string targetDirPath = Path.Combine(environmentInfo.SchedulerAppsBaseDirPath, _projectInfo.SchedulerAppDirName);
      string targetDirNetworkPath = environmentInfo.GetAppServerNetworkPath(targetDirPath);
      string executablePath = Path.Combine(targetDirPath, _projectInfo.SchedulerAppExeName);

      if (!_directoryAdapter.Exists(targetDirNetworkPath))
      {
        throw new DeploymentTaskException(string.Format("Target directory does not exist: {0}", targetDirNetworkPath));
      }

      ScheduledTaskDetails taskDetails = _taskScheduler.GetScheduledTaskDetails(machineName, _projectInfo.SchedulerAppName);

      CheckIfTaskIsRunning(taskDetails, environmentInfo);

      // create a step to disable scheduler app.
      AddEnableTaskStep(machineName, false);

      Lazy<string> binariesDirPathProvider = AddStepsToObtainBinaries(environmentInfo);

      AddSubTask(new BackupFilesDeploymentStep(targetDirNetworkPath));

      // create a step for copying the binaries to the target machine
      AddSubTask(
        new CopyFilesDeploymentStep(
          binariesDirPathProvider,
          new Lazy<string>(() => targetDirNetworkPath)));

      AddTaskConfiguraitonSteps(taskDetails, environmentInfo, executablePath);

      // create a step to enable scheduler app.
      AddEnableTaskStep(machineName, true);
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Deploy scheduler app '{0} ({1}:{2})' to '{3}'.",
            _projectInfo.Name,
            DeploymentInfo.ProjectConfigurationName,
            DeploymentInfo.ProjectConfigurationBuildId,
            DeploymentInfo.TargetEnvironmentName);
      }
    }

    #endregion Overrides of DeploymentTaskBase

    private bool HasSettingsChanged(ScheduledTaskDetails taskDetails, string executablePath)
    {
      if (taskDetails == null)
      {
        return false;
      }

      return !(taskDetails.Name == _projectInfo.SchedulerAppName
               && taskDetails.ScheduledHour == _projectInfo.ScheduledHour
               && taskDetails.ScheduledMinute == _projectInfo.ScheduledMinute
               && taskDetails.ExecutionTimeLimitInMinutes == _projectInfo.ExecutionTimeLimitInMinutes
               && taskDetails.ExeAbsolutePath == executablePath);
    }

    private void AddTaskConfiguraitonSteps(ScheduledTaskDetails taskDetails, EnvironmentInfo environmentInfo, string executablePath)
    {
      bool hasSettingsChanged = HasSettingsChanged(taskDetails, executablePath);
      bool taskExists = taskDetails != null;

      EnvironmentUser environmentUser = null;
      string environmentUserPassword = null;

      if (!taskExists || hasSettingsChanged)
      {
        // collect password
        environmentUserPassword =
          PasswordCollectorHelper.CollectPasssword(
            _passwordCollector,
            environmentInfo,
            environmentInfo.AppServerMachineName,
            _projectInfo.SchedulerAppUserId,
            out environmentUser);
      }

      if (!taskExists)
      {
        // create a step for scheduling a new app
        AddSubTask(
          new ScheduleNewAppDeploymentStep(
            _taskScheduler,
            environmentInfo.AppServerMachineName,
            executablePath,
            environmentUser.UserName,
            environmentUserPassword));
      }
      else if (hasSettingsChanged)
      {
        // create a step for updating an existing scheduler app
        AddSubTask(
          new UpdateAppScheduleDeploymentStep(
            _taskScheduler,
            environmentInfo.AppServerMachineName,
            executablePath,
            environmentUser.UserName,
            environmentUserPassword));
      }
    }

    private Lazy<string> AddStepsToObtainBinaries(EnvironmentInfo environmentInfo)
    {
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

      if (_projectInfo.ArtifactsAreEnvironmentSpecific)
      {
        var binariesConfiguratorStep = new ConfigureBinariesStep(
          environmentInfo.ConfigurationTemplateName, GetTempDirPath());

        AddSubTask(binariesConfiguratorStep);
      }

      return new Lazy<string>(() => extractArtifactsDeploymentStep.BinariesDirPath);
    }

    private void CheckIfTaskIsRunning(ScheduledTaskDetails taskDetails, EnvironmentInfo environmentInfo)
    {
      if (taskDetails != null && taskDetails.IsRunning)
      {
        throw new DeploymentTaskException(string.Format(
          "Task: {0} on machine: {1} is already running. Deployment aborted. Last run time: {2}, next run time: {3}",
          environmentInfo.AppServerMachineName,
          _projectInfo.SchedulerAppName,
          taskDetails.LastRunTime,
          taskDetails.NextRunTime));
      }
    }

    private void AddEnableTaskStep(string machineName, bool enable)
    {
      AddSubTask(
        new EnableSchedulerAppStep(
          _taskScheduler,
          machineName,
          enable));
    }

    private void AddBackupStep(string targetDirNetworkPath)
    {
      if (_directoryAdapter.Exists(targetDirNetworkPath))
      {
        AddSubTask(new BackupFilesDeploymentStep(targetDirNetworkPath));
      }
      else
      {
        throw new DeploymentTaskException(string.Format("Target directory does not exist: {0}", targetDirNetworkPath));
      }
    }
  }
}