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
    private readonly SchedulerAppProjectInfo _projectInfo;

    private readonly string _projectConfigurationName;
    private readonly string _projectConfigurationBuildId;

    #region Constructor(s)

    public DeploySchedulerAppDeploymentTask(
      IEnvironmentInfoRepository environmentInfoRepository,
      IArtifactsRepository artifactsRepository,
      ITaskScheduler taskScheduler,
      IPasswordCollector passwordCollector,
      SchedulerAppProjectInfo projectInfo,
      string projectConfigurationName,
      string projectConfigurationBuildId,
      string targetEnvironmentName)
      : base(environmentInfoRepository, targetEnvironmentName)
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

      _artifactsRepository = artifactsRepository;
      _taskScheduler = taskScheduler;
      _passwordCollector = passwordCollector;
      _projectInfo = projectInfo;
      _projectConfigurationName = projectConfigurationName;
      _projectConfigurationBuildId = projectConfigurationBuildId;
    }

    #endregion

    #region Overrides of DeploymentTaskBase

    protected override void DoPrepare()
    {
      EnvironmentInfo environmentInfo = GetEnvironmentInfo();

      CreateTemporaryDirectory();

      // create a step for downloading the artifacts
      var downloadArtifactsDeploymentStep =
        new DownloadArtifactsDeploymentStep(
          _artifactsRepository,
          _projectInfo,
          _projectConfigurationName,
          _projectConfigurationBuildId,
          TempDirPath);

      AddSubTask(downloadArtifactsDeploymentStep);

      // create a step for extracting the artifacts
      var extractArtifactsDeploymentStep =
        new ExtractArtifactsDeploymentStep(
          environmentInfo,
          _projectInfo,
          _projectConfigurationName,
          _projectConfigurationBuildId,
          downloadArtifactsDeploymentStep.ArtifactsFilePath,
          TempDirPath);

      AddSubTask(extractArtifactsDeploymentStep);

      // create a step for copying the binaries to the target machine
      string targetDirPath = Path.Combine(environmentInfo.SchedulerAppsBaseDirPath, _projectInfo.SchedulerAppDirName);

      // create a backup step if needed
      string targetDirNetworkPath = environmentInfo.GetAppServerNetworkPath(targetDirPath);

      if (Directory.Exists(targetDirNetworkPath))
      {
        AddSubTask(new BackupFilesDeploymentStep(targetDirNetworkPath));
      }

      AddSubTask(
        new CopyFilesDeploymentStep(
          extractArtifactsDeploymentStep.BinariesDirPath,
          environmentInfo.GetAppServerNetworkPath(targetDirPath)));

      // determine if the task should be scheduled anew or if its schedule should be updated
      string machineName = environmentInfo.AppServerMachineName;
      string taskName = _projectInfo.SchedulerAppName;
      string executablePath = Path.Combine(targetDirPath, _projectInfo.SchedulerAppExeName);
      bool taskIsScheduled = _taskScheduler.IsTaskScheduled(machineName, taskName);

      // collect password
      EnvironmentUser environmentUser;

      string environmentUserPassword =
        PasswordCollectorHelper
          .CollectPasssword(
            _passwordCollector,
            environmentInfo,
            _projectInfo.SchedulerAppUserId,
            out environmentUser);

      if (!taskIsScheduled)
      {
        // create a step for scheduling a new app
        AddSubTask(
          new ScheduleNewAppDeploymentStep(
            _taskScheduler,
            machineName,
            _projectInfo,
            executablePath,
            environmentUser.UserName,
            environmentUserPassword));
      }
      else
      {
        // create a step for updating an existing scheduler app
        AddSubTask(
          new UpdateAppScheduleDeploymentStep(
            _taskScheduler,
            machineName,
            _projectInfo,
            executablePath,
            environmentUser.UserName,
            environmentUserPassword));
      }
    }

    protected override void DoExecute()
    {
      try
      {
        base.DoExecute();
      }
      finally
      {
        DeleteTemporaryDirectory();
      }
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Deploy scheduler app '{0} ({1}:{2})' to '{3}'.",
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

    #endregion
  }
}
