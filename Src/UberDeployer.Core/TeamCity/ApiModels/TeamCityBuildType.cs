using System.Collections.Generic;

using Newtonsoft.Json;

namespace UberDeployer.Core.TeamCity.ApiModels
{
  public class TeamCityProject
  {
    public string Id { get; set; }

    public string Name { get; set; }
  }

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

  public class TeamCityBuildTypeBranch
  {
    public string Name { get; set; }

    [JsonProperty("default")]
    public bool IsDefault { get; set; }
  }

  public class TeamCityBuild
  {
    public string Id { get; set; }

    public string BuildTypeId { get; set; }

    public string Status { get; set; }

    public string BranchName { get; set; }
  }

  public class TeamCityBuildParams
  {
    public int Skip { get; set; }

    public int Take { get; set; }

    public string BranchName { get; set; }
  }
}
