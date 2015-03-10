using Newtonsoft.Json;

namespace UberDeployer.Core.TeamCity.Models
{
  public class ProjectBranch
  {
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("default")]
    public bool IsDefault { get; set; }

    public override string ToString()
    {
      return string.Format("Name: {0}, Default {1}", Name, IsDefault ? "true" : "false");
    }
  }
}