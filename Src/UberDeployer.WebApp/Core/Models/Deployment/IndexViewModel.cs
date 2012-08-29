namespace UberDeployer.WebApp.Core.Models.Deployment
{
  public class IndexViewModel : BaseViewModel
  {
    public IndexViewModel()
    {
      CurrentAppPage = AppPage.Deployment;
    }

    public string TipOfTheDay { get; set; }
  }
}
