using System;

namespace UberDeployer.Agent.Proxy.Dto
{
  public class DeploymentRequest
  {
    public DateTime DateStarted { get; set; }
    
    public DateTime DateFinished { get; set; }

    public string RequesterIdentity { get; set; }

    public string ProjectName { get; set; }

    public string ProjectConfigurationName { get; set; }

    public string ProjectConfigurationBuildId { get; set; }

    public string TargetEnvironmentName { get; set; }

    public bool FinishedSuccessfully { get; set; }
  }
}
