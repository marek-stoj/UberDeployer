using System;
using System.IO;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Iis;
using UberDeployer.Core.Management.MsDeploy;

namespace UberDeployer.Core.Deployment
{
  // TODO IMM HI: move common code up
  public class DeployWebAppDeploymentTask : DeploymentTask
  {
    private readonly IMsDeploy _msDeploy;
    protected readonly IArtifactsRepository _artifactsRepository;
    protected readonly WebAppProjectInfo _projectInfo;
    protected readonly string _projectConfigurationName;
    protected readonly string _projectConfigurationBuildId;

    private readonly IIisManager _iisManager;

    #region Constructor(s)

    public DeployWebAppDeploymentTask(
      IMsDeploy msDeploy,
      IEnvironmentInfoRepository environmentInfoRepository,
      IArtifactsRepository artifactsRepository,
      IIisManager iisManager,
      WebAppProjectInfo projectInfo,
      string projectConfigurationName,
      string projectConfigurationBuildId,
      string targetEnvironmentName)
      : base(environmentInfoRepository, targetEnvironmentName)
    {
      if (msDeploy == null)
      {
        throw new ArgumentNullException("msDeploy");
      }

      if (artifactsRepository == null)
      {
        throw new ArgumentNullException("artifactsRepository");
      }

      if (iisManager == null)
      {
        throw new ArgumentNullException("iisManager");
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

      _msDeploy = msDeploy;
      _artifactsRepository = artifactsRepository;
      _iisManager = iisManager;
      _projectInfo = projectInfo;
      _projectConfigurationName = projectConfigurationName;
      _projectConfigurationBuildId = projectConfigurationBuildId;
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

      foreach (string webServerMachineName in environmentInfo.WebServerMachineNames)
      {
        string webApplicationPhysicalPath =
          _iisManager.GetWebApplicationPath(
            webServerMachineName,
            string.Format("{0}/{1}", _projectInfo.IisSiteName, _projectInfo.WebAppName));

        if (!string.IsNullOrEmpty(webApplicationPhysicalPath))
        {
          string webApplicationNetworkPath =
            string.Format(
              "\\\\{0}\\{1}${2}",
              webServerMachineName,
              webApplicationPhysicalPath[0],
              webApplicationPhysicalPath.Substring(2));

          if (Directory.Exists(webApplicationNetworkPath))
          {
            var backupFilesDeploymentStep = new BackupFilesDeploymentStep(webApplicationNetworkPath);

            AddSubTask(backupFilesDeploymentStep);
          }
        }

        // create a step for creating a WebDeploy package
        // TODO IMM HI: add possibility to specify physical path on the target machine
        var createWebDeployPackageDeploymentStep =
          new CreateWebDeployPackageDeploymentStep(
            _msDeploy,
            extractArtifactsDeploymentStep.BinariesDirPath,
            _projectInfo.IisSiteName,
            _projectInfo.WebAppName);

        AddSubTask(createWebDeployPackageDeploymentStep);

        // create a step for deploying the WebDeploy package to the target machine
        var deployWebDeployPackageDeploymentStep =
          new DeployWebDeployPackageDeploymentStep(
            _msDeploy,
            createWebDeployPackageDeploymentStep.PackageFilePath,
            webServerMachineName);

        AddSubTask(deployWebDeployPackageDeploymentStep);

        // check if the app pool exists on the target machine
        if (!_iisManager.AppPoolExists(webServerMachineName, _projectInfo.AppPool.Name))
        {
          // create a step for creating a new app pool
          var createAppPoolDeploymentStep =
            new CreateAppPoolDeploymentStep(
              _iisManager,
              webServerMachineName,
              _projectInfo.AppPool);

          AddSubTask(createAppPoolDeploymentStep);
        }

        // create a step for assigning the app pool to the web application
        var setAppPoolDeploymentStep =
          new SetAppPoolDeploymentStep(
            _iisManager,
            webServerMachineName,
            _projectInfo.IisSiteName,
            _projectInfo.WebAppName,
            _projectInfo.AppPool);

        AddSubTask(setAppPoolDeploymentStep);
      }
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Deploy web app '{0} ({1}:{2})' to '{3}'.",
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
