using System.Collections.Generic;

namespace UberDeployer.Core.TeamCity.ApiModels
{
  public class TeamCityBuildType
  {
    public TeamCityBuildType()
    {
      Branches = new List<TeamCityBuildTypeBranch>();
    }

    public string Id { get; set; }

    public string Name { get; set; }

    public string ProjectName { get; set; }

    public string ProjectId { get; set; }

    public List<TeamCityBuildTypeBranch> Branches { get; set; }
  }
}
