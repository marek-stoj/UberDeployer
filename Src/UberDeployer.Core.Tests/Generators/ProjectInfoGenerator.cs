using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Tests.Generators
{
  public static class ProjectInfoGenerator
  {
    public static NtServiceProjectInfo GetNtServiceProjectInfo(bool areEnvironmentSpecific = false)
    {
      return
        new NtServiceProjectInfo(
          "name",
          "artifacts_repository_name",
          "artifacts_repository_dir_name",
          !areEnvironmentSpecific,
          "nt_service_name",
          "nt_service_dir_name",
          "nt_service_display_name",
          "nt_service_exe_name",
          "nt_service_user_id");
    }

    public static WebAppProjectInfo GetWebAppProjectInfo()
    {
      return
        new WebAppProjectInfo(
          "name",
          "artifacts_repository_name",
          "artifacts_repository_dir_name",
          true);
    }

    public static WebServiceProjectInfo GetWebServiceProjectInfo()
    {
      return
        new WebServiceProjectInfo(
          "name",
          "artifacts_repository_name",
          "artifacts_repository_dir_name",
          true);
    }

    public static DbProjectInfo GetDbProjectInfo(bool areEnvironmentSpecific = false)
    {
      return
        new DbProjectInfo(
          "name",
          "artifacts_repository_name",
          "artifacts_repository_dir_name",
          !areEnvironmentSpecific,
          "database_name");
    }

    public static TerminalAppProjectInfo GetTerminalAppProjectInfo()
    {
      return
        new TerminalAppProjectInfo(
          "name",
          "artifacts_repository_name",
          "artifacts_repository_dir_name",
          true,
          "terminal_app_name",
          "terminal_app_dir_name",
          "terminal_app_exe_name");
    }

    public static SchedulerAppProjectInfo GetSchedulerAppProjectInfo()
    {
      return
        new SchedulerAppProjectInfo(
          "name",
          "artifacts_repository_name",
          "artifacts_repository_dir_name",
          true,
          "scheduler_app_name",
          "scheduler_app_dir_name",
          "scheduler_app_exe_name",
          "scheduler_app_user_id",
          0,
          0,
          0);
    }
  }
}
