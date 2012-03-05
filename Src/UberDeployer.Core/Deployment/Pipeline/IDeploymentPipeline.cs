namespace UberDeployer.Core.Deployment.Pipeline
{
  public interface IDeploymentPipeline
  {
    void AddModule(IDeploymentPipelineModule module);

    void StartDeployment(DeploymentTask deploymentTask);
  }
}
