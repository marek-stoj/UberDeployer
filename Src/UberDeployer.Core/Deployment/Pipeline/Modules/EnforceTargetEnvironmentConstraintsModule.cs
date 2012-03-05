using System;

namespace UberDeployer.Core.Deployment.Pipeline.Modules
{
  // TODO IMM HI: KRD-specific; move somewhere else
  public class EnforceTargetEnvironmentConstraintsModule : IDeploymentPipelineModule
  {
    public const string ProductionEnvironmentName = "prod";
    public const string ProductionProjectConfigurationName = "Production";

    #region IDeploymentPipelineModule Members

    public void OnDeploymentTaskStarting(DeploymentTask deploymentTask)
    {
      if (deploymentTask.TargetEnvironmentName == ProductionEnvironmentName
       && deploymentTask.ProjectConfigurationName != ProductionProjectConfigurationName)
      {
        throw new InvalidOperationException(string.Format("Can't deploy project ('{0}') with non-production configuration ('{1}') to the production environment!", deploymentTask.ProjectName, deploymentTask.ProjectConfigurationName));
      }
    }

    public void OnDeploymentTaskFinished(DeploymentTask deploymentTask, DateTime dateRequested, string projectName, string targetEnvironmentName, bool finishedSuccessfully)
    {
      // do nothing
    }

    #endregion
  }
}
