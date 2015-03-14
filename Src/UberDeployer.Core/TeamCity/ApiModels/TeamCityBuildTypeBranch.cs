using Newtonsoft.Json;

namespace UberDeployer.Core.TeamCity.ApiModels
{
  public class TeamCityBuildTypeBranch
  {
    public string Name { get; set; }

    [JsonProperty("default")]
    public bool IsDefault { get; set; }
  }
}