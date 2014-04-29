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
    public WebAppProjectInfo(string name, string artifactsRepositoryName, IEnumerable<string> allowedEnvironmentNames, string artifactsRepositoryDirName, bool artifactsAreNotEnvironmentSpecific, string appPoolId, string webSiteName, string webAppDirName, string webAppName = null)
      : base(name, artifactsRepositoryName, allowedEnvironmentNames, artifactsRepositoryDirName, artifactsAreNotEnvironmentSpecific)
    {
      Guard.NotNullNorEmpty(appPoolId, "appPoolId");
      Guard.NotNullNorEmpty(webSiteName, "webSiteName");
      Guard.NotNullNorEmpty(webAppDirName, "webAppDirName");

      AppPoolId = appPoolId;
      WebSiteName = webSiteName;
      WebAppDirName = webAppDirName;
      WebAppName = webAppName;
    }

    public override ProjectType Type
    {
      get { return ProjectType.WebApp; }
    }

    public override InputParams CreateEmptyInputParams()
    {
      return new WebAppInputParams();
    }

    public override DeploymentTask CreateDeploymentTask(IObjectFactory objectFactory)
    {
      Guard.NotNull(objectFactory, "objectFactory");      

      return
        new DeployWebAppDeploymentTask(
          objectFactory.CreateProjectInfoRepository(),
          objectFactory.CreateEnvironmentInfoRepository(),
          objectFactory.CreateIMsDeploy(),
          objectFactory.CreateArtifactsRepository(),
          objectFactory.CreateIIisManager(),
          objectFactory.CreateFileAdapter(),
          objectFactory.CreateZipFileAdapter());
    }

    public override IEnumerable<string> GetTargetFolders(IObjectFactory objectFactory, EnvironmentInfo environmentInfo)
    {
      Guard.NotNull(objectFactory, "objectFactory");
      Guard.NotNull(environmentInfo, "environmentInfo");

      WebAppProjectConfiguration configuration =
        environmentInfo.GetWebAppProjectConfiguration(this);

      return
        environmentInfo
          .WebServerMachineNames
          .Select(
            wsmn =>
            environmentInfo.GetWebServerNetworkPath(
              wsmn,
              Path.Combine(environmentInfo.WebAppsBaseDirPath, configuration.WebAppDirName)))
          .ToList();
    }

    public IEnumerable<string> GetTargetUrls(EnvironmentInfo environmentInfo)
    {
      if (environmentInfo == null)
      {
        throw new ArgumentNullException("environmentInfo");
      }

      WebAppProjectConfiguration webAppProjectConfiguration =
        environmentInfo.GetWebAppProjectConfiguration(this);

      // TODO IMM HI: what about https vs http?
      return
        environmentInfo.WebServerMachineNames
          .Select(
            wsmn =>
            string.Format(
              "http://{0}/{1}",
              wsmn,
              webAppProjectConfiguration.WebAppName))
          .ToList();
    }

    public override string GetMainAssemblyFileName()
    {
      // TODO IMM HI: add mainassemblyfilename to config?
      return string.Format("bin\\{0}.dll", Name);
    }

    public string AppPoolId { get; private set; }

    public string WebSiteName { get; private set; }

    public string WebAppDirName { get; private set; }

    public string WebAppName { get; private set; }
  }
}
