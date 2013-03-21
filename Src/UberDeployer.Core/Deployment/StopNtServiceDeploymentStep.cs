using UberDeployer.Core.Management.NtServices;

namespace UberDeployer.Core.Deployment
{
  public class StopNtServiceDeploymentStep : NtServiceControlDeploymentStep
  {
    #region Constructor(s)

    public StopNtServiceDeploymentStep(INtServiceManager ntServiceManager, string machineName, string serviceName)
      : base(ntServiceManager, machineName, serviceName, NtServiceControlAction.Stop)
    {
    }

    #endregion
  }
}
