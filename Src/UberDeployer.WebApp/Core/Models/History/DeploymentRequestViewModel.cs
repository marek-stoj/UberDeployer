using System;

namespace UberDeployer.WebApp.Core.Models.History
{
  public class DeploymentRequestViewModel
  {
    public DateTime DateRequested { get; set; }

    public string RequesterIdentity { get; set; }

    public string ProjectName { get; set; }

    public string TargetEnvironmentName { get; set; }

    public bool FinishedSuccessfully { get; set; }
  }
}
