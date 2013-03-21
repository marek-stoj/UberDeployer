using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Tests.Deployment.Pipeline
{
  public class DummyDeploymentTask : DeploymentTask
  {
    public DummyDeploymentTask(IProjectInfoRepository projectInfoRepository, IEnvironmentInfoRepository environmentInfoRepository)
      : base(projectInfoRepository, environmentInfoRepository)
    {
    }
  }
}
