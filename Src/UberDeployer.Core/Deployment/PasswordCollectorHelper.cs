using System;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  public static class PasswordCollectorHelper
  {
    public static string CollectPasssword(IPasswordCollector passwordCollector, Guid deploymentId, EnvironmentInfo environmentInfo, string machineName, EnvironmentUser environmentUser, EventHandler<DiagnosticMessageEventArgs> onDiagnosticMessagePostedAction)
    {
      Guard.NotEmpty(deploymentId, "deploymentId");
      Guard.NotNull(passwordCollector, "passwordCollector");
      Guard.NotNull(environmentInfo, "environmentInfo");
      Guard.NotNullNorEmpty(machineName, "machineName");
      Guard.NotNull(environmentUser, "environmentUser");

      passwordCollector.DiagnosticMessagePosted += onDiagnosticMessagePostedAction;

      try
      {
        string environmentUserPassword =
          passwordCollector.CollectPasswordForUser(
            deploymentId,
            environmentInfo.Name,
            machineName,
            environmentUser.UserName);

        if (string.IsNullOrEmpty(environmentUserPassword))
        {
          throw new InvalidOperationException(string.Format("Couldn't obtain password for user named '{0}' for environment named '{1}'.", environmentUser.UserName, environmentInfo.Name));
        }

        return environmentUserPassword;
      }
      finally
      {
        passwordCollector.DiagnosticMessagePosted -= onDiagnosticMessagePostedAction;
      }
    }
  }
}
