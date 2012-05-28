using System;

namespace UberDeployer.Core.Deployment
{
  public class DiagnosticMessageEventArgs : EventArgs
  {
    public DiagnosticMessageEventArgs(string message, MessageType messageType)
    {
      if (string.IsNullOrEmpty(message))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "message");
      }

      Message = message;
      MessageType = messageType;
    }

    public string Message { get; private set; }

    public MessageType MessageType { get; private set; }
  }
}
