using System.Runtime.Serialization;

namespace UberDeployer.Agent.Proxy.Dto.Input
{
  [KnownType(typeof(DbInputParams))]
  [KnownType(typeof(NtServiceInputParams))]
  [KnownType(typeof(SchedulerAppInputParams))]
  [KnownType(typeof(TerminalAppInputParams))]
  [KnownType(typeof(WebAppInputParams))]
  [KnownType(typeof(WebServiceInputParams))]
  public abstract class InputParams
  {
  }
}
