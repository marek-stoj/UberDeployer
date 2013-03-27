using System;
using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.WebApp.Core.Connectivity
{
  public class ClientNotConnectedException : Exception
  {
    public ClientNotConnectedException(string userIdentity)
      : base(string.Format("Client with user identity '{0}' is not connected.", userIdentity))
    {
      Guard.NotNullNorEmpty(userIdentity, "userIdentity");

      UserIdentity = userIdentity;
    }

    public string UserIdentity { get; private set; }
  }
}
