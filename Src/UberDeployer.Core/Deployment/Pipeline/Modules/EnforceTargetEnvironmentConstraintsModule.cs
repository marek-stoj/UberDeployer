﻿using System;

namespace UberDeployer.Core.Deployment.Pipeline.Modules
{
  // TODO IMM HI: KRD-specific; move somewhere else
  public class EnforceTargetEnvironmentConstraintsModule : IDeploymentPipelineModule
  {
    public const string ProductionEnvironmentName = "prod";
    public const string ProductionProjectConfigurationName = "Production";

    #region IDeploymentPipelineModule Members

    public void OnDeploymentTaskStarting(DeploymentTask deploymentTask, DeploymentContext deploymentContext)
    {
      if (deploymentTask.DeploymentInfo.TargetEnvironmentName == ProductionEnvironmentName
       && deploymentTask.DeploymentInfo.ProjectConfigurationName != ProductionProjectConfigurationName)
      {
        throw new InvalidOperationException(string.Format(
          "Can't deploy project ('{0}') with non-production configuration ('{1}') to the production environment!", 
          deploymentTask.DeploymentInfo.ProjectName, 
          deploymentTask.DeploymentInfo.ProjectConfigurationName));
      }
    }

    public void OnDeploymentTaskFinished(DeploymentTask deploymentTask, DeploymentContext deploymentContext)
    {
      // do nothing
    }

    #endregion
  }
}
