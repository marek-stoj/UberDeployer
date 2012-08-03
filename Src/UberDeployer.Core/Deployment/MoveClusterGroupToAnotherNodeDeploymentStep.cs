using System;
using UberDeployer.Core.Management.FailoverCluster;

namespace UberDeployer.Core.Deployment
{
  public class MoveClusterGroupToAnotherNodeDeploymentStep : DeploymentStep
  {
    private readonly IFailoverClusterManager _failoverClusterManager;
    private readonly string _failoverClusterMachineName;
    private readonly string _clusterGroupName;
    private readonly string _targetNodeName;

    #region Constructor(s)

    public MoveClusterGroupToAnotherNodeDeploymentStep(IFailoverClusterManager failoverClusterManager, string failoverClusterMachineName, string clusterGroupName, string targetNodeName)
    {
      if (failoverClusterManager == null)
      {
        throw new ArgumentNullException("failoverClusterManager");
      }

      if (string.IsNullOrEmpty(failoverClusterMachineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "failoverClusterMachineName");
      }

      if (string.IsNullOrEmpty(clusterGroupName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "clusterGroupName");
      }

      if (string.IsNullOrEmpty(targetNodeName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "targetNodeName");
      }

      _failoverClusterManager = failoverClusterManager;
      _failoverClusterMachineName = failoverClusterMachineName;
      _clusterGroupName = clusterGroupName;
      _targetNodeName = targetNodeName;
    }

    #endregion

    #region Overrides of DeploymentStep

    protected override void DoExecute()
    {
      _failoverClusterManager
        .MoveToAnotherNode(
          _failoverClusterMachineName,
          _clusterGroupName,
          _targetNodeName);
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Move cluster group '{0}' to node '{1}' in cluster '{2}'.",
            _clusterGroupName,
            _targetNodeName,
            _failoverClusterMachineName);
      }
    }

    #endregion
  }
}
