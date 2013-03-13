using System.Collections.Generic;

namespace UberDeployer.Core.Domain
{
  public class WebInputParams : InputParams
  {
    public List<string> WebMachines { get; set; }
  }
}