using System;
using System.Collections.Generic;
using NUnit.Framework;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Tests.Domain
{
  // TODO IMM HI: formatting
  [TestFixture]
  public class EnvironmentInfoTests
  {
    private const string _AppServerMachine = "app_server";
    private const string _FailoverClusterMachineName = "failover_cluster";
    private const string _EnvironmentName = "environment";
    private const string _ConfigurationTemplateName = "conf";
    private static readonly List<string> _WebMachineNames = new List<string> { "web1", "web2" };
    private const string _TerminalMachineName = "terminal_server";
    private const string _DatabaseMachineName = "databae_server";
    private const string _NtServicesBaseDirPath = "C:\\NtServices";
    private const string _WebAppsBaseDirPath = "C:\\WebApps";
    private const string _SchedulerAppsBaseDirPath = "C:\\SchedulerApps";
    private const string _TerminalAppsBaseDirPath = "C:\\TerminalApps";

    private static readonly List<EnvironmentUser> _EnvironmentUsers =
      new List<EnvironmentUser>
      {
        new EnvironmentUser("Sample.User", "some_user@centrala.kaczmarski.pl"),
      };

    private static readonly List<IisAppPoolInfo> _AppPoolInfos =
      new List<IisAppPoolInfo>()
      {
        new IisAppPoolInfo("apppool", IisAppPoolVersion.V4_0, IisAppPoolMode.Integrated),
      };

    private static readonly List<WebAppProjectConfiguration> _WebAppProjectConfigurations =
      new List<WebAppProjectConfiguration>
      {
        new WebAppProjectConfiguration("prj1", "website", "apppool", "prj1"),
      };

    private static readonly List<ProjectToFailoverClusterGroupMapping> _ProjectToFailoverClusterGroupMappings =
      new List<ProjectToFailoverClusterGroupMapping>
      {
        new ProjectToFailoverClusterGroupMapping("prj1", "cg1"),
      };

    [Test]
    public void Test_EnvironmentInfoTests_Does_Not_Allow_Template_null()
    {
      Assert.Throws<ArgumentNullException>(
        () =>
        {
          new EnvironmentInfo(
            _EnvironmentName,
            null,
            _AppServerMachine,
            _FailoverClusterMachineName,
            _WebMachineNames,
            _TerminalMachineName,
            _DatabaseMachineName,
            _NtServicesBaseDirPath,
            _WebAppsBaseDirPath,
            _SchedulerAppsBaseDirPath,
            _TerminalAppsBaseDirPath,
            false,
            _EnvironmentUsers,
            _AppPoolInfos,
            _WebAppProjectConfigurations,
            _ProjectToFailoverClusterGroupMappings);
        });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Allows_Empty_Template()
    {
      Assert.DoesNotThrow(
        () =>
        {
          new EnvironmentInfo(
            _EnvironmentName,
            "",
            _AppServerMachine,
            _FailoverClusterMachineName,
            _WebMachineNames,
            _TerminalMachineName,
            _DatabaseMachineName,
            _NtServicesBaseDirPath,
            _WebAppsBaseDirPath,
            _SchedulerAppsBaseDirPath,
            _TerminalAppsBaseDirPath,
            false,
            _EnvironmentUsers,
            _AppPoolInfos,
            _WebAppProjectConfigurations,
            _ProjectToFailoverClusterGroupMappings);
        });
    }

    [Test]
    public void Test_GetAppServerNetworkPath_Throws_When_path_startswithbackslashes()
    {
      var envInfo = new EnvironmentInfo(
        _EnvironmentName,
        _ConfigurationTemplateName,
        _AppServerMachine,
        _FailoverClusterMachineName,
        _WebMachineNames,
        _TerminalMachineName,
        _DatabaseMachineName,
        _NtServicesBaseDirPath,
        _WebAppsBaseDirPath,
        _SchedulerAppsBaseDirPath,
        _TerminalAppsBaseDirPath,
        false,
        _EnvironmentUsers,
        _AppPoolInfos,
        _WebAppProjectConfigurations,
        _ProjectToFailoverClusterGroupMappings);

      Assert.Throws<ArgumentException>(
        () => envInfo.GetAppServerNetworkPath(@"\\kasjdkasdj"));
    }

    [Test]
    public void Test_GetAppServerNetworkPath_Throws_When_path_doesntstartwithdriveletter()
    {
      var envInfo =
        new EnvironmentInfo(
          _EnvironmentName,
          _ConfigurationTemplateName,
          _AppServerMachine,
          _FailoverClusterMachineName,
          _WebMachineNames,
          _TerminalMachineName,
          _DatabaseMachineName,
          _NtServicesBaseDirPath,
          _WebAppsBaseDirPath,
          _SchedulerAppsBaseDirPath,
          _TerminalAppsBaseDirPath,
          false,
          _EnvironmentUsers,
          _AppPoolInfos,
          _WebAppProjectConfigurations,
          _ProjectToFailoverClusterGroupMappings);

      Assert.Throws<ArgumentException>(
        () => envInfo.GetAppServerNetworkPath("qlwelqwelw"));
    }

    [Test]
    public void GetWebServerNetworkPath_WhenAbsoluteLocalPathIsCorrect_ReturnCorrectPath()
    {
      var envInfo =
        new EnvironmentInfo(
          _EnvironmentName,
          _ConfigurationTemplateName,
          _AppServerMachine,
          _FailoverClusterMachineName,
          _WebMachineNames,
          _TerminalMachineName,
          _DatabaseMachineName,
          _NtServicesBaseDirPath,
          _WebAppsBaseDirPath,
          _SchedulerAppsBaseDirPath,
          _TerminalAppsBaseDirPath,
          false,
          _EnvironmentUsers,
          _AppPoolInfos,
          _WebAppProjectConfigurations,
          _ProjectToFailoverClusterGroupMappings);

      Assert.AreEqual(
        "\\\\" + _WebMachineNames[0] + "\\c$\\",
        envInfo.GetWebServerNetworkPath(_WebMachineNames[0], "c:\\"));
    }
  }
}
