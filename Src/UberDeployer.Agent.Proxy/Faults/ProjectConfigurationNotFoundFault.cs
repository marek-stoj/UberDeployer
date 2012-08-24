namespace UberDeployer.Agent.Proxy.Faults
{
  public class ProjectConfigurationNotFoundFault
  {
    public string ProjectName { get; set; }

    public string ProjectConfigurationName { get; set; }
  }
}
