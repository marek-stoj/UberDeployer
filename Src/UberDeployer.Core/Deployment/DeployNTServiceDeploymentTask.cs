using System;
using System.IO;
using System.ServiceProcess;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.NtServices;

namespace UberDeployer.Core.Deployment
{
  public class DeployNtServiceDeploymentTask : DeploymentTask
  {
    private readonly IArtifactsRepository _artifactsRepository;
    private readonly NtServiceProjectInfo _projectInfo;

    private readonly string _projectConfigurationName;
    private readonly string _projectConfigurationBuildId;
    private readonly INtServiceManager _ntServiceManager;
    private readonly IPasswordCollector _passwordCollector;

    #region Constructor(s)

    public DeployNtServiceDeploymentTask(
      IEnvironmentInfoRepository environmentInfoRepository,
      IArtifactsRepository artifactsRepository,
      INtServiceManager ntServiceManager,
      IPasswordCollector passwordCollector,
      NtServiceProjectInfo projectInfo,
      string projectConfigurationName,
      string projectConfigurationBuildId,
      string targetEnvironmentName)
      : base(environmentInfoRepository, targetEnvironmentName)
    {
      if (artifactsRepository == null)
      {
        throw new ArgumentNullException("artifactsRepository");
      }

      if (ntServiceManager == null)
      {
        throw new ArgumentNullException("ntServiceManager");
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

      if (passwordCollector == null)
      {
        throw new ArgumentNullException("passwordCollector");
      }

      _artifactsRepository = artifactsRepository;
      _projectInfo = projectInfo;
      _projectConfigurationName = projectConfigurationName;
      _projectConfigurationBuildId = projectConfigurationBuildId;
      _ntServiceManager = ntServiceManager;
      _passwordCollector = passwordCollector;
    }

    #endregion

    #region Overrides of DeploymentTaskBase

    protected override void DoPrepare()
    {
      EnvironmentInfo environmentInfo = GetEnvironmentInfo();

      // create a step for downloading the artifacts
      var downloadArtifactsDeploymentStep =
        new DownloadArtifactsDeploymentStep(
          _artifactsRepository,
          _projectInfo,
          _projectConfigurationName,
          _projectConfigurationBuildId,
          GetTempDirPath());

      AddSubTask(downloadArtifactsDeploymentStep);

      // create a step for extracting the artifacts
      var extractArtifactsDeploymentStep =
        new ExtractArtifactsDeploymentStep(
          environmentInfo,
          _projectInfo,
          _projectConfigurationName,
          _projectConfigurationBuildId,
          downloadArtifactsDeploymentStep.ArtifactsFilePath,
          GetTempDirPath());

      AddSubTask(extractArtifactsDeploymentStep);

      // check if the service is present on the target machine
      bool serviceExists =
        _ntServiceManager
          .DoesServiceExist(environmentInfo.AppServerMachineName, _projectInfo.NtServiceName);

      if (serviceExists)
      {
        // create a step for stopping the service
        AddSubTask(
          new StopNtServiceDeploymentStep(
            _ntServiceManager,
            environmentInfo.AppServerMachineName,
            _projectInfo.NtServiceName));
      }

      // create a step for copying the binaries to the target machine
      string targetDirPath = Path.Combine(environmentInfo.NtServicesBaseDirPath, _projectInfo.NtServiceDirName);

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

      if (!serviceExists)
      {
        // collect password
        EnvironmentUser environmentUser;

        string environmentUserPassword =
          PasswordCollectorHelper
            .CollectPasssword(
              _passwordCollector,
              environmentInfo,
              _projectInfo.NtServiceUserId,
              out environmentUser);

        // create a step for installing the service,
        string serviceExecutablePath = Path.Combine(targetDirPath, _projectInfo.NtServiceExeName);

        var ntServiceDescriptor =
          new NtServiceDescriptor(
            _projectInfo.NtServiceName,
            serviceExecutablePath,
            ServiceAccount.NetworkService,
            ServiceStartMode.Automatic,
            _projectInfo.NtServiceDisplayName,
            environmentUser.UserName,
            environmentUserPassword);

        AddSubTask(
          new InstallNtServiceDeploymentStep(
            _ntServiceManager,
            environmentInfo.AppServerMachineName,
            ntServiceDescriptor));
      }

      // create a step for starting the service
      AddSubTask(
        new StartNtServiceDeploymentStep(
          _ntServiceManager,
          environmentInfo.AppServerMachineName,
          _projectInfo.NtServiceName));
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Deploy NT service '{0} ({1}:{2})' to '{3}'.",
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
