using System;
using System.Collections.Generic;
using System.Linq;
using MS.Internal.ServerClusters;

namespace UberDeployer.Core.Management.FailoverCluster
{
  public class PowerShellFailoverClusterManager : IFailoverClusterManager
  {
    private static readonly Guid _DebugLogProviderId = new Guid("F46E2229-6E96-4AA8-A37C-4227F8A4FF14");
    
    private const ClusterAccessRights _RequiredClusterAccessRights = ClusterAccessRights.GenericAll;

    private static bool _debugLogInitialized;

    private static readonly object _mutex = new object();

    #region IFailoverClusterManager Members

    public IEnumerable<string> GetPossibleNodeNames(string clusterMachineName, string clusterGroupName)
    {
      if (string.IsNullOrEmpty(clusterMachineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "clusterMachineName");
      }

      if (string.IsNullOrEmpty(clusterGroupName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "clusterGroupName");
      }

      Cluster cluster = OpenCluster(clusterMachineName);
      ClusterGroup clusterGroup = GetClusterGroup(cluster, clusterGroupName);
      ClusterResourceCollection clusterGroupResources = clusterGroup.GetResources();

      HashSet<string> possibleNodeNames;

      if (clusterGroupResources == null || clusterGroupResources.Count == 0)
      {
        possibleNodeNames = new HashSet<string>();
      }
      else
      {
        possibleNodeNames =
          new HashSet<string>(
            clusterGroupResources[0]
              .GetPossibleOwnerNodes()
              .Select(n => n.Name));

        foreach (ClusterResource clusterResource in clusterGroupResources.Skip(1))
        {
          possibleNodeNames.IntersectWith(
            clusterResource
              .GetPossibleOwnerNodes()
              .Select(n => n.Name));
        }
      }

      return possibleNodeNames;
    }

    public string GetCurrentNodeName(string clusterMachineName, string clusterGroupName)
    {
      if (string.IsNullOrEmpty(clusterMachineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "clusterMachineName");
      }

      if (string.IsNullOrEmpty(clusterGroupName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "clusterGroupName");
      }

      Cluster cluster = OpenCluster(clusterMachineName);
      ClusterGroup clusterGroup = GetClusterGroup(cluster, clusterGroupName);
      ClusterNode clusterNode = clusterGroup.GetOwnerNode();

      return clusterNode != null ? clusterNode.Name : null;
    }

    public void MoveToAnotherNode(string clusterMachineName, string clusterGroupName, string targetNodeName)
    {
      if (string.IsNullOrEmpty(clusterMachineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "clusterMachineName");
      }

      if (string.IsNullOrEmpty(clusterGroupName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "clusterGroupName");
      }

      if (string.IsNullOrEmpty(targetNodeName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "targetNodeName");
      }

      Cluster cluster = OpenCluster(clusterMachineName);
      ClusterGroup clusterGroup = GetClusterGroup(cluster, clusterGroupName);
      ClusterNode targetNode = GetNode(targetNodeName, cluster);

      clusterGroup.Move(targetNode);
    }

    #endregion

    #region Private methods

    private static void EnsureDebugLogIsInitialized()
    {
      lock (_mutex)
      {
        if (_debugLogInitialized)
        {
          return;
        }

        DebugLog.StartInitialize(_DebugLogProviderId);

        _debugLogInitialized = true;
      }
    }

    private static Cluster OpenCluster(string clusterMachineName)
    {
      EnsureDebugLogIsInitialized();

      ClusterAccessRights grantedAccess = ClusterAccessRights.None;
      Cluster cluster = Cluster.Open(clusterMachineName, ClusterAccessRights.MaximumAllowed, ref grantedAccess);

      if (cluster == null)
      {
        throw new ArgumentException(string.Format("Couldn't open cluster '{0}'.", clusterMachineName), "clusterMachineName");
      }

      if (grantedAccess != _RequiredClusterAccessRights)
      {
        throw new InvalidOperationException(string.Format("Access granted to cluster '{0}' is too low. Got '{1}' but '{2}' is required.", clusterMachineName, grantedAccess, _RequiredClusterAccessRights));
      }

      return cluster;
    }

    private static ClusterGroup GetClusterGroup(Cluster cluster, string clusterGroupName)
    {
      ClusterGroup clusterGroup;

      try
      {
        clusterGroup = cluster.GetGroup(clusterGroupName);
      }
      catch (ClusterObjectDeletedException)
      {
        clusterGroup = null;
      }

      if (clusterGroup == null)
      {
        throw new InvalidOperationException(string.Format("Couldn't retrieve cluster group '{0}' from cluster '{1}'. Does the cluster group exist?", clusterGroupName, cluster.Name));
      }

      return clusterGroup;
    }

    private static ClusterNode GetNode(string targetNodeName, Cluster cluster)
    {
      ClusterNode node;

      try
      {
        node = cluster.GetNode(targetNodeName);
      }
      catch (ApplicationException)
      {
        node = null;
      }

      if (node == null)
      {
        throw new InvalidOperationException(string.Format("Couldn't retrieve node '{0}' from cluster '{1}'. Does the node exist?", targetNodeName, cluster.Name));
      }

      return node;
    }

    #endregion
  }
}
