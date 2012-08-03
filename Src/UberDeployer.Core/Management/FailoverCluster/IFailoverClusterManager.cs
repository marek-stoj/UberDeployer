using System.Collections.Generic;

namespace UberDeployer.Core.Management.FailoverCluster
{
  public interface IFailoverClusterManager
  {
    IEnumerable<string> GetPossibleNodeNames(string clusterMachineName, string clusterGroupName);

    string GetCurrentNodeName(string clusterMachineName, string clusterGroupName);
    
    void MoveToAnotherNode(string clusterMachineName, string clusterGroupName, string targetNodeName);
  }
}
