using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UberDeployer.Agent.Proxy.Dto;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Deployment;

namespace UberDeployer.Core.Domain
{
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

    public override DeploymentTask CreateDeploymentTask(IObjectFactory objectFactory)
    {
      Guard.NotNull(objectFactory, "objectFactory");      

      return
        new DeployWebAppDeploymentTask(
          objectFactory.CreateIMsDeploy(),
          objectFactory.CreateEnvironmentInfoRepository(),
          objectFactory.CreateArtifactsRepository(),
          objectFactory.CreateIIisManager());
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

    public string IisSiteName { get; private set; }

    public string WebAppName { get; private set; }

    public string WebAppDirName { get; private set; }

    public IisAppPoolInfo AppPool { get; private set; }
  }
}
