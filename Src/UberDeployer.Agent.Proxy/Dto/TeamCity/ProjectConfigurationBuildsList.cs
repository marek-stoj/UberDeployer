using System.Collections.Generic;

namespace UberDeployer.Agent.Proxy.Dto.TeamCity
{
  public class ProjectConfigurationBuildsList
  {
    public List<ProjectConfigurationBuild> Builds { get; set; }

    public int Count { get; set; }
  }
}
