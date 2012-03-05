using System;
using System.Security.Principal;

namespace UberDeployer.Core.Deployment.Pipeline.Modules
{
  public class AuditingModule : IDeploymentPipelineModule
  {
    private readonly IDeploymentRequestRepository _deploymentRequestRepository;

    #region Constructor(s)

    public AuditingModule(IDeploymentRequestRepository deploymentRequestRepository)
    {
      if (deploymentRequestRepository == null)
      {
        throw new ArgumentNullException("deploymentRequestRepository");
      }

      _deploymentRequestRepository = deploymentRequestRepository;
    }

    #endregion

    #region IDeploymentPipelineModule Members

    public void OnDeploymentTaskStarting(DeploymentTask deploymentTask)
    {
      // do nothing
    }

    public void OnDeploymentTaskFinished(DeploymentTask deploymentTask, DateTime dateRequested, string projectName, string targetEnvironmentName, bool finishedSuccessfully)
    {
      WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
      string requesterIdentity;

      if (windowsIdentity != null)
      {
        requesterIdentity = windowsIdentity.Name;
      }
      else
      {
        requesterIdentity = "(no windows identity)";
      }

      var deploymentRequest =
        new DeploymentRequest
          {
            RequesterIdentity = requesterIdentity,
            DateRequested = dateRequested,
            ProjectName = projectName,
            TargetEnvironmentName = targetEnvironmentName,
            FinishedSuccessfully = finishedSuccessfully,
          };

      _deploymentRequestRepository.AddDeploymentRequest(deploymentRequest);
    }

    #endregion
  }
}
