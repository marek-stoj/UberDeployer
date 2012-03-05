using System;
using System.Linq;
using UberDeployer.Core.Domain;
using UberDeployer.Core.TeamCity;
using UberDeployer.Core.TeamCity.Models;

namespace UberDeployer.Core.DataAccess
{
  public class TeamCityArtifactsRepository : IArtifactsRepository
  {
    private readonly ITeamCityClient _teamCityClient;

    #region Constructor(s)

    public TeamCityArtifactsRepository(ITeamCityClient teamCityClient)
    {
      if (teamCityClient == null)
      {
        throw new ArgumentNullException("teamCityClient");
      }

      _teamCityClient = teamCityClient;
    }

    #endregion

    #region IArtifactsRepository Members

    public void GetArtifacts(string projectName, string projectConfigurationName, string projectConfigurationBuildId, string destinationFilePath)
    {
      if (string.IsNullOrEmpty(projectName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "projectName");
      }

      if (string.IsNullOrEmpty(projectConfigurationName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "projectConfigurationName");
      }

      if (string.IsNullOrEmpty(projectConfigurationBuildId))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "projectConfigurationBuildId");
      }

      if (string.IsNullOrEmpty(destinationFilePath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "destinationFilePath");
      }

      // TODO IMM HI: check for nulls

      Project project = _teamCityClient.GetProjectByName(projectName);
      ProjectDetails projectDetails = _teamCityClient.GetProjectDetails(project);

      ProjectConfiguration projectConfiguration =
        projectDetails.ConfigurationsList.Configurations
          .Where(pd => pd.Name == projectConfigurationName)
          .SingleOrDefault();

      ProjectConfigurationDetails projectConfigurationDetails =
        _teamCityClient.GetProjectConfigurationDetails(projectConfiguration);

      ProjectConfigurationBuild projectConfigurationBuild =
        FindProjectConfigurationBuild(projectConfigurationDetails, projectConfigurationBuildId);

      if (projectConfigurationBuild == null)
      {
        throw new ArgumentException(string.Format("Couldn't find build with id '{0}' for project '{1} ({2})'.", projectConfigurationBuildId, projectName, projectConfigurationName), "projectConfigurationBuildId");
      }

      if (projectConfigurationBuild.Status != BuildStatus.Success)
      {
        throw new InvalidOperationException(string.Format("Can't get artifacts for a build which is not successful. Project name: '{0}'. Project configuration name: '{1}'. Project configuration build id: '{2}'.", projectName, projectConfigurationName, projectConfigurationBuildId));
      }

      _teamCityClient.DownloadArtifacts(projectConfigurationBuild, destinationFilePath);
    }

    // TODO IMM HI: we can probably optimize this by telling TeamCity to give as the build with the specified id
    private ProjectConfigurationBuild FindProjectConfigurationBuild(ProjectConfigurationDetails projectConfigurationDetails, string projectConfigurationBuildId)
    {
      const int buildsPerPage = 10;
      int startIndex = 0;

      while (true)
      {
        ProjectConfigurationBuildsList projectConfigurationBuildsList =
          _teamCityClient.GetProjectConfigurationBuilds(projectConfigurationDetails, startIndex, buildsPerPage);

        if (projectConfigurationBuildsList.Count == 0)
        {
          return null;
        }

        ProjectConfigurationBuild projectConfigurationBuild =
          projectConfigurationBuildsList.Builds
            .Where(pcb => pcb.Id == projectConfigurationBuildId)
            .SingleOrDefault();

        if (projectConfigurationBuild != null)
        {
          return projectConfigurationBuild;
        }

        startIndex += buildsPerPage;
      }
    }

    #endregion
  }
}
