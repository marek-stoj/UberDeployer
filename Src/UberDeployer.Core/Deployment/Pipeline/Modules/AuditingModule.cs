using System;
using UberDeployer.Core.Domain;

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

    public void OnDeploymentTaskStarting(DeploymentInfo deploymentInfo, DeploymentTask deploymentTask, DeploymentContext deploymentContext)
    {
      // do nothing
    }

    public void OnDeploymentTaskFinished(DeploymentInfo deploymentInfo, DeploymentTask deploymentTask, DeploymentContext deploymentContext)
    {
      if (deploymentInfo.IsSimulation)
      {
        return;
      }

      var deploymentRequest =
        new DeploymentRequest
          {
            RequesterIdentity = deploymentContext.RequesterIdentity,
            DateStarted = deploymentContext.DateStarted,
            DateFinished = deploymentContext.DateFinished,
            ProjectName = deploymentInfo.ProjectName,
            ProjectConfigurationName = deploymentInfo.ProjectConfigurationName,
            ProjectConfigurationBuildId = deploymentInfo.ProjectConfigurationBuildId,
            TargetEnvironmentName = deploymentInfo.TargetEnvironmentName,
            FinishedSuccessfully = deploymentContext.FinishedSuccessfully,
          };

      _deploymentRequestRepository.AddDeploymentRequest(deploymentRequest);
    }

    #endregion
  }
}
