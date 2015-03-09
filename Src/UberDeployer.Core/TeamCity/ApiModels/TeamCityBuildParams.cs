namespace UberDeployer.Core.TeamCity.ApiModels
{
  public class TeamCityBuildParams
  {
    public int Skip { get; set; }

    public int Take { get; set; }

    public string BranchName { get; set; }

    public bool OnlySuccessful { get; set; }

    public static TeamCityBuildParams Default
    {
      get
      {
        return new TeamCityBuildParams
        {
          Skip = 0,
          Take = 20,
          OnlySuccessful = true
        };
      }
    }
  }
}