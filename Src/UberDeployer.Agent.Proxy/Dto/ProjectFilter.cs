namespace UberDeployer.Agent.Proxy.Dto
{
  public class ProjectFilter
  {
    private static ProjectFilter _emptyFilter;

    public static ProjectFilter Empty
    {
      get { return _emptyFilter ?? (_emptyFilter = new ProjectFilter()); }
    }

    public string Name { get; set; }

    public string EnvironmentName { get; set; }
  }
}
