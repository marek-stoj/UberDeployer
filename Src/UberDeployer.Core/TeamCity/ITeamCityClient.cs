using System.Collections.Generic;
using UberDeployer.Core.TeamCity.Models;

namespace UberDeployer.Core.TeamCity
{
  public interface ITeamCityClient
  {
    IEnumerable<Project> GetAllProjects();

    Project GetProjectByName(string projectName);

    ProjectDetails GetProjectDetails(Project project);

    ProjectConfigurationDetails GetProjectConfigurationDetails(ProjectConfiguration projectConfiguration);

    ProjectConfigurationBuildsList GetProjectConfigurationBuilds(ProjectConfigurationDetails projectConfigurationDetails, int startIndex, int maxCount);

    void DownloadArtifacts(ProjectConfigurationBuild projectConfigurationBuild, string destinationFilePath);
  }
}
