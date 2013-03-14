using System.Collections.Generic;

namespace UberDeployer.Agent.Proxy.Dto.Input
{
  public class WebAppInputParams : InputParams
  {
    /// <summary>
    /// Can be null - it will mean that we want to deploy to all web machines on target environment.
    /// </summary>
    public List<string> OnlyIncludedWebMachines { get; set; }
  }
}
