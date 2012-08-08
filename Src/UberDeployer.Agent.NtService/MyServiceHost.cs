using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace UberDeployer.Agent.NtService
{
  public class MyServiceHost : ServiceHost
  {
    public MyServiceHost(Type serviceType)
      : base(serviceType)
    {
      ApplyCommonServiceHostBehaviors();
    }

    private void ApplyCommonServiceHostBehaviors()
    {
      KeyedByTypeCollection<IServiceBehavior> serviceBehaviors = Description.Behaviors;

      if (!ServiceHostContainsBehavior(typeof(UnhandledExceptionsLoggingBehaviorAttribute)))
      {
        serviceBehaviors.Add(new UnhandledExceptionsLoggingBehaviorAttribute());
      }
    }

    private bool ServiceHostContainsBehavior(Type serviceBehaviorType)
    {
      return Description.Behaviors.Any(sb => sb.GetType() == serviceBehaviorType);
    }
  }
}
