using System.Web;

namespace UberDeployer.WebApp.Core.Utils
{
  public class SecurityUtils
  {
    public static string CurrentUsername
    {
      get
      {
        return
          (HttpContext.Current.User != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity != null && !string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
            ? HttpContext.Current.User.Identity.Name
            : "?";
      }
    }

  }
}
