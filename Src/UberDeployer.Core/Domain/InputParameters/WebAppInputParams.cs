using System.Collections.Generic;

namespace UberDeployer.Core.Domain.InputParameters
{
  public class WebAppInputParams : InputParams
  {
    public List<string> WebMachines { get; set; }
  }
}