using System;
using System.ComponentModel;
using System.IO;
using UberDeployer.Core.Deployment;

namespace UberDeployer.Core.Domain
{
  // TODO IMM HI: create WebServiceProjectInfo
  public class WebAppProjectInfo : ProjectInfo
  {
    public WebAppProjectInfo(string name, string artifactsRepositoryName, string artifactsRepositoryDirName, string iisSiteName, string webAppName, string webAppDirName, IisAppPoolInfo appPoolInfo)
      : base(name, artifactsRepositoryName, artifactsRepositoryDirName)
    {
      if (string.IsNullOrEmpty(iisSiteName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "iisSiteName");
      }

      if (appPoolInfo == null)
      {
        throw new ArgumentNullException("appPoolInfo");
      }
      
      if (string.IsNullOrEmpty(webAppName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "webAppName");
      }

      if (string.IsNullOrEmpty(webAppDirName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "webAppDirName");
      }

      IisSiteName = iisSiteName;
      WebAppName = webAppName;
      WebAppDirName = webAppDirName;
      AppPool = appPoolInfo;
    }

    public override DeploymentTask CreateDeploymentTask(IObjectFactory objectFactory, string projectConfigurationName, string projectConfigurationBuildId, string targetEnvironmentName)
    {
      if (objectFactory == null)
      {
        throw new ArgumentNullException("objectFactory");
      }

      return
        new DeployWebAppDeploymentTask(
          objectFactory.CreateIMsDeploy(),
          objectFactory.CreateEnvironmentInfoRepository(),
          objectFactory.CreateArtifactsRepository(),
          objectFactory.CreateIIisManager(),
          this,
          projectConfigurationName,
          projectConfigurationBuildId,
          targetEnvironmentName);
    }

    public override string GetTargetFolder(EnvironmentInfo environmentInfo)
    {
      if (environmentInfo == null)
      {
        throw new ArgumentNullException("environmentInfo");
      }

      // TODO IMM HI: this might be wrong (due to msdeploy)!
      return
        environmentInfo.GetWebServerNetworkPath(
          Path.Combine(environmentInfo.WebAppsBaseDirPath, WebAppDirName));
    }

    [Category("Specific")]
    public string GetTargetUrl(EnvironmentInfo environmentInfo)
    {
      if (environmentInfo == null)
      {
        throw new ArgumentNullException("environmentInfo");
      }

      return
        string.Format(
          "http://{0}/{1}",
          environmentInfo.WebServerMachineName,
          WebAppName);
    }

    [Category("Specific")]
    public string IisSiteName { get; private set; }

    [Category("Specific")]
    public string WebAppName { get; private set; }

    [Category("Specific")]
    public string WebAppDirName { get; private set; }

    [Category("Specific")]
    public IisAppPoolInfo AppPool { get; private set; }
  }
}
