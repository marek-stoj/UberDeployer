using System;
using System.Collections.Generic;
using System.Linq;
using UberDeployer.Core.Deployment;
using UberDeployer.Common;

namespace UberDeployer.Agent.Service.Diagnostics
{
  public class InMemoryDiagnosticMessagesLogger : IDiagnosticMessagesLogger
  {
    private readonly Dictionary<Guid, List<DiagnosticMessage>> _diagnosticMessagesByClientId;
    
    private long _prevMessageId;

    private static InMemoryDiagnosticMessagesLogger _instance;
    private static readonly object _mutex = new object();

    #region Constructor(s)

    private InMemoryDiagnosticMessagesLogger()
    {
      _diagnosticMessagesByClientId = new Dictionary<Guid, List<DiagnosticMessage>>();
    }

    #endregion

    #region IDiagnosticMessagesLogger Members

    public void LogMessage(Guid uniqueClientId, DiagnosticMessageType messageType, string message)
    {
      if (uniqueClientId == Guid.Empty)
      {
        throw new ArgumentException("Argument can't be Guid.Empty.", "uniqueClientId");
      }

      if (string.IsNullOrEmpty(message))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "message");
      }

      lock (_mutex)
      {
        List<DiagnosticMessage> diagnosticMessages;

        if (!_diagnosticMessagesByClientId.TryGetValue(uniqueClientId, out diagnosticMessages))
        {
          diagnosticMessages = new List<DiagnosticMessage>();
          _diagnosticMessagesByClientId.Add(uniqueClientId, diagnosticMessages);
        }

        long messageId = ++_prevMessageId;

        diagnosticMessages.Add(
          new DiagnosticMessage(
            messageId,
            DateTime.UtcNow,
            messageType,
            message));
      }
    }

    public IEnumerable<DiagnosticMessage> GetMessages(Guid uniqueClientId, long lastSeenMaxMessageId)
    {
      if (uniqueClientId == Guid.Empty)
      {
        throw new ArgumentException("Argument can't be Guid.Empty.", "uniqueClientId");
      }

      lock (_mutex)
      {
        if (lastSeenMaxMessageId > _prevMessageId)
        {
          lastSeenMaxMessageId = -1;
        }

        List<DiagnosticMessage> messagesToReturn;
        List<DiagnosticMessage> messages;

        if (!_diagnosticMessagesByClientId.TryGetValue(uniqueClientId, out messages)
         || messages.Count == 0)
        {
          messagesToReturn = Enumerable.Empty<DiagnosticMessage>().ToList();
        }
        else
        {
          // take all messages such that dm.MessageId > lastSeenMaxMessageId
          // we're using binary search to find the index of a message with messageId = lastSeenMaxMessageId

          int indexOfLastSeenMaxMessageId =
            messages.BinarySearch(dm => Comparer<long>.Default.Compare(dm.MessageId, lastSeenMaxMessageId));

          int indexToTakeFrom =
            indexOfLastSeenMaxMessageId >= 0
              ? indexOfLastSeenMaxMessageId + 1
              : ~indexOfLastSeenMaxMessageId;

          messagesToReturn =
            messages.Skip(indexToTakeFrom)
              .ToList();
        }

        return messagesToReturn;
      }
    }

    #endregion

    #region Properties

    public static InMemoryDiagnosticMessagesLogger Instance
    {
      get
      {
        lock (_mutex)
        {
          return _instance ?? (_instance = new InMemoryDiagnosticMessagesLogger());
        }
      }
    }

    #endregion
  }
}
