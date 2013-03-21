using System;
using System.Collections.Generic;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment.Pipeline
{
  public class DeploymentPipeline : IDeploymentPipeline
  {
    public event EventHandler<DiagnosticMessageEventArgs> DiagnosticMessagePosted;

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

    // TODO IMM HI: xxx somehow mark simulation in history
    public void StartDeployment(DeploymentInfo deploymentInfo, DeploymentTask deploymentTask, DeploymentContext deploymentContext)
    {
      Guard.NotNull(deploymentInfo, "deploymentInfo");
      Guard.NotNull(deploymentTask, "deploymentTask");
      Guard.NotNull(deploymentContext, "deploymentContext");

      PostDiagnosticMessage(string.Format("Starting{0} '{1}'.", (deploymentInfo.IsSimulation ? " (simulation)" : ""), deploymentTask.GetType().Name), DiagnosticMessageType.Info);

      deploymentContext.DateStarted = DateTime.UtcNow;

      deploymentTask.DiagnosticMessagePosted += OnDeploymentTaskDiagnosticMessagePosted;

      bool finishedSuccessfully = false;

      OnDeploymentTaskStarting(deploymentInfo, deploymentTask, deploymentContext);

      try
      {
        deploymentTask.PrepareAndExecute(deploymentInfo);

        finishedSuccessfully = true;

        PostDiagnosticMessage(string.Format("Finished{0} '{1}' (\"{2}\").", (deploymentInfo.IsSimulation ? " (simulation)" : ""), deploymentTask.GetType().Name, deploymentTask.Description), DiagnosticMessageType.Info);
        PostDiagnosticMessage("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -", DiagnosticMessageType.Info);
      }
      finally
      {
        deploymentContext.DateFinished = DateTime.UtcNow;
        deploymentContext.FinishedSuccessfully = finishedSuccessfully;

        // TODO IMM HI: catch exceptions; pass them upstream using some mechanisms like DeploymentTask.DiagnosticMessagePosted event
        OnDeploymentTaskFinished(deploymentInfo, deploymentTask, deploymentContext);

        // TODO IMM HI: xxx
        deploymentTask.DiagnosticMessagePosted -= OnDeploymentTaskDiagnosticMessagePosted;
      }
    }

    protected void PostDiagnosticMessage(string message, DiagnosticMessageType diagnosticMessageType)
    {
      if (string.IsNullOrEmpty(message))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "message");
      }

      OnDiagnosticMessagePosted(this, new DiagnosticMessageEventArgs(diagnosticMessageType, message));
    }

    protected void OnDiagnosticMessagePosted(object sender, DiagnosticMessageEventArgs diagnosticMessageEventArgs)
    {
      var eventHandler = DiagnosticMessagePosted;

      if (eventHandler != null)
      {
        eventHandler(sender, diagnosticMessageEventArgs);
      }
    }

    private void OnDeploymentTaskStarting(DeploymentInfo deploymentInfo, DeploymentTask deploymentTask, DeploymentContext deploymentContext)
    {
      foreach (IDeploymentPipelineModule deploymentPipelineModule in _modules)
      {
        deploymentPipelineModule.OnDeploymentTaskStarting(deploymentInfo, deploymentTask, deploymentContext);
      }
    }

    private void OnDeploymentTaskFinished(DeploymentInfo deploymentInfo, DeploymentTask deploymentTask, DeploymentContext deploymentContext)
    {
      foreach (IDeploymentPipelineModule deploymentPipelineModule in _modules)
      {
        deploymentPipelineModule.OnDeploymentTaskFinished(deploymentInfo, deploymentTask, deploymentContext);
      }
    }

    private void OnDeploymentTaskDiagnosticMessagePosted(object sender, DiagnosticMessageEventArgs e)
    {
      OnDiagnosticMessagePosted(this, e);
    }
  }
}
