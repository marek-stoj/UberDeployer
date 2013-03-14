using System;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Management.MsDeploy;

namespace UberDeployer.Core.Deployment
{
  public class DeployWebDeployPackageDeploymentStep : DeploymentStep
  {
    private readonly IMsDeploy _msDeploy;
    private readonly string _webServerMachineName;
    private readonly Lazy<string> _packageFilePathProvider;

    #region Constructor(s)

    public DeployWebDeployPackageDeploymentStep(IMsDeploy msDeploy, string webServerMachineName, Lazy<string> packageFilePathProvider)
    {
      Guard.NotNull(msDeploy, "msDeploy");
      Guard.NotNullNorEmpty(webServerMachineName, "webServerMachineName");
      Guard.NotNull(packageFilePathProvider, "packageFilePathProvider");

      _msDeploy = msDeploy;
      _webServerMachineName = webServerMachineName;
      _packageFilePathProvider = packageFilePathProvider;
    }

    #endregion

    #region Overrides of DeploymentTaskBase

    protected override void DoExecute()
    {
      // TODO IMM HI: dependency
      var msDeployArgs =
        new[]
          {
            "-verb:sync",
            string.Format("-source:package=\"{0}\"", _packageFilePathProvider.Value),
            string.Format("-dest:auto,computerName=\"{0}\"", _webServerMachineName),
          };

      string consoleOutput;

      _msDeploy.Run(msDeployArgs, out consoleOutput);
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Deploy WebDeploy package to machine '{0}' from '{1}'.",
            _webServerMachineName,
            _packageFilePathProvider.Value);
      }
    }

    #endregion
  }
}
