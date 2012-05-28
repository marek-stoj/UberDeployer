using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using UberDeployer.Core.Deployment;

namespace UberDeployer.Core.Domain
{
  // TODO IMM HI: create WebServiceProjectInfo
  public class WebAppProjectInfo : ProjectInfo
  {
    public WebAppProjectInfo(string name, string artifactsRepositoryName, string artifactsRepositoryDirName, bool artifactsAreNotEnvironmentSpecific, string iisSiteName, string webAppName, string webAppDirName, IisAppPoolInfo appPoolInfo)
      : base(name, artifactsRepositoryName, artifactsRepositoryDirName, artifactsAreNotEnvironmentSpecific)
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

    public override IEnumerable<string> GetTargetFolders(EnvironmentInfo environmentInfo)
    {
      if (environmentInfo == null)
      {
        throw new ArgumentNullException("environmentInfo");
      }

      // TODO IMM HI: this might be wrong (due to msdeploy)!
      return
        environmentInfo
          .WebServerMachineNames
          .Select(
            wsmn =>
            environmentInfo.GetWebServerNetworkPath(
              wsmn,
              Path.Combine(environmentInfo.WebAppsBaseDirPath, WebAppDirName)))
          .ToList();
    }

    public IEnumerable<string> GetTargetUrls(EnvironmentInfo environmentInfo)
    {
      if (environmentInfo == null)
      {
        throw new ArgumentNullException("environmentInfo");
      }

      // TODO IMM HI: what about https vs http?
      return
        environmentInfo.WebServerMachineNames
          .Select(wsmn => string.Format("http://{0}/{1}", wsmn, WebAppName))
          .ToList();
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
