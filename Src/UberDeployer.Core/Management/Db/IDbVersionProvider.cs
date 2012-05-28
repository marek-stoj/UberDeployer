using System.Collections.Generic;

namespace UberDeployer.Core.Management.Db
{
  public interface IDbVersionProvider
  {
    IEnumerable<string> GetVersions(string dbName, string sqlServerName);
  }
}
