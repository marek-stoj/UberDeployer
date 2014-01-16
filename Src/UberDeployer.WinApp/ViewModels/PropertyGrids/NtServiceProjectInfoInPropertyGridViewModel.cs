using System.ComponentModel;

namespace UberDeployer.WinApp.ViewModels.PropertyGrids
{
  public class NtServiceProjectInfoInPropertyGridViewModel : ProjectInfoInPropertyGridViewModel
  {
    [Category("Specific")]
    [ReadOnly(true)]
    public string NtServiceName { get; set; }

    [Category("Specific")]
    [ReadOnly(true)]
    public string NtServiceDirName { get; set; }

    [Category("Specific")]
    [ReadOnly(true)]
    public string NtServiceDisplayName { get; set; }

    [Category("Specific")]
    [ReadOnly(true)]
    public string NtServiceExeName { get; set; }

    [Category("Specific")]
    [ReadOnly(true)]
    public string NtServiceUserId { get; set; }
  }
}
