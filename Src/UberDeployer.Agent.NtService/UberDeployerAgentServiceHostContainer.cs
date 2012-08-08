using System;
using System.Collections.Generic;
using UberDeployer.Agent.Service;

namespace UberDeployer.Agent.NtService
{
  public class UberDeployerAgentServiceHostContainer : MyServiceHostContainer
  {
    protected override string ApplicationName
    {
      get { return "UberDeployer.Agent.NtService"; }
    }

    protected override IEnumerable<Type> ServiceTypes
    {
      get { yield return typeof(AgentService); }
    }
  }
}
