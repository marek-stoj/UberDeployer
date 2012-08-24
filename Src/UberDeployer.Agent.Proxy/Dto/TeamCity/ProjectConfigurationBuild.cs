namespace UberDeployer.Agent.Proxy.Dto.TeamCity
{
  public class ProjectConfigurationBuild
  {
    public string Id { get; set; }

    public string BuildTypeId { get; set; }

    public string Number { get; set; }

    public string StartDate { get; set; }

    public BuildStatus Status { get; set; }

    public string WebUrl { get; set; }
  }
}
