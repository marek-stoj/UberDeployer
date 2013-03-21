using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.NtServices;

namespace UberDeployer.Core.Deployment
{
  public class StartNtServiceDeploymentStep : NtServiceControlDeploymentStep
  {
    #region Constructor(s)

    public StartNtServiceDeploymentStep(ProjectInfo projectInfo, INtServiceManager ntServiceManager, string machineName, string serviceName)
      : base(projectInfo, ntServiceManager, machineName, serviceName, NtServiceControlAction.Start)
    {
    }

    #endregion
  }
}
