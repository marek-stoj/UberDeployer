using System;
using System.Web.Mvc;
using UberDeployer.WebApp.Core.Models.Deployment;
using UberDeployer.WebApp.Core.Services;
using UberDeployer.WebApp.Core.Utils;

namespace UberDeployer.WebApp.Core.Controllers
{
  public class DeploymentController : UberDeployerWebAppController
  {
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
          };

      return View(viewModel);
    }
  }
}
