using System;

namespace UberDeployer.Core.Deployment
{
  public class CollectedCredentials
  {
    public CollectedCredentials(string userName, string password = null)
    {
      if (string.IsNullOrEmpty(userName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "userName");
      }

      UserName = userName;
      Password = password ?? "";
    }

    public string UserName { get; private set; }

    public string Password { get; private set; }
  }
}
