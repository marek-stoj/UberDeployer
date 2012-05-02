using System;
using System.Collections.Generic;
using System.ComponentModel;
using UberDeployer.Core.Deployment;

namespace UberDeployer.Core.Domain
{
  // TODO IMM HI: get rid of System.ComponentModel attributes in ProjectInfo and all derived classes?
  public abstract class ProjectInfo
  {
    protected ProjectInfo(string name, string artifactsRepositoryName, string artifactsRepositoryDirName = null)
    {
      if (string.IsNullOrEmpty(name))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "name");
      }

      if (string.IsNullOrEmpty(artifactsRepositoryName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "artifactsRepositoryName");
      }

      Name = name;
      ArtifactsRepositoryName = artifactsRepositoryName;
      ArtifactsRepositoryDirName = artifactsRepositoryDirName;
    }

    public abstract DeploymentTask CreateDeploymentTask(IObjectFactory objectFactory, string projectConfigurationName, string projectConfigurationBuildId, string targetEnvironmentName);

    public abstract IEnumerable<string> GetTargetFolders(EnvironmentInfo environmentInfo);

    [Category("Common")]
    public string Name { get; private set; }

    [Category("Common")]
    public virtual string Type
    {
      get { return GetType().Name.Replace("ProjectInfo", ""); }
    }

    [Category("Common")]
    public string ArtifactsRepositoryName { get; private set; }

    [Category("Common")]
    public string ArtifactsRepositoryDirName { get; private set; }
  }
}
