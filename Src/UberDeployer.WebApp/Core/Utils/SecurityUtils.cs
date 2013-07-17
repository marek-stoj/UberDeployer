using System.Threading;
using System.Web;
using UberDeployer.Common;

namespace UberDeployer.WebApp.Core.Utils
{
  public class SecurityUtils
  {
    private const string _AppSettingKey_CanDeployRole = "CanDeployRole";

    private static readonly string _canDeployRole;

    static SecurityUtils()
    {
      _canDeployRole = AppSettingsUtils.ReadAppSettingStringOptional(_AppSettingKey_CanDeployRole);
    }

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

    public static bool CanDeploy
    {
      get { return string.IsNullOrEmpty(_canDeployRole) || Thread.CurrentPrincipal.IsInRole(_canDeployRole); }
    }
  }
}
