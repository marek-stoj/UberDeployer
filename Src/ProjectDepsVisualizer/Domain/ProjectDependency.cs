namespace ProjectDepsVisualizer.Domain
{
  public class ProjectDependency
  {
    public override string ToString()
    {
      return string.Format("{0}, {1}, {2}", ProjectName, ProjectConfiguration, ProjectVersion);
    }

    public string ProjectName { get; set; }

    public string ProjectConfiguration { get; set; }

    public string ProjectVersion { get; set; }
  }
}
