using System;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Iis;

namespace UberDeployer.Core.Deployment
{
  public class SetAppPoolDeploymentStep : DeploymentStep
  {
    private readonly IIisManager _iisManager;
    private readonly string _machineName;
    private readonly string _iisSiteName;
    private readonly string _webAppName;
    private readonly IisAppPoolInfo _appPoolInfo;

    public SetAppPoolDeploymentStep(IIisManager iisManager, string machineName, string iisSiteName, string webAppName, IisAppPoolInfo appPoolInfo)
    {
      if (iisManager == null)
      {
        throw new ArgumentNullException("iisManager");
      }
      

      if (string.IsNullOrEmpty(machineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "machineName");
      }

      if (string.IsNullOrEmpty(iisSiteName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "iisSiteName");
      }

      if (string.IsNullOrEmpty(webAppName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "webAppName");
      }

      if (appPoolInfo == null)
      {
        throw new ArgumentNullException("appPoolInfo");
      }

      _iisManager = iisManager;
      _machineName = machineName;
      _iisSiteName = iisSiteName;
      _webAppName = webAppName;
      _appPoolInfo = appPoolInfo;
    }

    #region Overrides of DeploymentStep

    protected override void DoExecute()
    {
      if (!_iisManager.AppPoolExists(_machineName, _appPoolInfo.Name))
      {
        throw new InvalidOperationException(string.Format("Application pool named '{0}' doesn't exist on '{1}'.", _appPoolInfo.Name, _machineName));
      }

      // TODO IMM HI: we could verify here that the existing app pool has the same version and mode as the one needed by the web app
      _iisManager.SetAppPool(_machineName, FullWebAppName, _appPoolInfo.Name);
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Set application pool of web app '{0}' to '{1}' on '{2}'.",
            FullWebAppName,
            _appPoolInfo.Name,
            _machineName);
      }
    }

    protected string FullWebAppName
    {
      get { return string.Format("{0}/{1}", _iisSiteName, _webAppName); }
    }

    #endregion
  }
}
