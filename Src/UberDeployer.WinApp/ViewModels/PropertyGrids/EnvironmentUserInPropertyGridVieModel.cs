using System.ComponentModel;

namespace UberDeployer.WinApp.ViewModels.PropertyGrids
{
  [TypeConverter(typeof(EnvironmentUserConverter))]
  public class EnvironmentUserInPropertyGridVieModel
  {
    [ReadOnly(true)]
    public string Id { get; set; }

    [ReadOnly(true)]
    public string UserName { get; set; }
  }
}
