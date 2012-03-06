using System;

namespace ProjectDepsVisualizer.Core
{
  public class LogMessageEventArgs : EventArgs
  {
    public LogMessageEventArgs(string message)
    {
      if (message == null) throw new ArgumentNullException("message");

      Message = message;
    }

    public string Message { get; private set; }
  }
}
