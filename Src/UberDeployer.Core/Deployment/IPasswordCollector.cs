namespace UberDeployer.Core.Deployment
{
  public interface IPasswordCollector
  {
    string CollectPasswordForUser(string environmentName, string machineName, string userName);
  }
}
