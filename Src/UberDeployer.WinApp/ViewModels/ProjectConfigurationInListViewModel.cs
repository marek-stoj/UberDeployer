using System;
using UberDeployer.Agent.Proxy.Dto.TeamCity;

namespace UberDeployer.WinApp.ViewModels
{
  public class ProjectConfigurationInListViewModel
  {
    public ProjectConfigurationInListViewModel(string projectName, ProjectConfiguration projectConfiguration)
    {
      if (string.IsNullOrEmpty(projectName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "projectName");
      }

      if (projectConfiguration == null)
      {
        throw new ArgumentNullException("projectConfiguration");
      }

      ProjectName = projectName;
      ProjectConfiguration = projectConfiguration;
    }

    public string ProjectName { get; private set; }

    public ProjectConfiguration ProjectConfiguration { get; private set; }

    public string Name
    {
      get { return ProjectConfiguration.Name; }
    }
  }
}
