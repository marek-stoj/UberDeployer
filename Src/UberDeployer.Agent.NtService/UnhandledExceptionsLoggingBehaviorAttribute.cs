using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using UberDeployer.Common;
using log4net;

namespace UberDeployer.Agent.NtService
{
  public class UnhandledExceptionsLoggingBehaviorAttribute : Attribute, IServiceBehavior
  {
    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
    {
      // do nothing
    }

    public void AddBindingParameters(
      ServiceDescription serviceDescription,
      ServiceHostBase serviceHostBase,
      Collection<ServiceEndpoint> endpoints,
      BindingParameterCollection bindingParameters)
    {
      // do nothing
    }

    public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
    {
      IErrorHandler errorHandler = null;

      try
      {
        errorHandler = new UnhandledExceptionsLoggingErrorHandler();
      }
      catch (MissingMethodException e)
      {
        throw new ArgumentException(
          "The errorHandlerType specified in the ErrorHandlerBehaviorAttribute constructor must have a public empty constructor.",
          e);
      }
      catch (InvalidCastException e)
      {
        throw new ArgumentException(
          "The errorHandlerType specified in the ErrorHandlerBehaviorAttribute constructor must implement System.ServiceModel.Dispatcher.IErrorHandler.",
          e);
      }
      catch (Exception exc)
      {
        _log.ErrorIfEnabled(() => "Unhandled exception while applying dispatch behavior.", exc);
      }

      foreach (ChannelDispatcher channelDispatcher in serviceHostBase.ChannelDispatchers.OfType<ChannelDispatcher>())
      {
        channelDispatcher.ErrorHandlers.Add(errorHandler);
      }
    }
  }
}
