using System;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Iis;

namespace UberDeployer.Core.Deployment
{
  public class CreateAppPoolDeploymentStep : DeploymentStep
  {
    private readonly IIisManager _iisManager;
    private readonly string _machineName;
    private readonly IisAppPoolInfo _appPoolInfo;

    #region Constructor(s)

    public CreateAppPoolDeploymentStep(ProjectInfo projectInfo, IIisManager iisManager, string machineName, IisAppPoolInfo appPoolInfo)
      : base(projectInfo)
    {
      Guard.NotNull(iisManager, "iisManager");
      Guard.NotNullNorEmpty(machineName, "machineName");
      Guard.NotNull(appPoolInfo, "appPoolInfo");

      _iisManager = iisManager;
      _machineName = machineName;
      _appPoolInfo = appPoolInfo;
    }

    #endregion

    #region Overrides of DeploymentStep

    protected override void DoExecute()
    {
      if (_iisManager.AppPoolExists(_machineName, _appPoolInfo.Name))
      {
        throw new InvalidOperationException(string.Format("Application pool named '{0}' already exists on '{1}'.", _appPoolInfo.Name, _machineName));
      }

      _iisManager.CreateAppPool(_machineName, _appPoolInfo);
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Create an application pool named '{0}' on '{1}'. Version: '{2}'. Mode: '{3}'.",
            _appPoolInfo.Name,
            _machineName,
            _appPoolInfo.Version,
            _appPoolInfo.Mode);
      }
    }

    #endregion
  }
}
