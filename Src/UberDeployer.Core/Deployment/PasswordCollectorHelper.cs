using System;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  public static class PasswordCollectorHelper
  {
    public static string CollectPasssword(IPasswordCollector passwordCollector, EnvironmentInfo environmentInfo, string machineName, string userId, out EnvironmentUser environmentUser)
    {
      if (passwordCollector == null)
      {
        throw new ArgumentNullException("passwordCollector");
      }

      if (environmentInfo == null)
      {
        throw new ArgumentNullException("environmentInfo");
      }

      if (string.IsNullOrEmpty(machineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "machineName");
      }

      if (string.IsNullOrEmpty(userId))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "userId");
      }

      environmentUser = environmentInfo.GetEnvironmentUserByName(userId);

      if (environmentUser == null)
      {
        throw new InvalidOperationException(string.Format("There's no environment user with id '{0}' defined in environment named '{1}'.", userId, environmentInfo.Name));
      }

      string environmentUserPassword =
        passwordCollector.CollectPasswordForUser(
          environmentInfo.Name,
          machineName,
          environmentUser.UserName);

      if (string.IsNullOrEmpty(environmentUserPassword))
      {
        throw new InvalidOperationException(string.Format("Couldn't obtain password for user named '{0}' for environment named '{1}'.", environmentUser.UserName, environmentInfo.Name));
      }

      return environmentUserPassword;
    }
  }
}
