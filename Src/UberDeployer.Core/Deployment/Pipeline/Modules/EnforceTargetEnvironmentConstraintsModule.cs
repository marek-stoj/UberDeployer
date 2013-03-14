using System;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment.Pipeline.Modules
{
  // TODO IMM HI: KRD-specific; move somewhere else
  public class EnforceTargetEnvironmentConstraintsModule : IDeploymentPipelineModule
  {
    public const string ProductionEnvironmentName = "prod";
    public const string ProductionProjectConfigurationName = "Production";

    #region IDeploymentPipelineModule Members

    public void OnDeploymentTaskStarting(DeploymentInfo deploymentInfo, DeploymentTask deploymentTask, DeploymentContext deploymentContext)
    {
      if (deploymentInfo.TargetEnvironmentName == ProductionEnvironmentName
       && deploymentInfo.ProjectConfigurationName != ProductionProjectConfigurationName)
      {
        throw new InvalidOperationException(string.Format(
          "Can't deploy project ('{0}') with non-production configuration ('{1}') to the production environment!", 
          deploymentInfo.ProjectName, 
          deploymentInfo.ProjectConfigurationName));
      }
    }

    public void OnDeploymentTaskFinished(DeploymentInfo deploymentInfo, DeploymentTask deploymentTask, DeploymentContext deploymentContext)
    {
      // do nothing
    }

    #endregion
  }
}
