using System.Collections.Generic;
using Newtonsoft.Json;

namespace UberDeployer.Core.TeamCity.Models
{
  public class ProjectConfigurationBuildsList
  {
    #region Overrides of object

    public override string ToString()
    {
      return
        string.Format(
          "BuildsCount: {0}",
          Builds != null ? Builds.Count : 0);
    }

    #endregion

    #region Properties

    [JsonProperty("build")]
    public List<ProjectConfigurationBuild> Builds { get; set; }

    public int Count { get; set; }

    #endregion
  }
}
