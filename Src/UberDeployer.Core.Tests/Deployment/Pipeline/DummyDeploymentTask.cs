using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Tests.Deployment.Pipeline
{
  public class DummyDeploymentTask : DeploymentTask
  {
    public DummyDeploymentTask(IEnvironmentInfoRepository environmentInfoRepository)
      : base(environmentInfoRepository)
    {
    }

    protected override void DoPrepare()
    {
      // do nothing
    }      }
}
