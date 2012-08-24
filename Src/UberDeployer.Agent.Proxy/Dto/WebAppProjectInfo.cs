namespace UberDeployer.Agent.Proxy.Dto
{
  public class WebAppProjectInfo : ProjectInfo
  {
    public string IisSiteName { get; set; }

    public string WebAppName { get; set; }

    public string WebAppDirName { get; set; }

    public IisAppPoolInfo AppPool { get; set; }
  }
}
