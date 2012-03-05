using System.Collections.Generic;

namespace UberDeployer.Core.Deployment.Pipeline.Modules
{
  public interface IDeploymentRequestRepository
  {
    void AddDeploymentRequest(DeploymentRequest deploymentRequest);
    
    IEnumerable<DeploymentRequest> GetDeploymentRequests(int startIndex, int maxCount);
  }
}
