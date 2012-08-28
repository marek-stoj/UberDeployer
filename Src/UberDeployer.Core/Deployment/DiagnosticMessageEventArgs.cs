using System;

namespace UberDeployer.Core.Deployment
{
  public class DiagnosticMessageEventArgs : EventArgs
  {
    public DiagnosticMessageEventArgs(DiagnosticMessageType messageType, string message)
    {
      if (string.IsNullOrEmpty(message))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "message");
      }

      MessageType = messageType;
      Message = message;
    }

    public DiagnosticMessageType MessageType { get; private set; }

    public string Message { get; private set; }
  }
}
