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

    private static readonly List<ProjectToFailoverClusterGroupMapping> _ProjectToFailoverClusterGroupMappings =
      new List<ProjectToFailoverClusterGroupMapping>
        {
          new ProjectToFailoverClusterGroupMapping("prj1", "cg1"),
        };

    [Test]
    public void Test_EnvironmentInfoTests_Throws_When_Name_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
          {
            new EnvironmentInfo(
              null,
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
              _ProjectToFailoverClusterGroupMappings);
          });
    }

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
              _ProjectToFailoverClusterGroupMappings);
          });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Throws_When_Machine_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
          {
            new EnvironmentInfo(
              _EnvironmentName,
              _ConfigurationTemplateName,
              null,
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
              _ProjectToFailoverClusterGroupMappings);
          });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Throws_When_FailoverClusterMachineName_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
          {
            new EnvironmentInfo(
              _EnvironmentName,
              _ConfigurationTemplateName,
              _AppServerMachine,
              null,
              _WebMachineNames,
              _TerminalMachineName,
              _DatabaseMachineName,
              _NtServicesBaseDirPath,
              _WebAppsBaseDirPath,
              _SchedulerAppsBaseDirPath,
              _TerminalAppsBaseDirPath,
              false,
              _EnvironmentUsers,
              _ProjectToFailoverClusterGroupMappings);
          });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Throws_When_Webmachine_null()
    {
      Assert.Throws<ArgumentNullException>(
        () =>
          {
            new EnvironmentInfo(
              _EnvironmentName,
              _ConfigurationTemplateName,
              _AppServerMachine,
              _FailoverClusterMachineName,
              null,
              _TerminalMachineName,
              _DatabaseMachineName,
              _NtServicesBaseDirPath,
              _WebAppsBaseDirPath,
              _SchedulerAppsBaseDirPath,
              _TerminalAppsBaseDirPath,
              false,
              _EnvironmentUsers,
              _ProjectToFailoverClusterGroupMappings);
          });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Throws_When_Terminalmachine_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
          {
            new EnvironmentInfo(
              _EnvironmentName,
              _ConfigurationTemplateName,
              _AppServerMachine,
              _FailoverClusterMachineName,
              _WebMachineNames,
              null,
              _DatabaseMachineName,
              _NtServicesBaseDirPath,
              _WebAppsBaseDirPath,
              _SchedulerAppsBaseDirPath,
              _TerminalAppsBaseDirPath,
              false,
              _EnvironmentUsers,
              _ProjectToFailoverClusterGroupMappings);
          });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Throws_When_BaseDirPath_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
          {
            new EnvironmentInfo(
              _EnvironmentName,
              _ConfigurationTemplateName,
              _AppServerMachine,
              _FailoverClusterMachineName,
              _WebMachineNames,
              _TerminalMachineName,
              _DatabaseMachineName,
              null,
              _WebAppsBaseDirPath,
              _SchedulerAppsBaseDirPath,
              _TerminalAppsBaseDirPath,
              false,
              _EnvironmentUsers,
              _ProjectToFailoverClusterGroupMappings);
          });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Throws_When_Webbasedir_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
          {
            new EnvironmentInfo(
              _EnvironmentName,
              _ConfigurationTemplateName,
              _AppServerMachine,
              _FailoverClusterMachineName,
              _WebMachineNames,
              _TerminalMachineName,
              _DatabaseMachineName,
              _NtServicesBaseDirPath,
              null,
              _SchedulerAppsBaseDirPath,
              _TerminalAppsBaseDirPath,
              false,
              _EnvironmentUsers,
              _ProjectToFailoverClusterGroupMappings);
          });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Throws_When_Scheduler_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
          {
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
              null,
              _TerminalAppsBaseDirPath,
              false,
              _EnvironmentUsers,
              _ProjectToFailoverClusterGroupMappings);
          });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Throws_When_Terminal_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
          {
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
              null,
              false,
              _EnvironmentUsers,
              _ProjectToFailoverClusterGroupMappings);
          });
    }

    [Test]
    public void Test_GetAppServerNetworkPath_Throws_When_path_null()
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
          _ProjectToFailoverClusterGroupMappings);

      Assert.Throws<ArgumentException>(
        () => envInfo.GetAppServerNetworkPath(null));
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
          _ProjectToFailoverClusterGroupMappings);

      Assert.AreEqual(
        "\\\\" + _WebMachineNames[0] + "\\c$\\",
        envInfo.GetWebServerNetworkPath(_WebMachineNames[0], "c:\\"));
    }
  }
}
