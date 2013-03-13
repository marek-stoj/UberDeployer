using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment.Pipeline
{
  public interface IDeploymentPipeline
  {
    void AddModule(IDeploymentPipelineModule module);

    void StartDeployment(DeploymentInfo deploymentInfo, DeploymentTask deploymentTask, DeploymentContext deploymentContext);
  }
}
