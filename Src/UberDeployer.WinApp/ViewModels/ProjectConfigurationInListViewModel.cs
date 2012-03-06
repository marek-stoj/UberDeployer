using System;
using UberDeployer.Core.TeamCity.Models;

namespace UberDeployer.WinApp.ViewModels
{
  public class ProjectConfigurationInListViewModel
  {
    public ProjectConfigurationInListViewModel(ProjectConfiguration projectConfiguration)
    {
      if (projectConfiguration == null)
      {
        throw new ArgumentNullException("projectConfiguration");
      }

      ProjectConfiguration = projectConfiguration;
    }

    public ProjectConfiguration ProjectConfiguration { get; private set; }

    public string Name
    {
      get { return ProjectConfiguration.Name; }
    }
  }
}
