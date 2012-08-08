using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using UberDeployer.Common;
using log4net;
using log4net.Config;

namespace UberDeployer.Agent.NtService
{
  public abstract class MyServiceHostContainer
  {
    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private readonly List<MyServiceHost> _serviceHosts;

    protected MyServiceHostContainer()
    {
      _serviceHosts = new List<MyServiceHost>();
    }

    protected abstract string ApplicationName { get; }

    protected abstract IEnumerable<Type> ServiceTypes { get; }

    protected IEnumerable<MyServiceHost> ServiceHosts
    {
      get { return _serviceHosts.AsReadOnly(); }
    }

    public void Start()
    {
      try
      {
        GlobalContext.Properties["applicationName"] = ApplicationName;
        XmlConfigurator.Configure();

        CreateServiceHosts();
        StartServiceHosts();

        _log.InfoIfEnabled(() => string.Format("{0} service started.", ApplicationName));
      }
      catch (Exception exc)
      {
        _log.ErrorIfEnabled(() => "Error while starting.", exc);

        Stop();

        throw;
      }
    }

    public void Stop()
    {
      try
      {
        StopServiceHosts();

        _log.InfoIfEnabled(() => string.Format("{0} service stopped.", ApplicationName));
      }
      catch (Exception exc)
      {
        _log.ErrorIfEnabled(() => "Error occured while stopping.", exc);

        throw;
      }
    }

    protected virtual void OnServiceHostsStarting()
    {
      // do nothing
    }

    protected virtual void OnServiceHostsStarted()
    {
      // do nothing
    }

    protected virtual void OnServiceHostsStopping()
    {
      // do nothing
    }

    protected virtual void OnServiceHostsStopped()
    {
      // do nothing
    }

    private static void StopServiceHost(ServiceHost servicehost)
    {
      if (servicehost.State == CommunicationState.Opened)
      {
        servicehost.Close();
      }
      else
      {
        servicehost.Abort();
      }
    }

    private void CreateServiceHosts()
    {
      _serviceHosts.Clear();

      IEnumerable<Type> serviceHostTypes = ServiceTypes;

      if (serviceHostTypes == null)
      {
        throw new InvalidOperationException("ServiceTypes can't be null.");
      }

      foreach (Type serviceHostType in serviceHostTypes)
      {
        var serviceHost = new MyServiceHost(serviceHostType);

        _serviceHosts.Add(serviceHost);
      }
    }

    private void StartServiceHosts()
    {
      OnServiceHostsStarting();

      _serviceHosts.ForEach(sh => sh.Open());

      OnServiceHostsStarted();
    }

    private void StopServiceHosts()
    {
      OnServiceHostsStopping();

      _serviceHosts.ForEach(sh => StopServiceHost(sh));

      OnServiceHostsStopped();
    }
  }
}