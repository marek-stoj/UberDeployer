using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace UberDeployer.Agent.Proxy
{
  public abstract class WcfProxy<T>
    where T : class
  {
    #region Nested types

    private class Proxy : ClientBase<T>, IDisposable
    {
      public Proxy()
      {
      }

      public Proxy(string endpointConfigurationName)
        : base(endpointConfigurationName)
      {
      }

      public T GetChannel()
      {
        return Channel;
      }

      public void Dispose()
      {
        if (State == CommunicationState.Faulted)
        {
          Abort();
        }
        else
        {
          Close();
        }
      }
    }

    #endregion

    private readonly List<IEndpointBehavior> _endpointBehaviors = new List<IEndpointBehavior>();
    private readonly Func<T> _getCustomInstance;
    private readonly string _endpointConfigurationName;

    #region Constructor(s)

    protected WcfProxy()
    {
    }

    protected WcfProxy(string endpointConfigurationName)
      : this()
    {
      _endpointConfigurationName = endpointConfigurationName;
    }

    protected WcfProxy(Func<T> getCustomInstance)
      : this()
    {
      _getCustomInstance = getCustomInstance;
    }

    #endregion

    #region Public methods

    public void AddBehavior(IEndpointBehavior endpointBehavior)
    {
      _endpointBehaviors.Add(endpointBehavior);
    }

    #endregion

    #region IDiposable members

    public void Dispose()
    {
      // zgodnosc ze stara wersja
    }

    #endregion

    #region Protected methods

    protected virtual TResult Exec<TResult>(Expression<Func<T, TResult>> func)
    {
      return DoExec(func.Compile());
    }

    protected virtual void Exec(Expression<Action<T>> func)
    {
      DoExec(func.Compile());
    }

    #endregion

    #region Private methods

    private TResult DoExec<TResult>(Func<T, TResult> func)
    {
      if (_getCustomInstance != null && _getCustomInstance() != null)
      {
        return func(_getCustomInstance());
      }

      Proxy wcfProxy =
        string.IsNullOrEmpty(_endpointConfigurationName)
          ? new Proxy()
          : new Proxy(_endpointConfigurationName);

      using (wcfProxy)
      {
        foreach (IEndpointBehavior endpointBehavior in _endpointBehaviors)
        {
          wcfProxy.ChannelFactory.Endpoint.Behaviors.Add(endpointBehavior);
        }

        try
        {
          return func(wcfProxy.GetChannel());
        }
        catch (Exception)
        {
          wcfProxy.Abort();

          throw;
        }
      }
    }

    private void DoExec(Action<T> func)
    {
      if (_getCustomInstance != null && _getCustomInstance() != null)
      {
        func(_getCustomInstance());
        return;
      }

      Proxy wcfProxy =
        string.IsNullOrEmpty(_endpointConfigurationName)
          ? new Proxy()
          : new Proxy(_endpointConfigurationName);

      using (wcfProxy)
      {
        foreach (IEndpointBehavior endpointBehavior in _endpointBehaviors)
        {
          wcfProxy.ChannelFactory.Endpoint.Behaviors.Add(endpointBehavior);
        }

        try
        {
          func(wcfProxy.GetChannel());
        }
        catch (Exception)
        {
          wcfProxy.Abort();

          throw;
        }
      }
    }

    #endregion
  }
}
