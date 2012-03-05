using System.Collections.Generic;
using Newtonsoft.Json;

namespace UberDeployer.Core.TeamCity.Models
{
  public class ProjectConfigurationsList
  {
    #region Overrides of object

    public override string ToString()
    {
      return string.Format("ConfigurationsCount: {0}", Configurations != null ? Configurations.Count : 0);
    }

    #endregion

    #region Properties

    [JsonProperty("buildType")]
    public List<ProjectConfiguration> Configurations { get; set; }

    #endregion
  }
}
