using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Core.Management.Metadata
{
  public class MachineSpecificProjectVersion
  {
    public MachineSpecificProjectVersion(string machineName, string projectVersion)
    {
      Guard.NotNullNorEmpty(machineName, "machineName");
      Guard.NotNullNorEmpty(projectVersion, "projectVersion");

      MachineName = machineName;
      ProjectVersion = projectVersion;
    }

    public string MachineName { get; private set; }

    public string ProjectVersion { get; private set; }
  }
}
