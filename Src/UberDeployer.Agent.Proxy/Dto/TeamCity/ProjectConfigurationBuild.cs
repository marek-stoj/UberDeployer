namespace UberDeployer.Agent.Proxy.Dto.TeamCity
{
  public class ProjectConfigurationBuild
  {
    public string Id { get; set; }

    public string BuildTypeId { get; set; }

    public string Number { get; set; }

    public string StartDate { get; set; }

    public bool Pinned { get; set; }

    public string BranchName { get; set; }
  }
}
