using System;

namespace UberDeployer.Core.Deployment
{
  public class DiagnosticMessageEventArgs : EventArgs
  {
    public DiagnosticMessageEventArgs(string message)
    {
      if (string.IsNullOrEmpty(message))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "message");
      }

      Message = message;
    }

    public string Message { get; private set; }
  }
}
