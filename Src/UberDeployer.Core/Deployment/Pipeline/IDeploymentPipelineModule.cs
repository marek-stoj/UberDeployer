using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment.Pipeline
{
  // TODO IMM HI: refactor
  public interface IDeploymentPipelineModule
  {
    void OnDeploymentTaskStarting(DeploymentInfo deploymentInfo, DeploymentTask deploymentTask, DeploymentContext deploymentContext);

    void OnDeploymentTaskFinished(DeploymentInfo deploymentInfo, DeploymentTask deploymentTask, DeploymentContext deploymentContext);
  }
}
