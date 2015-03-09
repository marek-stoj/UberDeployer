namespace UberDeployer.Core.TeamCity.ApiModels
{
  public class TeamCityBuild
  {
    public string Id { get; set; }

    public string BuildTypeId { get; set; }

    public string Number { get; set; }

    public bool Pinned { get; set; }

    public string BranchName { get; set; }
  }
}