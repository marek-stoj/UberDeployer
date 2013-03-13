using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UberDeployer.Common.SyntaxSugar;
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

    private readonly IIisManager _iisManager;

    private WebAppProjectInfo _webAppProjectInfo;

    #region Constructor(s)

    public DeployWebAppDeploymentTask(IMsDeploy msDeploy, IEnvironmentInfoRepository environmentInfoRepository, IArtifactsRepository artifactsRepository, IIisManager iisManager)
      : base(environmentInfoRepository)
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

      _msDeploy = msDeploy;
      _artifactsRepository = artifactsRepository;
      _iisManager = iisManager;      
    }

    #endregion Constructor(s)

    #region Overrides of DeploymentTaskBase

    protected override void DoPrepare()
    {
      WebInputParams webAppProjectInfo = (WebInputParams) DeploymentInfo.InputParams;
      
      EnvironmentInfo environmentInfo = GetEnvironmentInfo();

      if (webAppProjectInfo.WebMachines == null || !webAppProjectInfo.WebMachines.Any())
      {
        throw new DeploymentTaskException(string.Format("No web machine for '{0}' has been specified.", DeploymentInfo.TargetEnvironmentName));
      }

      string[] invalidMachineNames = webAppProjectInfo.WebMachines.Except(environmentInfo.WebServerMachineNames).ToArray();

      if (invalidMachineNames.Any())
      {
        throw new DeploymentTaskException(string.Format("Invalid web machines '{0}' have been specified.", string.Join(",", invalidMachineNames)));        
      }

      _webAppProjectInfo = DeploymentInfo.ProjectInfo as WebAppProjectInfo;

      Guard.NotNull(_webAppProjectInfo, "_webAppProjectInfo");

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

      if (DeploymentInfo.ProjectInfo.ArtifactsAreEnvironmentSpecific)
      {
        var binariesConfiguratorStep = new ConfigureBinariesStep(
          environmentInfo.ConfigurationTemplateName, GetTempDirPath());

        AddSubTask(binariesConfiguratorStep);
      }

      foreach (string webServerMachineName in webAppProjectInfo.WebMachines)
      {
        string webApplicationPhysicalPath =
          _iisManager.GetWebApplicationPath(
            webServerMachineName,
            string.Format("{0}/{1}", _webAppProjectInfo.WebSiteId, _webAppProjectInfo.WebAppName));

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
            _webAppProjectInfo.WebSiteId,
            _webAppProjectInfo.WebAppName);

        AddSubTask(createWebDeployPackageDeploymentStep);

        // create a step for deploying the WebDeploy package to the target machine
        var deployWebDeployPackageDeploymentStep =
          new DeployWebDeployPackageDeploymentStep(
            _msDeploy,
            createWebDeployPackageDeploymentStep.PackageFilePath,
            webServerMachineName);

        AddSubTask(deployWebDeployPackageDeploymentStep);

        // check if the app pool exists on the target machine
        if (!_iisManager.AppPoolExists(webServerMachineName, _webAppProjectInfo.AppPoolId.Name))
        {
          // create a step for creating a new app pool
          var createAppPoolDeploymentStep =
            new CreateAppPoolDeploymentStep(
              _iisManager,
              webServerMachineName,
              _webAppProjectInfo.AppPoolId);

          AddSubTask(createAppPoolDeploymentStep);
        }

        // create a step for assigning the app pool to the web application
        var setAppPoolDeploymentStep =
          new SetAppPoolDeploymentStep(
            _iisManager,
            webServerMachineName,
            _webAppProjectInfo.WebSiteId,
            _webAppProjectInfo.WebAppName,
            _webAppProjectInfo.AppPoolId);

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
            _webAppProjectInfo.Name,
            DeploymentInfo.ProjectConfigurationName,
            DeploymentInfo.ProjectConfigurationBuildId,
            DeploymentInfo.TargetEnvironmentName);
      }
    }

    #endregion Overrides of DeploymentTaskBase
  }
}