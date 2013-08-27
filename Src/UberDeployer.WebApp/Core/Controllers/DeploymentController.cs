using System;
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

    private static readonly bool _onlyDeployableCheckedByDefault;

    static DeploymentController()
    {
      _onlyDeployableCheckedByDefault =
        AppSettingsUtils.ReadAppSettingBool(_AppSettingsKey_OnlyDeployableCheckedByDefault);
    }

    [HttpGet]
    public ActionResult Index()
    {
      Tuple<string, string> todayDevLifeGif = DevLife.GetTodayGif();

      var viewModel =
        new IndexViewModel
          {
            TipOfTheDay = LifeProFuckingTips.GetTodayTip(),
            TodayDevLifeGifUrl = todayDevLifeGif.Item1,
            TodayDevLifeGifDescription = todayDevLifeGif.Item2,
            CanDeploy = SecurityUtils.CanDeploy,
            ShowOnlyDeployable = _onlyDeployableCheckedByDefault,
          };

      return View(viewModel);
    }
  }
}
