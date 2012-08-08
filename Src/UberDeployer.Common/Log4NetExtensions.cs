using System;
using log4net;

namespace UberDeployer.Common
{
  public static class Log4NetExtensions
  {
    public static void DebugIfEnabled(this ILog log, Func<object> messageFunc)
    {
      if (log.IsDebugEnabled)
      {
        log.Debug(messageFunc());
      }
    }

    public static void InfoIfEnabled(this ILog log, Func<object> messageFunc)
    {
      if (log.IsInfoEnabled)
      {
        log.Info(messageFunc());
      }
    }

    public static void WarnIfEnabled(this ILog log, Func<object> messageFunc, Exception exception = null)
    {
      if (log.IsWarnEnabled)
      {
        log.Warn(messageFunc(), exception);
      }
    }

    public static void ErrorIfEnabled(this ILog log, Func<object> messageFunc, Exception exception = null)
    {
      if (log.IsErrorEnabled)
      {
        log.Error(messageFunc(), exception);
      }
    }

    public static void FatalIfEnabled(this ILog log, Func<object> messageFunc, Exception exception = null)
    {
      if (log.IsFatalEnabled)
      {
        log.Fatal(messageFunc(), exception);
      }
    }
  }
}
