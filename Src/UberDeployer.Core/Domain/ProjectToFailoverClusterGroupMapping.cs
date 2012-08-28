using System;

namespace UberDeployer.Core.Domain
{
  public class ProjectToFailoverClusterGroupMapping
  {
    #region Constructor(s)

    public ProjectToFailoverClusterGroupMapping(string projectName, string clusterGroupName)
    {
      if (string.IsNullOrEmpty(projectName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "projectName");
      }

      if (string.IsNullOrEmpty(clusterGroupName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "clusterGroupName");
      }

      ProjectName = projectName;
      ClusterGroupName = clusterGroupName;
    }

    #endregion

    #region Overrides of Object

    public override string ToString()
    {
      return string.Format("{0} -> {1}", ProjectName, ClusterGroupName);
    }

    #endregion

    #region Properties

    public string ProjectName { get; private set; }

    public string ClusterGroupName { get; private set; }

    #endregion
  }
}
