using System;
using System.Collections.Generic;
using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Core.Management.Metadata
{
  public class ProjectMetadata
  {
    public ProjectMetadata(string projectName, string environmentName, IEnumerable<MachineSpecificProjectVersion> projectVersions)
    {
      Guard.NotNullNorEmpty(projectName, "projectName");
      Guard.NotNullNorEmpty(environmentName, "environmentName");
      
      if (projectVersions == null)
      {
        throw new ArgumentNullException("projectVersions");
      }

      ProjectName = projectName;
      EnvironmentName = environmentName;
      ProjectVersions = new List<MachineSpecificProjectVersion>(projectVersions);
    }

    public string ProjectName { get; private set; }

    public string EnvironmentName { get; private set; }

    public IEnumerable<MachineSpecificProjectVersion> ProjectVersions { get; private set; }
  }
}
