namespace UberDeployer.WebApp.Core.Models.Deployment
{
  public class IndexViewModel : BaseViewModel
  {
    public IndexViewModel()
    {
      CurrentAppPage = AppPage.Deployment;
    }

    public string TipOfTheDay { get; set; }

    public string TodayDevLifeGifUrl { get; set; }
    
    public string TodayDevLifeGifDescription { get; set; }

    public bool CanDeploy { get; set; }

    public bool ShowOnlyDeployable { get; set; }

    public bool IsCreatePackageVisible { get; set; }
  }
}
