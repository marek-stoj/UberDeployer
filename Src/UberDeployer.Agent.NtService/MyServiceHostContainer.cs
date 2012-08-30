using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using UberDeployer.Common;
using log4net;

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

    protected abstract IEnumerable<Type> ServiceTypes { get; }

    protected IEnumerable<MyServiceHost> ServiceHosts
    {
      get { return _serviceHosts.AsReadOnly(); }
    }

    public void Start()
    {
      try
      {
        CreateServiceHosts();
        StartServiceHosts();

        _log.InfoIfEnabled(() => "NT service has started.");
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

        _log.InfoIfEnabled(() => "NT service has stopped.");
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
