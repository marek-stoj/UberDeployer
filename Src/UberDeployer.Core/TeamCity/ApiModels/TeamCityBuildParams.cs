namespace UberDeployer.Core.TeamCity.ApiModels
{
  public class TeamCityBuildParams
  {
    public int Skip { get; set; }

    public int Take { get; set; }

    public string BranchName { get; set; }
  }
}