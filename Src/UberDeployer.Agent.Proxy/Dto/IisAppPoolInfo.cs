namespace UberDeployer.Agent.Proxy.Dto
{
  public class IisAppPoolInfo
  {
    public string Name { get; set; }

    public IisAppPoolVersion Version { get; set; }

    public IisAppPoolMode Mode { get; set; }
  }
}
