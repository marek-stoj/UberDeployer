namespace UberDeployer.WebApp.Core.Models.Deployment
{
  public class IndexViewModel : BaseViewModel
  {
    public IndexViewModel()
    {
      CurrentAppPage = AppPage.Deployment;
    }

    public string TipOfTheDay { get; set; }

    public string FunnyGifUrl { get; set; }
    
    public string FunnyGifDescription { get; set; }

    public bool CanDeploy { get; set; }

    public bool ShowOnlyDeployable { get; set; }

    public bool IsCreatePackageVisible { get; set; }

    /// <summary>
    /// Can be null.
    /// </summary>
    public InitialSelection InitialSelection { get; set; }
  }
}
