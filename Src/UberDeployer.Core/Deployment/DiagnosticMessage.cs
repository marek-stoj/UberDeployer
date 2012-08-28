using System;

namespace UberDeployer.Core.Deployment
{
  public class DiagnosticMessage
  {
    public DiagnosticMessage(long messageId, DateTime dateUtf, DiagnosticMessageType type, string message)
    {
      if (string.IsNullOrEmpty(message))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "message");
      }

      if (dateUtf.Kind != DateTimeKind.Utc)
      {
        throw new ArgumentException("Argument must be a UTC date.", "dateUtf");
      }

      MessageId = messageId;
      DateUtf = dateUtf;
      Type = type;
      Message = message;
    }

    public long MessageId { get; private set; }

    public DateTime DateUtf { get; private set; }

    public DiagnosticMessageType Type { get; private set; }

    public string Message { get; private set; }
  }
}
