using System;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Iis;

namespace UberDeployer.Core.Deployment
{
  public class SetAppPoolDeploymentStep : DeploymentStep
  {
    private readonly IIisManager _iisManager;
    private readonly string _machineName;
    private readonly string _webSiteName;
    private readonly IisAppPoolInfo _appPoolInfo;
    private readonly string _webAppName;

    public SetAppPoolDeploymentStep(ProjectInfo projectInfo, IIisManager iisManager, string machineName, string webSiteName, IisAppPoolInfo appPoolInfo, string webAppName = null)
      : base(projectInfo)
    {
      Guard.NotNull(iisManager, "iisManager");
      Guard.NotNullNorEmpty(machineName, "machineName");
      Guard.NotNullNorEmpty(webSiteName, "webSiteName");
      Guard.NotNull(appPoolInfo, "appPoolInfo");

      _iisManager = iisManager;
      _machineName = machineName;
      _webSiteName = webSiteName;
      _appPoolInfo = appPoolInfo;
      _webAppName = webAppName;
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
      get
      {
        string fullWebAppName = _webSiteName;

        if (!string.IsNullOrEmpty(_webAppName))
        {
          fullWebAppName += string.Format("/{0}", _webAppName);
        }

        return fullWebAppName;
      }
    }

    #endregion
  }
}
