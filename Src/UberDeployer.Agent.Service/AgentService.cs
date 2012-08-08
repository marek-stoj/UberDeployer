using UberDeployer.Agent.Proxy;

namespace UberDeployer.Agent.Service
{
  public class AgentService : IAgentService
  {
    #region IAgentService Members

    public string SayHello(string name)
    {
      return string.Format("Hello {0}!", name);
    }

    #endregion
  }
}
