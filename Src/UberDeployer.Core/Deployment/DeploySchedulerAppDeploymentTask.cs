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
    private Dictionary<Tuple<string, string>, ScheduledTaskDetails> _existingTaskDetailsByMachineNameAndTaskName;

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
      _existingTaskDetailsByMachineNameAndTaskName = new Dictionary<Tuple<string, string>, ScheduledTaskDetails>();

      foreach (string tmpSchedulerServerTasksMachineName in environmentInfo.SchedulerServerTasksMachineNames)
      {
        string schedulerServerTasksMachineName = tmpSchedulerServerTasksMachineName;

        _projectInfo.SchedulerAppTasks
          .ForEach(
            schedulerAppTask =>
            {
              ScheduledTaskDetails taskDetails =
                _taskScheduler.GetScheduledTaskDetails(schedulerServerTasksMachineName, schedulerAppTask.Name);

              _existingTaskDetailsByMachineNameAndTaskName.Add(
                Tuple.Create(schedulerServerTasksMachineName, schedulerAppTask.Name),
                taskDetails);

              EnsureTaskIsNotRunning(taskDetails, schedulerServerTasksMachineName);

              // create a step to disable scheduler task
              if (taskDetails != null && taskDetails.IsEnabled)
              {
                AddToggleSchedulerAppTaskEnabledStep(schedulerServerTasksMachineName, taskDetails.Name, false);
              }
            });
      }

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

      // create steps for copying the binaries to target binaries machines
      foreach (string schedulerServerBinariesMachineName in environmentInfo.SchedulerServerBinariesMachineNames)
      {
        string targetDirPath = GetTargetDirPath(environmentInfo);
        string targetDirNetworkPath = environmentInfo.GetSchedulerServerNetworkPath(schedulerServerBinariesMachineName, targetDirPath);

        AddSubTask(
          new CopyFilesDeploymentStep(
            _directoryAdapter,
            _fileAdapter,
            binariesDirPathProvider,
            new Lazy<string>(() => targetDirNetworkPath)));
      }

      foreach (string tmpSchedulerServerTasksMachineName in environmentInfo.SchedulerServerTasksMachineNames)
      {
        string schedulerServerTasksMachineName = tmpSchedulerServerTasksMachineName;

        _projectInfo.SchedulerAppTasks
          .ForEach(
            schedulerAppTask =>
            {
              string taskName = schedulerAppTask.Name;
              
              Tuple<string, string> machineNameAndTaskName =
                Tuple.Create(schedulerServerTasksMachineName, taskName);

              ScheduledTaskDetails existingTaskDetails =
                _existingTaskDetailsByMachineNameAndTaskName[machineNameAndTaskName];

              AddTaskConfigurationSteps(
                environmentInfo,
                schedulerServerTasksMachineName,
                schedulerAppTask,
                existingTaskDetails);

              // create a step to toggle scheduler task enabled
              if (existingTaskDetails == null || existingTaskDetails.IsEnabled)
              {
                AddToggleSchedulerAppTaskEnabledStep(
                  schedulerServerTasksMachineName,
                  taskName,
                  true);
              }
            });
      }
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

    private void AddTaskConfigurationSteps(EnvironmentInfo environmentInfo, string schedulerServerTasksMachineName, SchedulerAppTask schedulerAppTask, ScheduledTaskDetails taskDetails = null)
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
              schedulerServerTasksMachineName,
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
            schedulerServerTasksMachineName,
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
            schedulerServerTasksMachineName,
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
      foreach (Tuple<string, string> machineNameAndTaskName in _existingTaskDetailsByMachineNameAndTaskName.Keys)
      {
        string schedulerServerTasksMachineName = machineNameAndTaskName.Item1;
        string taskName = machineNameAndTaskName.Item2;

        try
        {
          ScheduledTaskDetails existingTaskDetails =
            _existingTaskDetailsByMachineNameAndTaskName[machineNameAndTaskName];

          if (existingTaskDetails == null || existingTaskDetails.IsEnabled)
          {
            ScheduledTaskDetails currentTaskDetails =
              _taskScheduler.GetScheduledTaskDetails(schedulerServerTasksMachineName, taskName);

            if (currentTaskDetails != null && !currentTaskDetails.IsEnabled)
            {
              _taskScheduler.ToggleTaskEnabled(schedulerServerTasksMachineName, taskName, true);
            }
          }
        }
        catch (Exception exc)
        {
          PostDiagnosticMessage(string.Format("Error while making sure that tasks that were enabled are enabled on machine '{0}'. Exception: {1}", schedulerServerTasksMachineName, exc), DiagnosticMessageType.Error);
        }
      }
    }

    private static void EnsureTaskIsNotRunning(ScheduledTaskDetails taskDetails, string schedulerServerTasksMachineName)
    {
      if (taskDetails == null || !taskDetails.IsRunning)
      {
        return;
      }

      throw
        new DeploymentTaskException(
          string.Format(
            "Task: {0} on machine: {1} is already running. Deployment aborted. Last run time: {2}, next run time: {3}",
            schedulerServerTasksMachineName,
            taskDetails.Name,
            taskDetails.LastRunTime,
            taskDetails.NextRunTime));
    }

    #endregion
  }
}
