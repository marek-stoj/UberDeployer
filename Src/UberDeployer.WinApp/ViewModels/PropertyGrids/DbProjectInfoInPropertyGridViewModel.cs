using System.ComponentModel;

namespace UberDeployer.WinApp.ViewModels.PropertyGrids
{
  public class DbProjectInfoInPropertyGridViewModel : ProjectInfoInPropertyGridViewModel
  {
    [Category("Specific")]
    [ReadOnly(true)]
    public string DbName { get; set; }
  }
}
