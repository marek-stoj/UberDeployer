using System;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment.Pipeline
{
  public interface IDeploymentPipeline
  {
    event EventHandler<DiagnosticMessageEventArgs> DiagnosticMessagePosted;

    void AddModule(IDeploymentPipelineModule module);

    void StartDeployment(DeploymentInfo deploymentInfo, DeploymentTask deploymentTask, DeploymentContext deploymentContext);
  }
}
