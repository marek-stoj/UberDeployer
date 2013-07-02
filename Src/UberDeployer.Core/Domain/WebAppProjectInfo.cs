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

    public WebAppProjectInfo(string name, string artifactsRepositoryName, string artifactsRepositoryDirName, bool artifactsAreNotEnvironmentSpecific)
      : base(name, artifactsRepositoryName, artifactsRepositoryDirName, artifactsAreNotEnvironmentSpecific)
    {
    }

    #endregion

    #region Overrides of ProjectInfo

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
          objectFactory.CreateIIisManager());
    }

    public override IEnumerable<string> GetTargetFolders(IObjectFactory objectFactory, EnvironmentInfo environmentInfo)
    {
      Guard.NotNull(objectFactory, "objectFactory");
      Guard.NotNull(environmentInfo, "environmentInfo");

      WebAppProjectConfiguration configuration =
        environmentInfo.GetWebAppProjectConfiguration(Name);

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
        environmentInfo.GetWebAppProjectConfiguration(Name);

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

    #endregion
  }
}
