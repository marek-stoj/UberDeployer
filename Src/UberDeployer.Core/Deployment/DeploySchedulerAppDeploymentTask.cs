using System;
using System.Collections.Generic;
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
    
    private SchedulerAppProjectInfo _projectInfo;

    #region Constructor(s)

    public DeploySchedulerAppDeploymentTask(
      IProjectInfoRepository projectInfoRepository,
      IEnvironmentInfoRepository environmentInfoRepository,
      IArtifactsRepository artifactsRepository,
      ITaskScheduler taskScheduler,
      IPasswordCollector passwordCollector,
      IDirectoryAdapter directoryAdapter)
      : base(projectInfoRepository, environmentInfoRepository)
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

    #endregion

    #region Overrides of DeploymentTaskBase

    protected override void DoPrepare()
    {
      EnvironmentInfo environmentInfo = GetEnvironmentInfo();
      _projectInfo = GetProjectInfo<SchedulerAppProjectInfo>();

      string machineName = environmentInfo.SchedulerServerMachineName;
      string targetDirPath = GetTargetDirPath(environmentInfo);
      string targetDirNetworkPath = environmentInfo.GetSchedulerServerNetworkPath(targetDirPath);

      var taskDetailsByName = new Dictionary<string, ScheduledTaskDetails>();

      _projectInfo.SchedulerTasks
        .ForEach(
          schedulerAppTask =>
          {
            ScheduledTaskDetails taskDetails =
              _taskScheduler.GetScheduledTaskDetails(machineName, schedulerAppTask.Name);

            taskDetailsByName.Add(schedulerAppTask.Name, taskDetails);

            EnsureTaskIsNotRunning(taskDetails, environmentInfo);

            // create a step to disable scheduler task
            if (taskDetails != null)
            {
              AddToggleSchedulerAppTaskEnabledStep(machineName, taskDetails.Name, false);
            }
          });

      Lazy<string> binariesDirPathProvider = AddStepsToObtainBinaries(environmentInfo);

      if (_directoryAdapter.Exists(targetDirNetworkPath))
      {
        AddSubTask(
          new BackupFilesDeploymentStep(targetDirNetworkPath));
      }

      // create a step for copying the binaries to the target machine
      AddSubTask(
        new CopyFilesDeploymentStep(
          binariesDirPathProvider,
          new Lazy<string>(() => targetDirNetworkPath)));

      _projectInfo.SchedulerTasks
        .ForEach(
          schedulerAppTask =>
          {
            ScheduledTaskDetails taskDetails =
              taskDetailsByName[schedulerAppTask.Name];

            AddTaskConfigurationSteps(
              environmentInfo,
              schedulerAppTask,
              taskDetails);

            // create a step to toggle scheduler task enabled
            AddToggleSchedulerAppTaskEnabledStep(
              machineName,
              schedulerAppTask.Name,
              true);
          });
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Deploy scheduler app '{0} ({1}:{2})' to '{3}'.",
            DeploymentInfo.ProjectName,
            DeploymentInfo.ProjectConfigurationName,
            DeploymentInfo.ProjectConfigurationBuildId,
            DeploymentInfo.TargetEnvironmentName);
      }
    }

    #endregion

    #region Private methods

    private void AddTaskConfigurationSteps(EnvironmentInfo environmentInfo, SchedulerAppTask schedulerAppTask, ScheduledTaskDetails taskDetails = null)
    {
      bool hasSettingsChanged = HasSettingsChanged(taskDetails, schedulerAppTask, environmentInfo);
      bool taskExists = taskDetails != null;

      EnvironmentUser environmentUser = null;
      string environmentUserPassword = null;

      if (!taskExists || hasSettingsChanged)
      {
        // collect password
        environmentUserPassword =
          PasswordCollectorHelper.CollectPasssword(
            _passwordCollector,
            DeploymentInfo.DeploymentId,
            environmentInfo,
            environmentInfo.SchedulerServerMachineName,
            schedulerAppTask.UserId,
            OnDiagnosticMessagePosted,
            out environmentUser);
      }

      string taskExecutablePath =
        GetTaskExecutablePath(schedulerAppTask, environmentInfo);

      if (!taskExists)
      {
        // create a step for scheduling a new app
        AddSubTask(
          new CreateSchedulerTaskDeploymentStep(
            environmentInfo.SchedulerServerMachineName,
            schedulerAppTask.Name,
            taskExecutablePath,
            environmentUser.UserName,
            environmentUserPassword,
            schedulerAppTask.ScheduledHour,
            schedulerAppTask.ScheduledMinute,
            schedulerAppTask.ExecutionTimeLimitInMinutes,
            _taskScheduler));
      }
      else if (hasSettingsChanged)
      {
        // create a step for updating an existing scheduler app
        AddSubTask(
          new UpdateSchedulerTaskDeploymentStep(
            environmentInfo.SchedulerServerMachineName,
            schedulerAppTask.Name,
            taskExecutablePath,
            environmentUser.UserName,
            environmentUserPassword,
            schedulerAppTask.ScheduledHour,
            schedulerAppTask.ScheduledMinute,
            schedulerAppTask.ExecutionTimeLimitInMinutes,
            _taskScheduler));
      }
    }

    private Lazy<string> AddStepsToObtainBinaries(EnvironmentInfo environmentInfo)
    {
      // create a step for downloading the artifacts
      var downloadArtifactsDeploymentStep =
        new DownloadArtifactsDeploymentStep(
          _projectInfo,
          DeploymentInfo,
          GetTempDirPath(),
          _artifactsRepository);

      AddSubTask(downloadArtifactsDeploymentStep);

      // create a step for extracting the artifacts
      var extractArtifactsDeploymentStep =
        new ExtractArtifactsDeploymentStep(
          _projectInfo, 
          environmentInfo,
          DeploymentInfo,
          downloadArtifactsDeploymentStep.ArtifactsFilePath,
          GetTempDirPath());

      AddSubTask(extractArtifactsDeploymentStep);

      if (_projectInfo.ArtifactsAreEnvironmentSpecific)
      {
        var binariesConfiguratorStep =
          new ConfigureBinariesStep(
            environmentInfo.ConfigurationTemplateName,
            GetTempDirPath());

        AddSubTask(binariesConfiguratorStep);
      }

      return new Lazy<string>(() => extractArtifactsDeploymentStep.BinariesDirPath);
    }

    private void AddToggleSchedulerAppTaskEnabledStep(string machineName, string taskName, bool enabled)
    {
      AddSubTask(
        new ToggleSchedulerAppEnabledStep(
          _taskScheduler,
          machineName,
          taskName,
          enabled));
    }

    private string GetTargetDirPath(EnvironmentInfo environmentInfo)
    {
      return Path.Combine(environmentInfo.SchedulerAppsBaseDirPath, _projectInfo.SchedulerAppDirName);
    }

    private string GetTaskExecutablePath(SchedulerAppTask schedulerAppTask, EnvironmentInfo environmentInfo)
    {
      string targetDirPath = GetTargetDirPath(environmentInfo);

      return Path.Combine(targetDirPath, schedulerAppTask.ExecutableName);
    }

    private static void EnsureTaskIsNotRunning(ScheduledTaskDetails taskDetails, EnvironmentInfo environmentInfo)
    {
      if (taskDetails == null || !taskDetails.IsRunning)
      {
        return;
      }

      throw
        new DeploymentTaskException(
          string.Format(
            "Task: {0} on machine: {1} is already running. Deployment aborted. Last run time: {2}, next run time: {3}",
            environmentInfo.SchedulerServerMachineName,
            taskDetails.Name,
            taskDetails.LastRunTime,
            taskDetails.NextRunTime));
    }

    private bool HasSettingsChanged(ScheduledTaskDetails taskDetails, SchedulerAppTask schedulerAppTask, EnvironmentInfo environmentInfo)
    {
      if (taskDetails == null)
      {
        return false;
      }

      string taskExecutablePath = GetTaskExecutablePath(schedulerAppTask, environmentInfo);

      return !(taskDetails.Name == schedulerAppTask.Name
          && taskDetails.ScheduledHour == schedulerAppTask.ScheduledHour
          && taskDetails.ScheduledMinute == schedulerAppTask.ScheduledMinute
          && taskDetails.ExecutionTimeLimitInMinutes == schedulerAppTask.ExecutionTimeLimitInMinutes
          && taskDetails.ExeAbsolutePath == taskExecutablePath);
    }

    #endregion
  }
}
