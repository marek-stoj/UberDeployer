using System.ServiceModel;

namespace UberDeployer.Agent.Proxy
{
  [ServiceContract]
  public interface IAgentService
  {
    [OperationContract]
    string SayHello(string name);
  }
}
