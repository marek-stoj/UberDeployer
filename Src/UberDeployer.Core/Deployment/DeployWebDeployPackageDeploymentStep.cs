using System;
using UberDeployer.Core.Management.MsDeploy;

namespace UberDeployer.Core.Deployment
{
  public class DeployWebDeployPackageDeploymentStep : DeploymentStep
  {
    private readonly string _packageFilePath;
    private readonly string _webServerMachineName;
    private readonly IMsDeploy _msDeploy;

    #region Constructor(s)

    public DeployWebDeployPackageDeploymentStep(IMsDeploy msDeploy, string packageFilePath, string webServerMachineName)
    {
      if (string.IsNullOrEmpty(packageFilePath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "packageFilePath");
      }

      if (string.IsNullOrEmpty(webServerMachineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "webServerMachineName");
      }

      if (msDeploy == null)
      {
        throw new ArgumentException("Argument can't be null", "msDeploy");
      }
         

      _packageFilePath = packageFilePath;
      _webServerMachineName = webServerMachineName;
      _msDeploy = msDeploy;
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
            string.Format("-source:package=\"{0}\"", _packageFilePath),
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
            _packageFilePath);
      }
    }

    #endregion
  }
}
