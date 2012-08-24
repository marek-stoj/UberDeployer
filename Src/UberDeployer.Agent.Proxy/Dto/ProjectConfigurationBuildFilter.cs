namespace UberDeployer.Agent.Proxy.Dto
{
  public class ProjectConfigurationBuildFilter
  {
    private static ProjectConfigurationBuildFilter _emptyFilter;

    public static ProjectConfigurationBuildFilter Empty
    {
      get { return _emptyFilter ?? (_emptyFilter = new ProjectConfigurationBuildFilter()); }
    }

    public string Number { get; set; }
  }
}
