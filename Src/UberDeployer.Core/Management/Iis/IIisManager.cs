using System.Collections.Generic;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Management.Iis
{
  public interface IIisManager
  {
    IDictionary<string, IisAppPoolInfo> GetAppPools(string machineName);
    
    bool AppPoolExists(string machineName, string appPoolName);
    
    void CreateAppPool(string machineName, IisAppPoolInfo appPoolInfo);
    
    void SetAppPool(string machineName, string fullWebAppName, string appPoolName);

    string GetWebApplicationPath(string machineName, string fullWebAppName);
  }
}
