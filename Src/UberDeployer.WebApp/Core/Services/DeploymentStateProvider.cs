using System;
using System.Collections.Generic;
using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.WebApp.Core.Services
{
  public class DeploymentStateProvider : IDeploymentStateProvider
  {
    private static readonly Dictionary<Guid, DeploymentState> _deploymentStateByIdDict = new Dictionary<Guid, DeploymentState>();
    private static readonly Dictionary<string, List<Guid>> _deploymentIdsByUserIdentityDict = new Dictionary<string, List<Guid>>();

    public void SetDeploymentState(Guid deploymentId, DeploymentState deploymentState)
    {
      Guard.NotEmpty(deploymentId, "deploymentId");
      Guard.NotNull(deploymentState, "deploymentState");

      _deploymentStateByIdDict[deploymentId] = deploymentState;

      List<Guid> deploymentIdsForUserIdentity;

      if (!_deploymentIdsByUserIdentityDict.TryGetValue(deploymentState.UserIdentity, out deploymentIdsForUserIdentity))
      {
        deploymentIdsForUserIdentity = new List<Guid>();
      }

      deploymentIdsForUserIdentity.Add(deploymentId);
    }

    public DeploymentState FindDeploymentState(Guid deploymentId)
    {
      DeploymentState deploymentState;

      if (_deploymentStateByIdDict.TryGetValue(deploymentId, out deploymentState))
      {
        return deploymentState;
      }

      return null;
    }

    public void RemoveAllDeploymentStates(string userIdentity)
    {
      Guard.NotNullNorEmpty(userIdentity, "userIdentity");

      List<Guid> deploymentIdsForUserIdentity;

      if (!_deploymentIdsByUserIdentityDict.TryGetValue(userIdentity, out deploymentIdsForUserIdentity))
      {
        return;
      }

      foreach (Guid deploymentId in deploymentIdsForUserIdentity)
      {
        _deploymentStateByIdDict.Remove(deploymentId);
      }

      _deploymentIdsByUserIdentityDict.Remove(userIdentity);
    }
  }
}
