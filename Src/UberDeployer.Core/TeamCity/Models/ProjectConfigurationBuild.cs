namespace UberDeployer.Core.TeamCity.Models
{
  public class ProjectConfigurationBuild
  {
    public string Id { get; set; }

    public string BuildTypeId { get; set; }

    public string Number { get; set; }

    public string StartDate { get; set; }

    public BuildStatus Status { get; set; }

    public string WebUrl { get; set; }

    public override string ToString()
    {
      return
        string.Format(
          "Id: {0}, BuildTypeId: {1}, Number: {2}, Status: {3}, WebUrl: {4}",
          Id,
          BuildTypeId,
          Number,
          Status,
          WebUrl);
    }
  }
}
