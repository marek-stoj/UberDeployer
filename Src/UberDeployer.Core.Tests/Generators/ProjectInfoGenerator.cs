using System;
using System.Collections.Generic;
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
          new[] { "env_name" },
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
          "webappprj",
          "artifacts_repository_name",
          new[] { "env_name" },
          "artifacts_repository_dir_name",
          true);
    }

    public static WebServiceProjectInfo GetWebServiceProjectInfo()
    {
      return
        new WebServiceProjectInfo(
          "name",
          "artifacts_repository_name",
          new[] { "env_name" },
          "artifacts_repository_dir_name",
          true);
    }

    public static DbProjectInfo GetDbProjectInfo(bool areEnvironmentSpecific = false)
    {
      return
        new DbProjectInfo(
          "dbprj",
          "artifacts_repository_name",
          new[] { "env_name" },
          "artifacts_repository_dir_name",
          !areEnvironmentSpecific,
          "database_name",
          "database_server");
    }

    public static TerminalAppProjectInfo GetTerminalAppProjectInfo()
    {
      return
        new TerminalAppProjectInfo(
          "name",
          "artifacts_repository_name",
          new[] { "env_name" },
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
          new[] { "env_name" },
          "artifacts_repository_dir_name",
          true,
          "scheduler_app_dir_name",
          "scheduler_app_exe_name",
          new List<SchedulerAppTask>
          {
            new SchedulerAppTask(
              "task_name_1",
              "task_executable_name_1",
              "task_user_1",
              0,
              0,
              0,
              Repetition.CreatedDisabled()),
            new SchedulerAppTask(
              "task_name_2",
              "task_executable_name_2",
              "task_user_2",
              0,
              0,
              0,
              Repetition.CreateEnabled(
                TimeSpan.FromMinutes(15.0),
                TimeSpan.FromDays(1.0),
                true)),
          });
    }
  }
}
