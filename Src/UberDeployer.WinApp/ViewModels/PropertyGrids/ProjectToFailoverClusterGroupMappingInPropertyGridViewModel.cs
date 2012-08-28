using System.ComponentModel;

namespace UberDeployer.WinApp.ViewModels.PropertyGrids
{
  [TypeConverter(typeof(ProjectToFailoverClusterGroupMappingConverter))]
  public class ProjectToFailoverClusterGroupMappingInPropertyGridViewModel
  {
    [ReadOnly(true)]
    public string ProjectName { get; set; }

    [ReadOnly(true)]
    public string ClusterGroupName { get; set; }
  }
}
