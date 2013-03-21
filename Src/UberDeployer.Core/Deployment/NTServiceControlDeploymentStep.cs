using System;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Management.NtServices;

namespace UberDeployer.Core.Deployment
{
  // TODO IMM HI: abstract away IServiceController so that we cant unit test this
  public abstract class NtServiceControlDeploymentStep : DeploymentStep
  {
    private readonly INtServiceManager _ntServiceManager;
    private readonly string _machineName;
    private readonly string _serviceName;
    private readonly NtServiceControlAction _action;

    #region Constructor(s)

    protected NtServiceControlDeploymentStep(INtServiceManager ntServiceManager, string machineName, string serviceName, NtServiceControlAction action)
    {
      Guard.NotNull(ntServiceManager, "ntServiceManager");
      Guard.NotNullNorEmpty(machineName, "machineName");
      Guard.NotNullNorEmpty(serviceName, "serviceName");

      _ntServiceManager = ntServiceManager;
      _machineName = machineName;
      _serviceName = serviceName;
      _action = action;
    }

    #endregion

    #region Overrides of DeploymentStep

    // TODO IMM HI: think about the timeout and leaving the services in inconsistent state when they take too long to start/stop
    protected override void DoExecute()
    {
      switch (_action)
      {
        case NtServiceControlAction.Start:
          _ntServiceManager.StartService(_machineName, _serviceName);
          break;
        
        case NtServiceControlAction.Stop:
          _ntServiceManager.StopService(_machineName, _serviceName);
          break;
        
        default:
          throw new NotSupportedException(string.Format("Unknown action: '{0}'.", _action));
      }
    }

    public override string Description
    {
      get { return string.Format("{0} NT service '{1}' on machine '{2}'.", _action, _serviceName, _machineName); }
    }

    #endregion
  }
}
