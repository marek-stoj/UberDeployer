using System.ComponentModel;

namespace UberDeployer.WinApp.ViewModels.PropertyGrids
{
  public class TerminalAppProjectInfoInPropertyGridViewModel : ProjectInfoInPropertyGridViewModel
  {
    [Category("Specific")]
    [ReadOnly(true)]
    public string TerminalAppName { get; set; }

    [Category("Specific")]
    [ReadOnly(true)]
    public string TerminalAppDirName { get; set; }

    [Category("Specific")]
    [ReadOnly(true)]
    public string TerminalAppExeName { get; set; }
  }
}
