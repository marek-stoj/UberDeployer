using System;
using System.Collections.Generic;
using System.IO;
using UberDeployer.Common;
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
    // ReSharper disable NotAccessedField.Local
    private readonly IDirectoryAdapter _directoryAdapter;
    // ReSharper restore NotAccessedField.Local
    private readonly IFileAdapter _fileAdapter;
    private readonly IZipFileAdapter _zipFileAdapter;
    
    private SchedulerAppProjectInfo _projectInfo;
    private Dictionary<string, string> _collectedPasswordsByUserName;
    private Dictionary<string, ScheduledTaskDetails> _existingTaskDetailsByName;

    #region Constructor(s)

    public DeploySchedulerAppDeploymentTask(
      IProjectInfoRepository projectInfoRepository,
      IEnvironmentInfoRepository environmentInfoRepository,
      IArtifactsRepository artifactsRepository,
      ITaskScheduler taskScheduler,
      IPasswordCollector passwordCollector,
      IDirectoryAdapter directoryAdapter,
      IFileAdapter fileAdapter,
      IZipFileAdapter zipFileAdapter)
      : base(projectInfoRepository, environmentInfoRepository)
    {
      Guard.NotNull(artifactsRepository, "artifactsRepository");
      Guard.NotNull(taskScheduler, "taskScheduler");
      Guard.NotNull(passwordCollector, "passwordCollector");
      Guard.NotNull(directoryAdapter, "directoryAdapter");
      Guard.NotNull(fileAdapter, "fileAdapter");
      Guard.NotNull(zipFileAdapter, "zipFileAdapter");

      _artifactsRepository = artifactsRepository;
      _taskScheduler = taskScheduler;
      _passwordCollector = passwordCollector;
      _directoryAdapter = directoryAdapter;
      _fileAdapter = fileAdapter;
      _zipFileAdapter = zipFileAdapter;
    }

    #endregion

    #region Overrides of DeploymentTaskBase

    protected override void DoPrepare()
    {
      EnvironmentInfo environmentInfo = GetEnvironmentInfo();

      _projectInfo = GetProjectInfo<SchedulerAppProjectInfo>();
      _collectedPasswordsByUserName = new Dictionary<string, string>();
      _existingTaskDetailsByName = new Dictionary<string, ScheduledTaskDetails>();

      string machineName = GetSchedulerServerMachineName(environmentInfo);
      string targetDirPath = GetTargetDirPath(environmentInfo);
      string targetDirNetworkPath = environmentInfo.GetSchedulerServerNetworkPath(targetDirPath);

      _projectInfo.SchedulerAppTasks
        .ForEach(
          schedulerAppTask =>
          {
            ScheduledTaskDetails taskDetails =
              _taskScheduler.GetScheduledTaskDetails(machineName, schedulerAppTask.Name);

            _existingTaskDetailsByName.Add(schedulerAppTask.Name, taskDetails);

            EnsureTaskIsNotRunning(taskDetails, environmentInfo);

            // create a step to disable scheduler task
            if (taskDetails != null && taskDetails.IsEnabled)
            {
              AddToggleSchedulerAppTaskEnabledStep(machineName, taskDetails.Name, false);
            }
          });

      Lazy<string> binariesDirPathProvider =
        AddStepsToObtainBinaries(environmentInfo);

/* // TODO IMM HI: xxx we don't need this for now - should we parameterize this somehow?
      if (_directoryAdapter.Exists(targetDirNetworkPath))
      {
        AddSubTask(
          new BackupFilesDeploymentStep(
            targetDirNetworkPath));
      }
*/

      // create a step for copying the binaries to the target machine
      AddSubTask(
        new CopyFilesDeploymentStep(
          _directoryAdapter,
          _fileAdapter,
          binariesDirPathProvider,
          new Lazy<string>(() => targetDirNetworkPath)));

      _projectInfo.SchedulerAppTasks
        .ForEach(
          schedulerAppTask =>
          {
            ScheduledTaskDetails existingTaskDetails =
              _existingTaskDetailsByName[schedulerAppTask.Name];

            AddTaskConfigurationSteps(
              environmentInfo,
              schedulerAppTask,
              existingTaskDetails);

            // create a step to toggle scheduler task enabled
            if (existingTaskDetails == null || existingTaskDetails.IsEnabled)
            {
              AddToggleSchedulerAppTaskEnabledStep(
                machineName,
                schedulerAppTask.Name,
                true);
            }
          });
    }

    protected override void DoExecute()
    {
      try
      {
        base.DoExecute();
      }
      catch
      {
        MakeSureTasksThatWereEnabledAreEnabled();

        throw;
      }
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

      EnvironmentUser environmentUser =
        environmentInfo.GetEnvironmentUserById(schedulerAppTask.UserId);

      string environmentUserPassword = null;

      if (!taskExists || hasSettingsChanged)
      {
        // collect password if not already collected
        if (!_collectedPasswordsByUserName.TryGetValue(environmentUser.UserName, out environmentUserPassword))
        {
          environmentUserPassword =
            PasswordCollectorHelper.CollectPasssword(
              _passwordCollector,
              DeploymentInfo.DeploymentId,
              environmentInfo,
              environmentInfo.SchedulerServerMachineName,
              environmentUser,
              OnDiagnosticMessagePosted);

          _collectedPasswordsByUserName.Add(environmentUser.UserName, environmentUserPassword);
        }
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
            schedulerAppTask.Repetition,
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
            schedulerAppTask.Repetition,
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
          GetTempDirPath(),
          _fileAdapter,
          _zipFileAdapter);

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
          && taskDetails.Repetition.Interval == schedulerAppTask.Repetition.Interval
          && taskDetails.Repetition.Duration == schedulerAppTask.Repetition.Duration
          && taskDetails.Repetition.StopAtDurationEnd == schedulerAppTask.Repetition.StopAtDurationEnd
          && taskDetails.ExeAbsolutePath == taskExecutablePath);
    }

    private void MakeSureTasksThatWereEnabledAreEnabled()
    {
      EnvironmentInfo environmentInfo = GetEnvironmentInfo();
      string machineName = GetSchedulerServerMachineName(environmentInfo);

      foreach (string taskName in _existingTaskDetailsByName.Keys)
      {
        ScheduledTaskDetails existingTaskDetails =
          _existingTaskDetailsByName[taskName];

        if (existingTaskDetails == null || existingTaskDetails.IsEnabled)
        {
          ScheduledTaskDetails currentTaskDetails =
            _taskScheduler.GetScheduledTaskDetails(machineName, taskName);

          if (currentTaskDetails != null && !currentTaskDetails.IsEnabled)
          {
            _taskScheduler.ToggleTaskEnabled(machineName, taskName, true);
          }
        }
      }
    }

    private static string GetSchedulerServerMachineName(EnvironmentInfo environmentInfo)
    {
      return environmentInfo.SchedulerServerMachineName;
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

    #endregion
  }
}
