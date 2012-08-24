using UberDeployer.Agent.Proxy.Dto;

namespace UberDeployer.WinApp.ViewModels
{
  public class EnvironmentInfoInComboBoxViewModel
  {
    public EnvironmentInfo EnvironmentInfo { get; set; }

    // TODO IMM HI: implement
    public string DisplayText
    {
      get { return EnvironmentInfo != null ? EnvironmentInfo.Name : "?"; }
    }
  }
}
