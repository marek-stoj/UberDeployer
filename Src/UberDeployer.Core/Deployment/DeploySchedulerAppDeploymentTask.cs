using System;
using System.IO;
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

    private SchedulerAppProjectInfo _projectInfo;

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

      _projectInfo = (SchedulerAppProjectInfo) DeploymentInfo.ProjectInfo;

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
        PasswordCollectorHelper.CollectPasssword(
          _passwordCollector,
          environmentInfo,
          environmentInfo.AppServerMachineName,
          _projectInfo.SchedulerAppUserId,
          out environmentUser);

      if (!taskIsScheduled)
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
      else
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
  }
}