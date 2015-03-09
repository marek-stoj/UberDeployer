using System.Collections.Generic;

using UberDeployer.Core.TeamCity.ApiModels;

namespace UberDeployer.Core.TeamCity
{
  public interface ITeamCityRestClient
  {
    IEnumerable<TeamCityProject> GetAllProjects();

    TeamCityProject GetProject(string projectName);

    IEnumerable<TeamCityBuildType> GetBuildTypes(string projectName);

    IEnumerable<TeamCityBuildType> GetBuildTypesWithBranches(string projectName);

    IEnumerable<TeamCityBuild> GetBuilds(string buildTypeId, string branchName, int start, int count, bool onlySuccessful);

    TeamCityBuild GetBuild(string buildId);

    TeamCityBuild GetLastSuccessfulBuild(string buildTypeId);

    void DownloadArtifacts(string buildId, string destinationFilePath);
  }
}
