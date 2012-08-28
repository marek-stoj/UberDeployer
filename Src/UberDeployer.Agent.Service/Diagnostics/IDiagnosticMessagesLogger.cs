using System;
using System.Collections.Generic;
using UberDeployer.Core.Deployment;

namespace UberDeployer.Agent.Service.Diagnostics
{
  public interface IDiagnosticMessagesLogger
  {
    void LogMessage(Guid uniqueClientId, DiagnosticMessageType messageType, string message);

    IEnumerable<DiagnosticMessage> GetMessages(Guid uniqueClientId, long lastSeenMaxMessageId);
  }
}
