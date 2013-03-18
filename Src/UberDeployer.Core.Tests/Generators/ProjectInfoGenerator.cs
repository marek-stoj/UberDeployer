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
          "artifactsRepositoryName",
          "artifactsRepositoryDirName",
          !areEnvironmentSpecific,
          "ntServiceName",
          "ntServiceDirName",
          "ntServiceDisplayName",
          "ntServiceExeName",
          "ntServiceUserId");
    }

    public static WebAppProjectInfo GetWebAppProjectInfo()
    {
      return
        new WebAppProjectInfo(
          "name",
          "artifactsRepositoryName",
          "artifactsRepositoryDirName",
          true,
          "webAppDirName");
    }

    public static WebServiceProjectInfo GetWebServiceProjectInfo()
    {
      return
        new WebServiceProjectInfo(
          "name",
          "artifactsRepositoryName",
          "artifactsRepositoryDirName",
          true,
          "webAppDirName");
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
          "artifactsRepositoryName",
          "artifactsRepositoryDirName",
          true,
          "terminalAppName",
          "terminalAppDirName",
          "terminalAppExeName");
    }

    public static SchedulerAppProjectInfo GetSchedulerAppProjectInfo()
    {
      return
        new SchedulerAppProjectInfo(
          "name",
          "artifactsRepositoryName",
          "artifactsRepositoryDirName",
          true,
          "schedulerAppName",
          "schedulerAppDirName",
          "schedulerAppExeName",
          "schedulerAppUserId",
          0,
          0,
          0);
    }
  }
}
