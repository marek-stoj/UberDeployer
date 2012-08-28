using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace UberDeployer.WinApp.ViewModels.PropertyGrids
{
  public class EnvironmentInfoInPropertyGridViewModel
  {
    [ReadOnly(true)]
    public string Name { get; set; }

    [ReadOnly(true)]
    public string ConfigurationTemplateName { get; set; }

    [ReadOnly(true)]
    public string AppServerMachineName { get; set; }

    [ReadOnly(true)]
    public string FailoverClusterMachineName { get; set; }

    [Browsable(false)]
    public List<string> WebServerMachineNames { get; set; }

    [ReadOnly(true)]
    public string TerminalServerMachineName { get; set; }

    [ReadOnly(true)]
    public string DatabaseServerMachineName { get; set; }

    [ReadOnly(true)]
    public string NtServicesBaseDirPath { get; set; }

    [ReadOnly(true)]
    public string WebAppsBaseDirPath { get; set; }

    [ReadOnly(true)]
    public string SchedulerAppsBaseDirPath { get; set; }

    [ReadOnly(true)]
    public string TerminalAppsBaseDirPath { get; set; }

    [ReadOnly(true)]
    public bool EnableFailoverClusteringForNtServices { get; set; }

    [Browsable(false)]
    public List<EnvironmentUserInPropertyGridVieModel> EnvironmentUsers { get; set; }

    [Browsable(false)]
    public List<ProjectToFailoverClusterGroupMappingInPropertyGridViewModel> ProjectToFailoverClusterGroupMappings { get; set; }

    [TypeConverter(typeof(WebServerMachineNamesCollectionConverter))]
    [Editor(typeof(ReadOnlyUITypeEditor), typeof(UITypeEditor))]
    [ReadOnly(true)]
    public WebServerMachineNameCollection WebServerMachinesNameCollection
    {
      get { return new WebServerMachineNameCollection(WebServerMachineNames); }
    }

    [TypeConverter(typeof(EnvironmentUsersCollectionConverter))]
    [Editor(typeof(ReadOnlyUITypeEditor), typeof(UITypeEditor))]
    [ReadOnly(true)]
    public EnvironmentUsersCollection EnvironmentUsersCollection
    {
      get { return new EnvironmentUsersCollection(EnvironmentUsers); }
    }

    [TypeConverter(typeof(ProjectToFailoverClusterGroupMappingsCollectionConverter))]
    [Editor(typeof(ReadOnlyUITypeEditor), typeof(UITypeEditor))]
    [ReadOnly(true)]
    public ProjectToFailoverClusterGroupMappingsCollection ProjectToFailoverClusterGroupMappingsCollection
    {
      get { return new ProjectToFailoverClusterGroupMappingsCollection(ProjectToFailoverClusterGroupMappings); }
    }
  }
}
