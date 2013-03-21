using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.NtServices;

namespace UberDeployer.Core.Deployment
{
  public class StopNtServiceDeploymentStep : NtServiceControlDeploymentStep
  {
    #region Constructor(s)

    public StopNtServiceDeploymentStep(ProjectInfo projectInfo, INtServiceManager ntServiceManager, string machineName, string serviceName)
      : base(projectInfo, ntServiceManager, machineName, serviceName, NtServiceControlAction.Stop)
    {
    }

    #endregion
  }
}
