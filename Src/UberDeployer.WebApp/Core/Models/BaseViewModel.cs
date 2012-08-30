using System.Web;

namespace UberDeployer.WebApp.Core.Models
{
  public abstract class BaseViewModel
  {
    public string Username
    {
      get
      {
        return
          HttpContext.Current.User != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity != null && !string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name)
            ? HttpContext.Current.User.Identity.Name
            : "?";
      }
    }

    public AppPage CurrentAppPage { get; set; }
  }
}
