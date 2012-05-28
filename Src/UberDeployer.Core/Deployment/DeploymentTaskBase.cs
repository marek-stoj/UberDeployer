using System;

namespace UberDeployer.Core.Deployment
{
  public abstract class DeploymentTaskBase
  {
    public event EventHandler<DiagnosticMessageEventArgs> DiagnosticMessagePosted;

    private bool _prepared;

    public void Prepare()
    {
      if (_prepared)
      {
        throw new InvalidOperationException("The task has already been prepared.");
      }

      PostDiagnosticMessage(string.Format("Preparing: {0}", Description));

      DoPrepare();

      _prepared = true;
    }

    public void Execute()
    {
      if (!_prepared)
      {
        throw new InvalidOperationException("The task has to be prepared before it can be executed.");
      }

      PostDiagnosticMessage(string.Format("Executing: {0}", Description));

      DoExecute();
    }

    public void PrepareAndExecute()
    {
      Prepare();
      Execute();
    }

    public abstract string Description { get; }

    protected abstract void DoExecute();

    protected abstract void DoPrepare();

    protected void PostDiagnosticMessage(string message)
    {
      PostDiagnosticMessage(message, MessageType.Info);
    }

    protected void PostDiagnosticMessage(string message, MessageType messageType)
    {
      if (string.IsNullOrEmpty(message))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "message");
      }

      OnDiagnosticMessagePosted(this, new DiagnosticMessageEventArgs(message, messageType));
    }

    protected void OnDiagnosticMessagePosted(object sender, DiagnosticMessageEventArgs diagnosticMessageEventArgs)
    {
      var eventHandler = DiagnosticMessagePosted;

      if (eventHandler != null)
      {
        eventHandler(sender, diagnosticMessageEventArgs);
      }
    }
  }
}
