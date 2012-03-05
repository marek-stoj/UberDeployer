using Newtonsoft.Json;

namespace UberDeployer.Core.TeamCity.Models
{
  public class ProjectDetails
  {
    #region Overrides of object

    public override string ToString()
    {
      return
        string.Format(
          "ProjectId: {0}, ProjectName: {1}, ProjectHref: {2}, ProjectWebUrl: {3}, IsProjectArchived: {4}, ConfigurationsCount: {5}",
          ProjectId,
          ProjectName,
          ProjectHref,
          ProjectWebUrl,
          IsProjectArchived,
          ConfigurationsList != null && ConfigurationsList.Configurations != null ? ConfigurationsList.Configurations.Count : 0);
    }

    #endregion

    #region Properties

    [JsonProperty("id")]
    public string ProjectId { get; set; }

    [JsonProperty("name")]
    public string ProjectName { get; set; }

    [JsonProperty("href")]
    public string ProjectHref { get; set; }

    [JsonProperty("webUrl")]
    public string ProjectWebUrl { get; set; }

    [JsonProperty("description")]
    public string ProjectDescription { get; set; }

    [JsonProperty("archived")]
    public bool IsProjectArchived { get; set; }

    [JsonProperty("buildTypes")]
    public ProjectConfigurationsList ConfigurationsList { get; set; }

    #endregion
  }
}
