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
    private const string _SchedulerMachineName = "scheduler_server";
    private const string _NtServicesBaseDirPath = "C:\\NtServices";
    private const string _WebAppsBaseDirPath = "C:\\WebApps";
    private const string _SchedulerAppsBaseDirPath = "C:\\SchedulerApps";
    private const string _TerminalAppsBaseDirPath = "C:\\TerminalApps";
    private const string _TerminalAppsShortcutFolder = "C:\\TerminalAppShortcuts";

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
            _SchedulerMachineName,
            _NtServicesBaseDirPath,
            _WebAppsBaseDirPath,
            _SchedulerAppsBaseDirPath,
            _TerminalAppsBaseDirPath,
            false,
            TestData._EnvironmentUsers,
            TestData._AppPoolInfos,
            TestData._DatabaseServers,
            TestData._WebAppProjectConfigurations,
            TestData._ProjectToFailoverClusterGroupMappings,
            TestData._DbProjectConfigurations,
            _TerminalAppsShortcutFolder);
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
            _SchedulerMachineName,
            _NtServicesBaseDirPath,
            _WebAppsBaseDirPath,
            _SchedulerAppsBaseDirPath,
            _TerminalAppsBaseDirPath,
            false,
            TestData._EnvironmentUsers,
            TestData._AppPoolInfos,
            TestData._DatabaseServers,
            TestData._WebAppProjectConfigurations,
            TestData._ProjectToFailoverClusterGroupMappings,
            TestData._DbProjectConfigurations,
            _TerminalAppsShortcutFolder);
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
        _SchedulerMachineName,
        _DatabaseMachineName,
        _NtServicesBaseDirPath,
        _WebAppsBaseDirPath,
        _SchedulerAppsBaseDirPath,
        _TerminalAppsBaseDirPath,
        false,
        _EnvironmentUsers,
        _AppPoolInfos,
        _WebAppProjectConfigurations,
        _ProjectToFailoverClusterGroupMappings,
        _TerminalAppsShortcutFolder);

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
          _SchedulerMachineName,
          _NtServicesBaseDirPath,
          _WebAppsBaseDirPath,
          _SchedulerAppsBaseDirPath,
          _TerminalAppsBaseDirPath,
          false,
          TestData._EnvironmentUsers,
          TestData._AppPoolInfos,
          TestData._DatabaseServers,
          TestData._WebAppProjectConfigurations,
          TestData._ProjectToFailoverClusterGroupMappings,
          TestData._DbProjectConfigurations,
          _TerminalAppsShortcutFolder);

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
          _SchedulerMachineName,
          _NtServicesBaseDirPath,
          _WebAppsBaseDirPath,
          _SchedulerAppsBaseDirPath,
          _TerminalAppsBaseDirPath,
          false,
          TestData._EnvironmentUsers,
          TestData._AppPoolInfos,
          TestData._DatabaseServers,
          TestData._WebAppProjectConfigurations,
          TestData._ProjectToFailoverClusterGroupMappings,
          TestData._DbProjectConfigurations,
          _TerminalAppsShortcutFolder);

      Assert.AreEqual(
        "\\\\" + _WebMachineNames[0] + "\\c$\\",
        envInfo.GetWebServerNetworkPath(_WebMachineNames[0], "c:\\"));
    }
  }
}
