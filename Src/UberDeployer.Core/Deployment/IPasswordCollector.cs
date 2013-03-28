using System;

namespace UberDeployer.Core.Deployment
{
  public interface IPasswordCollector
  {
    event EventHandler<DiagnosticMessageEventArgs> DiagnosticMessagePosted;

    string CollectPasswordForUser(Guid deploymentId, string environmentName, string machineName, string userName);
  }
}
