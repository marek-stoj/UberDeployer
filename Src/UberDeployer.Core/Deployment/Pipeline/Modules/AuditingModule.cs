using System;

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

    public void OnDeploymentTaskStarting(DeploymentTask deploymentTask, DeploymentContext deploymentContext)
    {
      // do nothing
    }

    public void OnDeploymentTaskFinished(DeploymentTask deploymentTask, DeploymentContext deploymentContext)
    {
      var deploymentRequest =
        new DeploymentRequest
          {
            RequesterIdentity = deploymentContext.RequesterIdentity,
            DateStarted = deploymentContext.DateStarted,
            DateFinished = deploymentContext.DateFinished,
            ProjectName = deploymentTask.DeploymentInfo.ProjectName,
            ProjectConfigurationName = deploymentTask.DeploymentInfo.ProjectConfigurationName,
            ProjectConfigurationBuildId = deploymentTask.DeploymentInfo.ProjectConfigurationBuildId,
            TargetEnvironmentName = deploymentTask.DeploymentInfo.TargetEnvironmentName,
            FinishedSuccessfully = deploymentContext.FinishedSuccessfully,
          };

      _deploymentRequestRepository.AddDeploymentRequest(deploymentRequest);
    }

    #endregion
  }
}
