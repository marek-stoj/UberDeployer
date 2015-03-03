using System.Collections.Generic;

namespace UberDeployer.Core.Management.Db
{
  public interface IDbVersionProvider
  {
    IEnumerable<DbVersionInfo> GetVersions(string dbName, string sqlServerName);
  }
}