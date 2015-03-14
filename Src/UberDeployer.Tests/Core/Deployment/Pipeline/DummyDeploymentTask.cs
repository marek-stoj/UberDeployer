using UberDeployer.Core.Deployment.Tasks;
using UberDeployer.Core.Domain;

namespace UberDeployer.Tests.Core.Deployment.Pipeline
{
  public class DummyDeploymentTask : DeploymentTask
  {
    public DummyDeploymentTask(IProjectInfoRepository projectInfoRepository, IEnvironmentInfoRepository environmentInfoRepository)
      : base(projectInfoRepository, environmentInfoRepository)
    {
    }
  }
}
