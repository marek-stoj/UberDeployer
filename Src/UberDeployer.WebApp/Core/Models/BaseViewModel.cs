using System.Security.Principal;

namespace UberDeployer.WebApp.Core.Models
{
  public abstract class BaseViewModel
  {
    public string Username
    {
      get
      {
        WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();

        return windowsIdentity != null ? windowsIdentity.Name : "?";
      }
    }

    public AppPage CurrentAppPage { get; set; }
  }
}
