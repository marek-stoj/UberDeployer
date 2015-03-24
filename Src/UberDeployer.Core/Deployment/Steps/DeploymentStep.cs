using UberDeployer.Core.Deployment.Tasks;

namespace UberDeployer.Core.Deployment.Steps
{
  public abstract class DeploymentStep : DeploymentTaskBase
  {
    #region Protected members

    protected override void DoPrepare()
    {
      // do nothing
    }

    protected override void DoExecute()
    {
      // do nothing
    }

    #endregion
  }
}
