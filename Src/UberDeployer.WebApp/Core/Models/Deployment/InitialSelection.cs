namespace UberDeployer.WebApp.Core.Models.Deployment
{
  public class InitialSelection
  {
    public string TargetEnvironmentName { get; set; }

    public string ProjectName { get; set; }

    public string ProjectConfigurationName { get; set; }

    public string ProjectConfigurationBuildId { get; set; }
  }
}
