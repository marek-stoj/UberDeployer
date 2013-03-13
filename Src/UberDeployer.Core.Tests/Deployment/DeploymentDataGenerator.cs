using System.Collections.Generic;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Tests.Deployment
{
  public class DeploymentDataGenerator
  {
    public static EnvironmentInfo GetEnvironmentInfo()
    {
      var environmentUsers =
        new List<EnvironmentUser>()
          {
            new EnvironmentUser("id", "user_name"),
            new EnvironmentUser("id2", "user_name2")
          };

      var appPoolInfos =
        new List<IisAppPoolInfo>()
        {
          new IisAppPoolInfo("apppool", IisAppPoolVersion.V4_0, IisAppPoolMode.Integrated),
        };

      var projectToWebSiteMappings =
        new List<ProjectToWebSiteMapping>
          {
            new ProjectToWebSiteMapping("prj1", "website"),
          };

      var projectToAppPoolMappings =
        new List<ProjectToAppPoolMapping>
          {
            new ProjectToAppPoolMapping("prj1", "apppool"),
          };

      var projectToFailoverClusterGroupMappings =
        new List<ProjectToFailoverClusterGroupMapping>
          {
            new ProjectToFailoverClusterGroupMapping("prj1", "cg1"),
          };

      return
        new EnvironmentInfo(
          "env_name",
          "config_template_name",
          "app_server_machine_name",
          "failover_cluster_machine_name",
          new[] { "web_server_machine_name" },
          "terminal_server_machine_name",
          "database_server_machine_name",
          "nt_service_base_dir_path",
          "web_apps_base_dir_path",
          "scheduler_apps_base_dir_path",
          "terminal_apps_base_dir_path",
          false,
          environmentUsers,
          appPoolInfos,
          projectToWebSiteMappings,
          projectToAppPoolMappings,
          projectToFailoverClusterGroupMappings);
    }

    public static DbProjectInfo GetDbProjectInfo(bool artifactsAreNotEnvironmentSpecific = true)
    {
      return
        new DbProjectInfo(
          "project_name",
          "artifacts_repository_name",
          "artifacts_repository_dir_name",
          artifactsAreNotEnvironmentSpecific,
          "database_name");
    }
  }
}
