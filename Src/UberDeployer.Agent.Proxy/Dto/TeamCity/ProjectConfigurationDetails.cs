namespace UberDeployer.Agent.Proxy.Dto.TeamCity
{
  public class ProjectConfigurationDetails
  {
    public string Id { get; set; }

    public string Name { get; set; }

    public string Href { get; set; }

    public string WebUrl { get; set; }

    public string Description { get; set; }

    public bool IsPaused { get; set; }

    public Project Project { get; set; }

    public BuildsLocation BuildsLocation { get; set; }
  }
}
