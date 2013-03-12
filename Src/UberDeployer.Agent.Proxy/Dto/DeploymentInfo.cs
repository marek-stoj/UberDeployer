using System.Collections.Generic;

namespace UberDeployer.Agent.Proxy.Dto
{
  public class DeploymentInfo
  {
    public string ProjectName { get; set; }
    public string ProjectConfigurationName { get; set; }
    public string ProjectConfigurationBuildId { get; set; }
    public string TargetEnvironmentName { get; set; }
    public List<string> TargetMachines { get; set; }
  }
}
