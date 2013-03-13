using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  public abstract class  DeploymentStep : DeploymentTaskBase
  {
    protected override void DoPrepare()
    {
      // do nothing
    }
  }
}
