using UberDeployer.WebApp.Core.Utils;

namespace UberDeployer.WebApp.Core.Models
{
  public abstract class BaseViewModel
  {
    public string Username
    {
      get { return SecurityUtils.CurrentUsername; }
    }

    public AppPage CurrentAppPage { get; set; }
  }
}
