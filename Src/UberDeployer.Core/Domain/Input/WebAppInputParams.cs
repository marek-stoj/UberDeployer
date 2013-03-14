using System.Collections.Generic;

namespace UberDeployer.Core.Domain.Input
{
  public class WebAppInputParams : InputParams
  {
    public List<string> WebMachines { get; set; }
  }
}