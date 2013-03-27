using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace UberDeployer.WebApp.Core.Connectivity
{
  public class MyPersistentConnection : PersistentConnection
  {
    protected override Task OnReceived(IRequest request, string connectionId, string data)
    {
      // Broadcast data to all clients
      return Connection.Broadcast(data);
    }
  }
}
