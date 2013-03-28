using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.WebApp.Core.Services;

namespace UberDeployer.WebApp.Core.Connectivity
{
  public class DeploymentHub : Hub
  {
    private static readonly Dictionary<string, string> _connectionIdUserIdentityDict = new Dictionary<string, string>();

    private readonly IDeploymentStateProvider _deploymentStateProvider;

    public DeploymentHub(IDeploymentStateProvider deploymentStateProvider)
    {
      Guard.NotNull(deploymentStateProvider, "deploymentStateProvider");

      _deploymentStateProvider = deploymentStateProvider;
    }

    public DeploymentHub()
      : this(new DeploymentStateProvider())
    {
    }

    public static void PromptForCredentials(Guid deploymentId, string userIdentity, string projectName, string projectConfigurationName, string targetEnvironmentName, string machineName, string username)
    {
      Guard.NotEmpty(deploymentId, "deploymentId");
      Guard.NotNullNorEmpty(userIdentity, "userIdentity");
      Guard.NotNullNorEmpty(projectName, "projectName");
      Guard.NotNullNorEmpty(projectConfigurationName, "projectConfigurationName");
      Guard.NotNullNorEmpty(targetEnvironmentName, "targetEnvironmentName");
      Guard.NotNullNorEmpty(machineName, "machineName");
      Guard.NotNullNorEmpty(username, "username");

      dynamic client = GetClient(userIdentity);

      if (client == null)
      {
        throw new ClientNotConnectedException(userIdentity);
      }

      client.promptForCredentials(
        new
        {
          deploymentId = deploymentId,
          projectName = projectName,
          projectConfigurationName = projectConfigurationName,
          targetEnvironmentName = targetEnvironmentName,
          machineName,
          username = username,
        });
    }

    public static void CancelPromptForCredentials(string userIdentity)
    {
      Guard.NotNullNorEmpty(userIdentity, "userIdentity");

      dynamic client = GetClient(userIdentity);

      if (client == null)
      {
        throw new ClientNotConnectedException(userIdentity);
      }

      client.cancelPromptForCredentials(new object());
    }

    public override Task OnConnected()
    {
      _connectionIdUserIdentityDict[UserIdentity] = Context.ConnectionId;

      return base.OnConnected();
    }

    public override Task OnDisconnected()
    {
      _connectionIdUserIdentityDict.Remove(UserIdentity);
      _deploymentStateProvider.RemoveAllDeploymentStates(UserIdentity);

      return base.OnDisconnected();
    }

    private static dynamic GetClient(string userIdentity)
    {
      Guard.NotNullNorEmpty(userIdentity, "userIdentity");

      string connectionId;

      if (!_connectionIdUserIdentityDict.TryGetValue(userIdentity, out connectionId))
      {
        throw new ClientNotConnectedException(userIdentity);
      }

      IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<DeploymentHub>();

      return hubContext.Clients.Client(connectionId);
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
