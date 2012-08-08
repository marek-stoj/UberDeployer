using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using UberDeployer.Common;
using log4net;

namespace UberDeployer.Agent.NtService
{
  public class UnhandledExceptionsLoggingErrorHandler : IErrorHandler
  {
    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
    {
    }

    public bool HandleError(Exception error)
    {
      if (error == null)
      {
        return false;
      }

      bool logError = false;

      if (!(error is FaultException))
      {
        logError = true;
      }
      else
      {
        Type errorType = error.GetType();

        if (!errorType.IsGenericType)
        {
          logError = true;
        }
        else
        {
          Type[] faultGenericArguments = errorType.GetGenericArguments();

          if (faultGenericArguments != null && faultGenericArguments.Length > 0)
          {
            Type faultType = faultGenericArguments[0];
            string faultNamespace = faultType.Namespace ?? string.Empty;

            if (faultNamespace.StartsWith("System."))
            {
              logError = true;
            }
          }
        }
      }

      if (logError)
      {
        _log.ErrorIfEnabled(() => "Unhandled exception.", error);
      }

      return false;
    }
  }
}
