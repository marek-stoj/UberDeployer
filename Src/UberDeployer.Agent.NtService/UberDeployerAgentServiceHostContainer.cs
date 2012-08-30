using System;
using System.Collections.Generic;
using UberDeployer.Agent.Service;
using UberDeployer.CommonConfiguration;

namespace UberDeployer.Agent.NtService
{
  public class UberDeployerAgentServiceHostContainer : MyServiceHostContainer
  {
    protected override void OnServiceHostsStarting()
    {
      Bootstraper.Bootstrap();

      base.OnServiceHostsStarting();
    }

    protected override IEnumerable<Type> ServiceTypes
    {
      get { yield return typeof(AgentService); }
    }
  }
}
