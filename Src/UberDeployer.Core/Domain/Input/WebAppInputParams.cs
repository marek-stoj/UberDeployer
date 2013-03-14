using System.Collections.Generic;

namespace UberDeployer.Core.Domain.Input
{
  public class WebAppInputParams : InputParams
  {
    public WebAppInputParams(IEnumerable<string> onlyIncludedWebMachines = null)
    {
      if (onlyIncludedWebMachines != null)
      {
        OnlyIncludedWebMachines = new List<string>(onlyIncludedWebMachines);
      }
    }

    /// <summary>
    /// Can be null - it will mean that we want to deploy to all web machines on target environment.
    /// </summary>
    public IEnumerable<string> OnlyIncludedWebMachines { get; private set; }
  }
}
