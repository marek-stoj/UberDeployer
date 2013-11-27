using System.Collections.Generic;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Tests.Generators
{
  public class DeploymentDataGenerator
  {
    public static EnvironmentInfo GetEnvironmentInfo()
    {
      return GetEnvironmentInfo(new[] { new EnvironmentUser("id", "user_name"), });
    }

    public static EnvironmentInfo GetEnvironmentInfo(IEnumerable<EnvironmentUser> users)
    {
      return
        new EnvironmentInfo(
          "env_name",
          "config_template_name",
          "app_server_machine_name",
          "failover_cluster_machine_name",
          new[] { "web_server_machine_name" },
          "terminal_server_machine_name",
          "schedulerServerMachineName",
          "nt_service_base_dir_path",
          "web_apps_base_dir_path",
          "X:\\scheduler_apps_base_dir_path",
          "X:\\terminal_apps_base_dir_path",
          false,
          users,
          TestData.AppPoolInfos,
          TestData.DatabaseServers,
          TestData.WebAppProjectConfigurations,
          TestData.ProjectToFailoverClusterGroupMappings,
          TestData.DbProjectConfigurations,
          "X:\\terminal_apps_shortcut_folder",
          "X:\\artifacts_deployment_dir_path");
    }

    public static ProjectInfo GetTerminalAppProjectInfo()
    {
      return new TerminalAppProjectInfo("project_name", "artifactsRepositoryName", new[] { "env_name" }, "artifactsRepositoryDirName", false, "terminalAppName", "terminalAppDirName", "terminalAppExeName");
    }
  }
}
