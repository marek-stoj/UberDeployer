namespace UberDeployer.Core.TeamCity.Models
{
  public class ProjectConfiguration
  {
    public ProjectConfiguration()
    {
      Branches = new ProjectBranchList();
    }

    public string Id { get; set; }

    public string Name { get; set; }

    public string Href { get; set; }

    public string WebUrl { get; set; }

    public string ProjectId { get; set; }

    public string ProjectName { get; set; }

    public ProjectBranchList Branches { get; set; }

    public override string ToString()
    {
      return
        string.Format(
          "Id: {0}, Name: {1}, Href: {2}, WebUrl: {3}, ProjectId: {4}, ProjectName: {5}",
          Id,
          Name,
          Href,
          WebUrl,
          ProjectId,
          ProjectName);
    }
  }
}
