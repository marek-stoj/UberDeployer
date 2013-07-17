using System;
using System.Collections.Generic;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain.Input;

namespace UberDeployer.Core.Domain
{
  public abstract class ProjectInfo
  {
    protected ProjectInfo(string name, string artifactsRepositoryName, IEnumerable<string> allowedEnvironmentNames, string artifactsRepositoryDirName = null, bool artifactsAreNotEnvironmentSpecific = false)
    {
      Guard.NotNullNorEmpty(name, "name");
      Guard.NotNullNorEmpty(artifactsRepositoryName, "artifactsRepositoryName");

      if (allowedEnvironmentNames == null)
      {
        throw new ArgumentNullException("allowedEnvironmentNames");
      }

      Name = name;
      ArtifactsRepositoryName = artifactsRepositoryName;
      ArtifactsRepositoryDirName = artifactsRepositoryDirName;
      ArtifactsAreEnvironmentSpecific = !artifactsAreNotEnvironmentSpecific;
      AllowedEnvironmentNames = new List<string>(allowedEnvironmentNames);
    }

    public abstract ProjectType Type { get; }

    public abstract InputParams CreateEmptyInputParams();

    public abstract DeploymentTask CreateDeploymentTask(IObjectFactory objectFactory);

    public abstract IEnumerable<string> GetTargetFolders(IObjectFactory objectFactory, EnvironmentInfo environmentInfo);

    public abstract string GetMainAssemblyFileName();

    public string Name { get; private set; }

    public string ArtifactsRepositoryName { get; private set; }

    public string ArtifactsRepositoryDirName { get; private set; }
    
    public bool ArtifactsAreEnvironmentSpecific { get; private set; }

    public IEnumerable<string> AllowedEnvironmentNames { get; private set; }
  }
}
