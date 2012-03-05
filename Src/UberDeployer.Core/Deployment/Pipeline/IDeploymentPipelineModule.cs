using System;

namespace UberDeployer.Core.Deployment.Pipeline
{
  // TODO IMM HI: refactor
  public interface IDeploymentPipelineModule
  {
    void OnDeploymentTaskStarting(DeploymentTask deploymentTask);

    void OnDeploymentTaskFinished(DeploymentTask deploymentTask, DateTime dateRequested, string projectName, string targetEnvironmentName, bool finishedSuccessfully);
  }
}
