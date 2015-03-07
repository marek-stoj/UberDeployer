using System.Collections.Generic;
using Newtonsoft.Json;

namespace UberDeployer.Core.TeamCity.Models
{
  public class ProjectConfigurationBuildsList
  {
    [JsonProperty("build")]
    public List<ProjectConfigurationBuild> Builds { get; set; }

    public int Count { get; set; }

    public override string ToString()
    {
      return
        string.Format(
          "BuildsCount: {0}",
          Builds != null ? Builds.Count : 0);
    }
  }
}
