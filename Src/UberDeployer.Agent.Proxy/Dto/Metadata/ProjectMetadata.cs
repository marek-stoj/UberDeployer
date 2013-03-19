using System.Collections.Generic;

namespace UberDeployer.Agent.Proxy.Dto.Metadata
{
  public class ProjectMetadata
  {
    public string ProjectName { get; set; }

    public string EnvironmentName { get; set; }

    public List<MachineSpecificProjectVersion> ProjectVersions { get; set; }
  }
}
