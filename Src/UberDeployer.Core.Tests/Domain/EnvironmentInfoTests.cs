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
    private const string Machine = "machine";
    private const string Name = "name";
    private const string Template = "template";
    private const string Webmachine = "webmachine";
    private const string Terminalmachine = "terminalmachine";
    private const string DatabaseMachine = "databasemachine";
    private const string BaseDirPath = "baseDirPath";
    private const string Webbasedir = "webbasedir";
    private const string Scheduler = "scheduler";
    private const string Terminal = "terminal";

    private static readonly List<EnvironmentUser> EnvironmentUsers =
      new List<EnvironmentUser>
        {
          new EnvironmentUser("Sample.User", "some_user@centrala.kaczmarski.pl"),
        };

    [Test]
    public void Test_EnvironmentInfoTests_Thows_When_Name_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new EnvironmentInfo(
            null, Template, Machine, Webmachine, Terminalmachine, DatabaseMachine, BaseDirPath, Webbasedir, Scheduler, Terminal, EnvironmentUsers);
        });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Does_Not_Allow_Template_null()
    {
      Assert.Throws<ArgumentNullException>(
        () =>
        {
          new EnvironmentInfo(
            Name, null, Machine, Webmachine, Terminalmachine, DatabaseMachine, BaseDirPath, Webbasedir, Scheduler, Terminal, EnvironmentUsers);
        });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Allows_Empty_Template()
    {
      Assert.DoesNotThrow(
        () =>
        {
          new EnvironmentInfo(
            Name, "", Machine, Webmachine, Terminalmachine, DatabaseMachine, BaseDirPath, Webbasedir, Scheduler, Terminal, EnvironmentUsers);
        });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Thows_When_Machine_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new EnvironmentInfo(
            Name, Template, null, Webmachine, Terminalmachine, DatabaseMachine, BaseDirPath, Webbasedir, Scheduler, Terminal, EnvironmentUsers);
        });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Thows_When_Webmachine_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new EnvironmentInfo(
            Name, Template, Machine, null, Terminalmachine, DatabaseMachine, BaseDirPath, Webbasedir, Scheduler, Terminal, EnvironmentUsers);
        });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Thows_When_Terminalmachine_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new EnvironmentInfo(
            Name, Template, Machine, Webmachine, null, DatabaseMachine, BaseDirPath, Webbasedir, Scheduler, Terminal, EnvironmentUsers);
        });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Thows_When_BaseDirPath_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new EnvironmentInfo(
            Name, Template, Machine, Webmachine, Terminalmachine, DatabaseMachine, null, Webbasedir, Scheduler, Terminal, EnvironmentUsers);
        });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Thows_When_Webbasedir_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new EnvironmentInfo(
            Name, Template, Machine, Webmachine, Terminalmachine, DatabaseMachine, BaseDirPath, null, Scheduler, Terminal, EnvironmentUsers);
        });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Thows_When_Scheduler_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new EnvironmentInfo(
            Name, Template, Machine, Webmachine, Terminalmachine, DatabaseMachine, BaseDirPath, Webbasedir, null, Terminal, EnvironmentUsers);
        });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Thows_When_Terminal_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new EnvironmentInfo(
            Name, Template, Machine, Webmachine, Terminalmachine, DatabaseMachine, BaseDirPath, Webbasedir, Scheduler, null, EnvironmentUsers);
        });
    }

    [Test]
    public void Test_GetAppServerNetworkPath_Throws_When_path_null()
    {
      var envInfo = new EnvironmentInfo(
            Name, Template, Machine, Webmachine, Terminalmachine, DatabaseMachine, BaseDirPath, Webbasedir, Scheduler, Terminal, EnvironmentUsers);

      Assert.Throws<ArgumentException>(
        () => envInfo.GetAppServerNetworkPath(null));
    }

    [Test]
    public void Test_GetAppServerNetworkPath_Throws_When_path_startswithbackslashes()
    {
      var envInfo = new EnvironmentInfo(
            Name, Template, Machine, Webmachine, Terminalmachine, DatabaseMachine, BaseDirPath, Webbasedir, Scheduler, Terminal, EnvironmentUsers);

      Assert.Throws<ArgumentException>(
        () => envInfo.GetAppServerNetworkPath(@"\\kasjdkasdj"));
    }

    [Test]
    public void Test_GetAppServerNetworkPath_Throws_When_path_doesntstartwithdriveletter()
    {
      var envInfo = new EnvironmentInfo(
            Name, Template, Machine, Webmachine, Terminalmachine, DatabaseMachine, BaseDirPath, Webbasedir, Scheduler, Terminal, EnvironmentUsers);

      Assert.Throws<ArgumentException>(
        () => envInfo.GetAppServerNetworkPath("qlwelqwelw"));
    }

    [Test]
    public void GetWebServerNetworkPath_WhenAbsoluteLocalPathIsCorrect_ReturnCorrectPath()
    {
      var envInfo =
        new EnvironmentInfo(
          Name,
          Template,
          Machine,
          Webmachine,
          Terminalmachine,
          DatabaseMachine,
          BaseDirPath,
          Webbasedir,
          Scheduler,
          Terminal,
          EnvironmentUsers);

      Assert.AreEqual("\\\\" + Webmachine + "\\c$\\", envInfo.GetWebServerNetworkPath("c:\\"));
    }
  }
}
