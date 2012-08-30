using System;

namespace UberDeployer.WebApp.Core.Models.History
{
  public class DeploymentRequestViewModel
  {
    public DateTime DateFinished { get; set; }

    public string RequesterIdentity { get; set; }

    public string ProjectName { get; set; }

    public string ProjectConfigurationName { get; set; }

    public string ProjectConfigurationBuildId { get; set; }

    public string TargetEnvironmentName { get; set; }

    public bool FinishedSuccessfully { get; set; }
  }
}
