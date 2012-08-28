using System.ComponentModel;
using System.Runtime.Serialization;

namespace UberDeployer.WinApp.ViewModels.PropertyGrids
{
  [KnownType(typeof(NtServiceProjectInfoInPropertyGridViewModel))]
  [KnownType(typeof(WebAppProjectInfoInPropertyGridViewModel))]
  [KnownType(typeof(WebServiceProjectInfoInPropertyGridViewModel))]
  [KnownType(typeof(TerminalAppProjectInfoInPropertyGridViewModel))]
  [KnownType(typeof(SchedulerAppProjectInfoInPropertyGridViewModel))]
  [KnownType(typeof(DbProjectInfoInPropertyGridViewModel))]
  public class ProjectInfoInPropertyGridViewModel
  {
    [Category("Common")]
    [ReadOnly(true)]
    public string Name { get; set; }

    [Category("Common")]
    [ReadOnly(true)]
    public string Type { get; set; }

    [Category("Common")]
    [ReadOnly(true)]
    public string ArtifactsRepositoryName { get; set; }

    [Category("Common")]
    [ReadOnly(true)]
    public string ArtifactsRepositoryDirName { get; set; }

    [Category("Common")]
    [ReadOnly(true)]
    public bool ArtifactsAreEnvironmentSpecific { get; set; }
  }
}
