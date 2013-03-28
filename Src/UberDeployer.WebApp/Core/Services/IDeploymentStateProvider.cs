using System;

namespace UberDeployer.WebApp.Core.Services
{
  public interface IDeploymentStateProvider
  {
    void SetDeploymentState(Guid deploymentId, DeploymentState deploymentState);

    DeploymentState FindDeploymentState(Guid deploymentId);

    void RemoveAllDeploymentStates(string userIdentity);
  }
}
