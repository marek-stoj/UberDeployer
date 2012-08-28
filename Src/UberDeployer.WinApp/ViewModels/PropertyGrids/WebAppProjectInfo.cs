using System.ComponentModel;
using UberDeployer.Agent.Proxy.Dto;

namespace UberDeployer.WinApp.ViewModels.PropertyGrids
{
  public class WebAppProjectInfoInPropertyGridViewModel : ProjectInfoInPropertyGridViewModel
  {
    [Category("Specific")]
    [ReadOnly(true)]
    public string IisSiteName { get; set; }

    [Category("Specific")]
    [ReadOnly(true)]
    public string WebAppName { get; set; }

    [Category("Specific")]
    [ReadOnly(true)]
    public string WebAppDirName { get; set; }

    [Category("Specific")]
    [ReadOnly(true)]
    public IisAppPoolInfo AppPool { get; set; }
  }
}
