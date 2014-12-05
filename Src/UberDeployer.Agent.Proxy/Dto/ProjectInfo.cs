﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UberDeployer.Agent.Proxy.Dto
{
  [KnownType(typeof(NtServiceProjectInfo))]
  [KnownType(typeof(WebAppProjectInfo))]
  [KnownType(typeof(WebServiceProjectInfo))]
  [KnownType(typeof(TerminalAppProjectInfo))]
  [KnownType(typeof(SchedulerAppProjectInfo))]
  [KnownType(typeof(DbProjectInfo))]
  [KnownType(typeof(UberDeployerAgentProjectInfo))]
  public class ProjectInfo
  {
    public string Name { get; set; }

    public ProjectType Type { get; set; }

    public string ArtifactsRepositoryName { get; set; }

    public string ArtifactsRepositoryDirName { get; set; }
    
    public bool ArtifactsAreEnvironmentSpecific { get; set; }

    public List<string> AllowedEnvironmentNames { get; set; }
  }
}
