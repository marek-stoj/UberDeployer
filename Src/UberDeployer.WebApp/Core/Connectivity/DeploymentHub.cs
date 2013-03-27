using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.WebApp.Core.Connectivity
{
  public class DeploymentHub : Hub
  {
    private static readonly Dictionary<string, string> _connectionIdUserIdentityDict = new Dictionary<string, string>();

    public static void Send(string userIdentity, object message)
    {
      Guard.NotNullNorEmpty(userIdentity, "userIdentity");
      Guard.NotNull(message, "message");

      string connectionId;

      if (!_connectionIdUserIdentityDict.TryGetValue(userIdentity, out connectionId))
      {
        throw new ClientNotConnectedException(userIdentity);
      }

      // TODO IMM HI: xxx remove
      IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<DeploymentHub>();

      dynamic client = hubContext.Clients.Client(connectionId);

      if (client == null)
      {
        throw new ClientNotConnectedException(userIdentity);
      }

      client.send(message);
    }

    public override Task OnConnected()
    {
      _connectionIdUserIdentityDict[UserIdentity] = Context.ConnectionId;

      return base.OnConnected();
    }

    public override Task OnDisconnected()
    {
      _connectionIdUserIdentityDict.Remove(UserIdentity);

      return base.OnDisconnected();
    }

    private static string UserIdentity
    {
      get
      {
        string userIdentity =
          Thread.CurrentPrincipal != null && Thread.CurrentPrincipal.Identity != null
            ? Thread.CurrentPrincipal.Identity.Name
            : null;

        if (string.IsNullOrEmpty(userIdentity))
        {
          throw new InvalidOperationException("No user identity!");
        }

        return userIdentity;
      }
    }
  }
}
