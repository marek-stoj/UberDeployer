using System.Collections.Generic;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Tests
{
  // TODO IMM HI: xxx names
  public static class TestData
  {
    public static readonly List<EnvironmentUser> _EnvironmentUsers =
      new List<EnvironmentUser>
        {
          new EnvironmentUser("Sample.User", "some_user@centrala.kaczmarski.pl"),
        };

    public static readonly List<IisAppPoolInfo> _AppPoolInfos =
      new List<IisAppPoolInfo>()
      {
        new IisAppPoolInfo("apppool", IisAppPoolVersion.V4_0, IisAppPoolMode.Integrated),
      };

    public static readonly List<DatabaseServer> _DatabaseServers =
      new List<DatabaseServer>()
      {
        new DatabaseServer("db_server_id", "db_server"),
      };

    public static readonly List<WebAppProjectConfiguration> _WebAppProjectConfigurations =
      new List<WebAppProjectConfiguration>
      {
        new WebAppProjectConfiguration("webappprj", "apppool", "website", "dir", "webapp"),
      };

    public static readonly List<DbProjectConfiguration> _DbProjectConfigurations =
      new List<DbProjectConfiguration>
      {
        new DbProjectConfiguration("dbprj", "db_server_id"),
      };

    public static readonly List<ProjectToFailoverClusterGroupMapping> _ProjectToFailoverClusterGroupMappings =
      new List<ProjectToFailoverClusterGroupMapping>
        {
          new ProjectToFailoverClusterGroupMapping("ntsvcprj", "cg1"),
        };
  }
}
