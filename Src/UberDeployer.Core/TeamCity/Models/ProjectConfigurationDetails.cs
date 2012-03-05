using Newtonsoft.Json;

namespace UberDeployer.Core.TeamCity.Models
{
  public class ProjectConfigurationDetails
  {
    #region Properties

    public string Id { get; set; }

    public string Name { get; set; }

    public string Href { get; set; }

    public string WebUrl { get; set; }

    public string Description { get; set; }

    [JsonProperty("paused")]
    public bool IsPaused { get; set; }

    public Project Project { get; set; }

    [JsonProperty("builds")]
    public BuildsLocation BuildsLocation { get; set; }

    #endregion
  }
}
