using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain.Input;

namespace UberDeployer.Core.Domain
{
  public class WebAppProjectInfo : ProjectInfo
  {
    #region Ctor(s)

    public WebAppProjectInfo(string name, string artifactsRepositoryName, string artifactsRepositoryDirName, bool artifactsAreNotEnvironmentSpecific, string webAppName, string webAppDirName)
      : base(name, artifactsRepositoryName, artifactsRepositoryDirName, artifactsAreNotEnvironmentSpecific)
    {
      Guard.NotNullNorEmpty(webAppName, "webAppName");
      Guard.NotNullNorEmpty(webAppDirName, "webAppDirName");

      WebAppName = webAppName;
      WebAppDirName = webAppDirName;
    }

    #endregion

    #region Overrides of ProjectInfo

    public override InputParams CreateEmptyInputParams()
    {
      return new WebAppInputParams();
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

    #endregion

    #region Properties

    public string WebAppName { get; private set; }

    public string WebAppDirName { get; private set; }
    
    #endregion
  }
}
