using System;

namespace UberDeployer.Agent.Proxy.Dto
{
  public class DiagnosticMessage
  {
    public long MessageId { get; set; }

    public DateTime DateUtf { get; set; }

    public string Message { get; set; }

    public DiagnosticMessageType Type { get; set; }
  }
}
