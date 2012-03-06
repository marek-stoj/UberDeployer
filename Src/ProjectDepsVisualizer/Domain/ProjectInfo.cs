using System.Collections.Generic;

namespace ProjectDepsVisualizer.Domain
{
  public class ProjectInfo
  {
    private List<ProjectDependency> _projectDependencies = new List<ProjectDependency>();

    public override string ToString()
    {
      return string.Format("{0}, {1}, {2} ({3} dep(s))", ProjectName, ProjectConfiguration, ProjectVersion, ProjectDependencies.Count);
    }

    public string ProjectName { get; set; }

    public string ProjectConfiguration { get; set; }

    public string ProjectVersion { get; set; }

    public List<ProjectDependency> ProjectDependencies
    {
      get { return _projectDependencies; }
      set { _projectDependencies = value; }
    }
  }
}
