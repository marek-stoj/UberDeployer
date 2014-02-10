using System.Web.Mvc;
using UberDeployer.Common;
using UberDeployer.WebApp.Core.Models.Deployment;
using UberDeployer.WebApp.Core.Services;
using UberDeployer.WebApp.Core.Utils;

namespace UberDeployer.WebApp.Core.Controllers
{
  public class DeploymentController : UberDeployerWebAppController
  {
    private const string _AppSettingsKey_OnlyDeployableCheckedByDefault = "OnlyDeployableCheckedByDefault";

    private const string _AppSettingsKey_IsCreatePackageVisible = "IsCreatePackageVisible";

    private static readonly bool _onlyDeployableCheckedByDefault;

    private static readonly bool _isCreatePackageVisible;

    static DeploymentController()
    {
      _onlyDeployableCheckedByDefault =
        AppSettingsUtils.ReadAppSettingBool(_AppSettingsKey_OnlyDeployableCheckedByDefault);

      _isCreatePackageVisible =
        AppSettingsUtils.ReadAppSettingBool(_AppSettingsKey_IsCreatePackageVisible);
    }

    [HttpGet]
    public ActionResult Index(string env = null, string prj = null, string prjCfg = null, string prjCfgBuild = null)
    {
      if (!string.IsNullOrEmpty(env) || !string.IsNullOrEmpty(prj) || !string.IsNullOrEmpty(prjCfg) || !string.IsNullOrEmpty(prjCfgBuild))
      {
        if (string.IsNullOrEmpty(env) || string.IsNullOrEmpty(prj) || string.IsNullOrEmpty(prjCfg))
        {
          return BadRequest();
        }
      }

      FunnyGif funnyGif = FunnyGifs.GetRandomGif();

      var viewModel =
        new IndexViewModel
        {
          TipOfTheDay = LifeProFuckingTips.GetTodayTip(),
          FunnyGifUrl = funnyGif.Url,
          FunnyGifDescription = funnyGif.Description,
          CanDeploy = SecurityUtils.CanDeploy,
          ShowOnlyDeployable = _onlyDeployableCheckedByDefault,
          IsCreatePackageVisible = _isCreatePackageVisible,
          InitialSelection =
            !string.IsNullOrEmpty(env)
              ? new InitialSelection
              {
                TargetEnvironmentName = env,
                ProjectName = prj,
                ProjectConfigurationName = prjCfg,
                ProjectConfigurationBuildId = prjCfgBuild,
              }
              : null,
        };

      return View(viewModel);
    }
  }
}
