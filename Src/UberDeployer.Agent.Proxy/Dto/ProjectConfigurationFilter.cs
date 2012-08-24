namespace UberDeployer.Agent.Proxy.Dto
{
  public class ProjectConfigurationFilter
  {
    private static ProjectConfigurationFilter _emptyFilter;

    public static ProjectConfigurationFilter Empty
    {
      get { return _emptyFilter ?? (_emptyFilter = new ProjectConfigurationFilter()); }
    }

    public string Name { get; set; }
  }
}
