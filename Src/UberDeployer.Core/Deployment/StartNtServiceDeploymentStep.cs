using UberDeployer.Core.Management.NtServices;

namespace UberDeployer.Core.Deployment
{
  public class StartNtServiceDeploymentStep : NtServiceControlDeploymentStep
  {
    #region Constructor(s)

    public StartNtServiceDeploymentStep(INtServiceManager ntServiceManager, string machineName, string serviceName)
      : base(ntServiceManager, machineName, serviceName, NtServiceControlAction.Start)
    {
    }

    #endregion
  }
}
