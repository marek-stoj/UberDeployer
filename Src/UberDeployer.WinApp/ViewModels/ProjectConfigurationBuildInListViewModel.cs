using UberDeployer.Core.TeamCity.Models;

namespace UberDeployer.WinApp.ViewModels
{
  public class ProjectConfigurationBuildInListViewModel
  {
    public ProjectConfigurationBuild ProjectConfigurationBuild { get; set; }

    public string Id
    {
      get { return ProjectConfigurationBuild.Id; }
    }

    public string Number
    {
      get { return ProjectConfigurationBuild.Number; }
    }

    public string Status
    {
      get { return ProjectConfigurationBuild.Status.ToString(); }
    }
  }
}
