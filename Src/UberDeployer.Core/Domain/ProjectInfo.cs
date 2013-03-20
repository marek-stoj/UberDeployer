﻿using System;
using System.Collections.Generic;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain.Input;

namespace UberDeployer.Core.Domain
{
  public abstract class ProjectInfo
  {
    protected ProjectInfo(string name, string artifactsRepositoryName, string artifactsRepositoryDirName = null, bool artifactsAreNotEnvironmentSpecific = false)
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
      ArtifactsAreEnvironmentSpecific = !artifactsAreNotEnvironmentSpecific;
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
  }
}
