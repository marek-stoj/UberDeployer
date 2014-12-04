using System.Collections.Generic;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Tests
{
  public static class TestData
  {
    public static readonly List<EnvironmentUser> EnvironmentUsers =
      new List<EnvironmentUser>
        {
          new EnvironmentUser("Sample.User", "some_user@centrala.kaczmarski.pl"),
        };

    public static readonly List<IisAppPoolInfo> AppPoolInfos =
      new List<IisAppPoolInfo>()
      {
        new IisAppPoolInfo("apppool", IisAppPoolVersion.V4_0, IisAppPoolMode.Integrated),
      };

    public static readonly List<DatabaseServer> DatabaseServers =
      new List<DatabaseServer>()
      {
        new DatabaseServer("db_server_id", "db_server"),
      };

    public static readonly List<ProjectToFailoverClusterGroupMapping> ProjectToFailoverClusterGroupMappings =
      new List<ProjectToFailoverClusterGroupMapping>
        {
          new ProjectToFailoverClusterGroupMapping("ntsvcprj", "cg1"),
          new ProjectToFailoverClusterGroupMapping("project_name", "cg1"),
        };

    public static readonly List<WebAppProjectConfigurationOverride> WebAppProjectConfigurationOverrides =
      new List<WebAppProjectConfigurationOverride>
      {
        new WebAppProjectConfigurationOverride("webappprj", "apppool", "website", "dir", "webapp"),
      };

    public static readonly List<DbProjectConfigurationOverride> DbProjectConfigurationOverrides =
      new List<DbProjectConfigurationOverride>
      {
        new DbProjectConfigurationOverride("dbprj", "db_server_id"),
      };
  }
}
