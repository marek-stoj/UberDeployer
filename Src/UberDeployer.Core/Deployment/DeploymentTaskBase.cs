using System;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  public abstract class DeploymentTaskBase
  {
    public event EventHandler<DiagnosticMessageEventArgs> DiagnosticMessagePosted;

    private DeploymentInfo _deploymentInfo;

    #region Public methods

    public void Prepare(DeploymentInfo deploymentInfo)
    {
      Guard.NotNull(deploymentInfo, "DeploymentInfo");

      if (IsPrepared)
      {
        throw new InvalidOperationException("The task has already been prepared.");
      }

      DeploymentInfo = deploymentInfo;

      PostDiagnosticMessage(string.Format("Preparing: {0}", Description), DiagnosticMessageType.Trace);

      DoPrepare();

      IsPrepared = true;
    }

    public void Execute()
    {
      if (!IsPrepared)
      {
        throw new InvalidOperationException("The task has to be prepared before it can be executed.");
      }

      if (!DeploymentInfo.IsSimulation)
      {
        PostDiagnosticMessage(string.Format("Executing: {0}", Description), DiagnosticMessageType.Info);
      }

      DoExecute();
    }

    public void PrepareAndExecute(DeploymentInfo deploymentInfo)
    {
      Prepare(deploymentInfo);
      Execute();
    }

    public abstract string Description { get; }

    #endregion

    #region Protected members

    protected abstract void DoExecute();

    protected abstract void DoPrepare();

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

    protected bool IsPrepared { get; private set; }

    // TODO IMM HI: xxx move down?
    protected DeploymentInfo DeploymentInfo
    {
      get
      {
        if (_deploymentInfo == null)
        {
          throw new InvalidOperationException("The task hasn't been prepared.");
        }

        return _deploymentInfo;
      }

      private set { _deploymentInfo = value; }
    }

    #endregion
  }
}
