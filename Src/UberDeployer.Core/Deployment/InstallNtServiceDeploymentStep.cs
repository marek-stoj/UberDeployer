using System;
using UberDeployer.Core.Management.NtServices;

namespace UberDeployer.Core.Deployment
{
  public class InstallNtServiceDeploymentStep : DeploymentStep
  {
    private readonly INtServiceManager _ntServiceManager;
    private readonly string _machineName;
    private readonly NtServiceDescriptor _ntServiceDescriptor;

    #region Constructor(s)

    public InstallNtServiceDeploymentStep(INtServiceManager ntServiceManager, string machineName, NtServiceDescriptor ntServiceDescriptor)
    {
      if (ntServiceManager == null)
      {
        throw new ArgumentNullException("ntServiceManager");
      }

      if (string.IsNullOrEmpty(machineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "machineName");
      }

      if (ntServiceDescriptor == null)
      {
        throw new ArgumentNullException("ntServiceDescriptor");
      }

      _ntServiceManager = ntServiceManager;
      _machineName = machineName;
      _ntServiceDescriptor = ntServiceDescriptor;
    }

    #endregion

    #region Overrides of DeploymentTaskBase

    protected override void DoExecute()
    {
      _ntServiceManager.InstallService(_machineName, _ntServiceDescriptor);
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Install NT service '{0}' on machine '{1}'.",
            _ntServiceDescriptor.ServiceName,
            _machineName);
      }
    }

    #endregion
  }
}