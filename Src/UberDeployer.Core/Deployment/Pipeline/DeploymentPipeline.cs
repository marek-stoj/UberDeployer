using System;
using System.Collections.Generic;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment.Pipeline
{
  public class DeploymentPipeline : IDeploymentPipeline
  {
    private readonly List<IDeploymentPipelineModule> _modules;

    public DeploymentPipeline()
    {
      _modules = new List<IDeploymentPipelineModule>();
    }

    public void AddModule(IDeploymentPipelineModule module)
    {
      if (module == null)
      {
        throw new ArgumentNullException("module");
      }

      _modules.Add(module);
    }

    public void StartDeployment(DeploymentInfo deploymentInfo, DeploymentTask deploymentTask, DeploymentContext deploymentContext)
    {
      Guard.NotNull(deploymentTask, "deploymentTask");
      Guard.NotNull(deploymentContext, "deploymentContext");

      deploymentContext.DateStarted = DateTime.UtcNow;

      bool finishedSuccessfully = false;

      OnDeploymentTaskStarting(deploymentTask, deploymentContext);

      try
      {
        deploymentTask.PrepareAndExecute(deploymentInfo);

        finishedSuccessfully = true;
      }
      finally
      {
        deploymentContext.DateFinished = DateTime.UtcNow;
        deploymentContext.FinishedSuccessfully = finishedSuccessfully;

        // TODO IMM HI: catch exceptions; pass them upstream using some mechanisms like DeploymentTask.DiagnosticMessagePosted event
        OnDeploymentTaskFinished(
          deploymentTask,
          deploymentContext);
      }
    }

    private void OnDeploymentTaskStarting(DeploymentTask deploymentTask, DeploymentContext deploymentContext)
    {
      foreach (IDeploymentPipelineModule deploymentPipelineModule in _modules)
      {
        deploymentPipelineModule.OnDeploymentTaskStarting(deploymentTask, deploymentContext);
      }
    }

    private void OnDeploymentTaskFinished(DeploymentTask deploymentTask, DeploymentContext deploymentContext)
    {
      foreach (IDeploymentPipelineModule deploymentPipelineModule in _modules)
      {
        deploymentPipelineModule.OnDeploymentTaskFinished(deploymentTask, deploymentContext);
      }
    }
  }
}
