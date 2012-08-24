namespace UberDeployer.Agent.Proxy.Dto.TeamCity
{
  public class ProjectDetails
  {
    public string ProjectId { get; set; }

    public string ProjectName { get; set; }

    public string ProjectHref { get; set; }

    public string ProjectWebUrl { get; set; }

    public string ProjectDescription { get; set; }

    public bool IsProjectArchived { get; set; }

    public ProjectConfigurationsList ConfigurationsList { get; set; }
  }
}
