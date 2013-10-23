using System;
using System.Collections.Generic;
using System.Threading;
using UberDeployer.Common.SyntaxSugar;
using System.Linq;

namespace UberDeployer.Common
{
  public static class RetryUtils
  {
    /// <param name="exceptionTypes"></param>
    /// <param name="retriesCount">How many retries. The action will be invoked at most (1 + retriesCount) times.</param>
    /// <param name="retryDelay"></param>
    /// <param name="action"></param>
    public static void RetryOnException(Type[] exceptionTypes, int retriesCount, int retryDelay, Action action)
    {
      Guard.NotNull(exceptionTypes, "exceptionTypes");

      if (exceptionTypes.Length == 0)
      {
        throw new ArgumentException("At least one exception type must be specified.", "exceptionTypes");
      }

      if (retriesCount < 0)
      {
        throw new ArgumentOutOfRangeException("retriesCount", "Argument must not be smaller than 0.");
      }

      if (retryDelay < 0)
      {
        throw new ArgumentOutOfRangeException("retryDelay", "Argument must not be smaller than 0.");
      }

      Guard.NotNull(action, "action");

      int currentRetry = 0;

      while (true)
      {
        try
        {
          action();

          break;
        }
        catch (Exception exc)
        {
          if (!ShouldRetryOnException(exc, exceptionTypes))
          {
            throw;
          }

          currentRetry++;

          if (currentRetry > retriesCount)
          {
            break;
          }

          if (retryDelay > 0)
          {
            Thread.Sleep(retryDelay);
          }
        }
      }
    }

    private static bool ShouldRetryOnException(Exception exception, IEnumerable<Type> exceptionTypes)
    {
      Type exceptionType = exception.GetType();

      return exceptionTypes.Any(et => et.IsAssignableFrom(exceptionType));
    }
  }
}
