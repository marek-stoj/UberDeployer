using System;
using UberDeployer.Agent.Proxy.Dto.Input;

namespace UberDeployer.Agent.Proxy.Dto
{
  public class DeploymentInfo
  {
    public Guid DeploymentId { get; set; }

    public bool IsSimulation { get; set; }
    
    public string ProjectName { get; set; }

    public string ProjectConfigurationName { get; set; }

    public string ProjectConfigurationBuildId { get; set; }

    public string TargetEnvironmentName { get; set; }

    public InputParams InputParams { get; set; }
  }
}
