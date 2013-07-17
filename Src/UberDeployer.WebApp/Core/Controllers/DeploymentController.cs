using System;
using System.Threading;
using System.Web.Mvc;
using UberDeployer.Common;
using UberDeployer.WebApp.Core.Models.Deployment;
using UberDeployer.WebApp.Core.Services;

namespace UberDeployer.WebApp.Core.Controllers
{
  public class DeploymentController : UberDeployerWebAppController
  {
    private const string _AppSettingKey_CanDeployRole = "CanDeployRole";

    private readonly string _canDeployRole;

    public DeploymentController(string canDeployRole = null)
    {
      _canDeployRole = canDeployRole;
    }

    public DeploymentController()
      : this(AppSettingsUtils.ReadAppSettingStringOptional(_AppSettingKey_CanDeployRole))
    {
    }

    [HttpGet]
    public ActionResult Index()
    {
      Tuple<string, string> todayDevLifeGif = DevLife.GetTodayGif();

      bool canDeploy =
        string.IsNullOrEmpty(_canDeployRole) || Thread.CurrentPrincipal.IsInRole(_canDeployRole);

      var viewModel =
        new IndexViewModel
          {
            TipOfTheDay = LifeProFuckingTips.GetTodayTip(),
            TodayDevLifeGifUrl = todayDevLifeGif.Item1,
            TodayDevLifeGifDescription = todayDevLifeGif.Item2,
            CanDeploy = canDeploy,
          };

      return View(viewModel);
    }
  }
}
