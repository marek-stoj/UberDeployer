using System;
using System.Collections.Generic;

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

    public void StartDeployment(DeploymentTask deploymentTask)
    {
      if (deploymentTask == null)
      {
        throw new ArgumentNullException("deploymentTask");
      }

      bool finishedSuccessfully = false;
      DateTime utcNow = DateTime.UtcNow;

      OnDeploymentTaskStarting(deploymentTask);

      try
      {
        deploymentTask.PrepareAndExecute();

        finishedSuccessfully = true;
      }
      finally
      {
        OnDeploymentTaskFinished(
          deploymentTask,
          utcNow,
          deploymentTask.ProjectName,
          deploymentTask.TargetEnvironmentName,
          finishedSuccessfully);
      }
    }

    private void OnDeploymentTaskStarting(DeploymentTask deploymentTask)
    {
      foreach (IDeploymentPipelineModule deploymentPipelineModule in _modules)
      {
        deploymentPipelineModule.OnDeploymentTaskStarting(deploymentTask);
      }
    }

    private void OnDeploymentTaskFinished(DeploymentTask deploymentTask, DateTime dateRequested, string projectName, string targetEnvironmentName, bool finishedSuccessfully)
    {
      foreach (IDeploymentPipelineModule deploymentPipelineModule in _modules)
      {
        deploymentPipelineModule.OnDeploymentTaskFinished(deploymentTask, dateRequested, projectName, targetEnvironmentName, finishedSuccessfully);
      }
    }
  }
}
