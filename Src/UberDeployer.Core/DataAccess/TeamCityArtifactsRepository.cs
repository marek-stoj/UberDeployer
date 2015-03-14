using System;

using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;
using UberDeployer.Core.TeamCity;

namespace UberDeployer.Core.DataAccess
{
  public class TeamCityArtifactsRepository : IArtifactsRepository
  {
    private readonly ITeamCityRestClient _teamCityClient;

    public TeamCityArtifactsRepository(ITeamCityRestClient teamCityClient)
    {
      if (teamCityClient == null)
      {
        throw new ArgumentNullException("teamCityClient");
      }

      _teamCityClient = teamCityClient;
    }

    public void GetArtifacts(string projectConfigurationBuildId, string destinationFilePath)
    {
      Guard.NotNullNorEmpty(projectConfigurationBuildId, "projectConfigurationBuildId");
      Guard.NotNullNorEmpty(destinationFilePath, "destinationFilePath");

      _teamCityClient.DownloadArtifacts(projectConfigurationBuildId, destinationFilePath);
    }
  }
}
