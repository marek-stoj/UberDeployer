namespace UberDeployer.Core.Deployment.Pipeline
{
  // TODO IMM HI: refactor
  public interface IDeploymentPipelineModule
  {
    void OnDeploymentTaskStarting(DeploymentTask deploymentTask, DeploymentContext deploymentContext);

    void OnDeploymentTaskFinished(DeploymentTask deploymentTask, DeploymentContext deploymentContext);
  }
}
