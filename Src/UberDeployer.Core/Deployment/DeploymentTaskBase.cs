using System;

namespace UberDeployer.Core.Deployment
{
  public abstract class DeploymentTaskBase
  {
    public event EventHandler<DiagnosticMessageEventArgs> DiagnosticMessagePosted;

    #region Public methods

    public void PrepareAndExecute()
    {
      Prepare();
      Execute();
    }

    public void Prepare()
    {
      if (IsPrepared)
      {
        throw new InvalidOperationException("The task has already been prepared.");
      }

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

      DoExecute();
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

    #endregion
  }
}
