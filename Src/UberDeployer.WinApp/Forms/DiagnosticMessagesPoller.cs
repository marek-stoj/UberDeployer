using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using UberDeployer.Agent.Proxy;
using UberDeployer.Agent.Proxy.Dto;

namespace UberDeployer.WinApp.Forms
{
  public class DiagnosticMessagesPoller
  {
    private const int _PollInterval = 250;

    private static long _lastSeenMaxDiagnosticMessageId = -1;

    private readonly IAgentService _agentService;
    private readonly Action<DiagnosticMessage> _action;

    private Thread _thread;
    private bool _stopRequested;

    private static readonly object _mutex = new object();

    public DiagnosticMessagesPoller(IAgentService agentService, Action<DiagnosticMessage> action)
    {
      if (agentService == null)
      {
        throw new ArgumentNullException("agentService");
      }

      if (action == null)
      {
        throw new ArgumentNullException("action");
      }

      _agentService = agentService;
      _action = action;
    }

    public void Start()
    {
      if (_thread != null)
      {
        throw new InvalidOperationException("Already started.");
      }

      _stopRequested = false;

      _thread = new Thread(Run);
      _thread.Start();
    }

    public void Stop()
    {
      if (_thread == null)
      {
        return;
      }

      _stopRequested = true;

      _thread.Join();
      _thread = null;
    }

    private void Run()
    {
      try
      {
        while (!_stopRequested)
        {
          try
          {
            Check();
          }
          catch (Exception exc)
          {
            // not much we can do here - let's keep going unless it's a critical exception
            if (exc is ThreadAbortException || exc is OutOfMemoryException || exc is StackOverflowException)
            {
              throw;
            }
          }
          finally
          {
            Thread.Sleep(_PollInterval);
          }
        }

        Check();
      }
      catch (Exception exc)
      {
        // not much we can do here - let's swallow the exception (because we're on an another thread) unless it's a critical exception
        if (exc is ThreadAbortException || exc is OutOfMemoryException || exc is StackOverflowException)
        {
          throw;
        }
      }
    }

    private void Check()
    {
      List<DiagnosticMessage> diagnosticMessages;

      lock (_mutex)
      {
        diagnosticMessages =
          _agentService.GetDiagnosticMessages(
            Program.UniqueClientId,
            _lastSeenMaxDiagnosticMessageId);

        if (diagnosticMessages.Count > 0)
        {
          _lastSeenMaxDiagnosticMessageId = diagnosticMessages.Select(dm => dm.MessageId).Max();
        }
      }

      foreach (DiagnosticMessage diagnosticMessage in diagnosticMessages)
      {
        _action(diagnosticMessage);
      }
    }
  }
}
