using System;
using System.IO;
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

    private SchedulerAppProjectInfo _projectInfo
    {
      get { return (SchedulerAppProjectInfo)DeploymentInfo.ProjectInfo; }
    }

    #region Constructor(s)

    public DeploySchedulerAppDeploymentTask(
      IEnvironmentInfoRepository environmentInfoRepository,
      IArtifactsRepository artifactsRepository,
      ITaskScheduler taskScheduler,
      IPasswordCollector passwordCollector)
      : base(environmentInfoRepository)
    {
      if (artifactsRepository == null)
      {
        throw new ArgumentNullException("artifactsRepository");
      }

      if (taskScheduler == null)
      {
        throw new ArgumentNullException("taskScheduler");
      }

      if (passwordCollector == null)
      {
        throw new ArgumentNullException("passwordCollector");
      }

      _artifactsRepository = artifactsRepository;
      _taskScheduler = taskScheduler;
      _passwordCollector = passwordCollector;
    }

    #endregion Constructor(s)

    #region Overrides of DeploymentTaskBase

    protected override void DoPrepare()
    {
      EnvironmentInfo environmentInfo = GetEnvironmentInfo();

      string machineName = environmentInfo.AppServerMachineName;
      string taskName = _projectInfo.SchedulerAppName;
      string targetDirPath = Path.Combine(environmentInfo.SchedulerAppsBaseDirPath, _projectInfo.SchedulerAppDirName);
      string targetDirNetworkPath = environmentInfo.GetAppServerNetworkPath(targetDirPath);
      string executablePath = Path.Combine(targetDirPath, _projectInfo.SchedulerAppExeName);

      ScheduledTaskDetails taskDetails = _taskScheduler.GetScheduledTaskDetails(machineName, taskName);

      if (taskDetails != null && taskDetails.IsRunning)
      {
        throw new DeploymentTaskException(string.Format(
          "Task: {0} on machine: {1} is already running. Deployment aborted. Last run time: {2}, next run time: {3}",
          environmentInfo.AppServerMachineName,
          _projectInfo.SchedulerAppName,
          taskDetails.LastRunTime,
          taskDetails.NextRunTime));
      }

      // create a step to disable scheduler app.
      AddSubTask(
        new EnableSchedulerAppStep(
          _taskScheduler,
          machineName,
          false));

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

      // create a backup step if needed
      if (Directory.Exists(targetDirNetworkPath))
      {
        AddSubTask(new BackupFilesDeploymentStep(targetDirNetworkPath));
      }

      // create a step for copying the binaries to the target machine
      AddSubTask(
        new CopyFilesDeploymentStep(
          new Lazy<string>(() => extractArtifactsDeploymentStep.BinariesDirPath),
          new Lazy<string>(() => environmentInfo.GetAppServerNetworkPath(targetDirPath))));

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
            machineName,
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
            machineName,
            executablePath,
            environmentUser.UserName,
            environmentUserPassword));
      }

      // create a step to enable scheduler app.
      AddSubTask(
        new EnableSchedulerAppStep(
          _taskScheduler,
          machineName,
          true));
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
  }
}