namespace UberDeployer.Agent.Proxy
{
  public class AgentServiceClient : WcfProxy<IAgentService>, IAgentService
  {
    #region IAgentService Members

    public string SayHello(string name)
    {
      return Exec(@as => @as.SayHello(name));
    }

    #endregion
  }
}
