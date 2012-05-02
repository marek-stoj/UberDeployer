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
    private static readonly List<string> Webmachines = new List<string> { "webmachine1", "webmachine2" };
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
            null, Template, Machine, Webmachines, Terminalmachine, DatabaseMachine, BaseDirPath, Webbasedir, Scheduler, Terminal, EnvironmentUsers);
        });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Does_Not_Allow_Template_null()
    {
      Assert.Throws<ArgumentNullException>(
        () =>
        {
          new EnvironmentInfo(
            Name, null, Machine, Webmachines, Terminalmachine, DatabaseMachine, BaseDirPath, Webbasedir, Scheduler, Terminal, EnvironmentUsers);
        });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Allows_Empty_Template()
    {
      Assert.DoesNotThrow(
        () =>
        {
          new EnvironmentInfo(
            Name, "", Machine, Webmachines, Terminalmachine, DatabaseMachine, BaseDirPath, Webbasedir, Scheduler, Terminal, EnvironmentUsers);
        });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Thows_When_Machine_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new EnvironmentInfo(
            Name, Template, null, Webmachines, Terminalmachine, DatabaseMachine, BaseDirPath, Webbasedir, Scheduler, Terminal, EnvironmentUsers);
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
            Name, Template, Machine, Webmachines, null, DatabaseMachine, BaseDirPath, Webbasedir, Scheduler, Terminal, EnvironmentUsers);
        });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Thows_When_BaseDirPath_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new EnvironmentInfo(
            Name, Template, Machine, Webmachines, Terminalmachine, DatabaseMachine, null, Webbasedir, Scheduler, Terminal, EnvironmentUsers);
        });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Thows_When_Webbasedir_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new EnvironmentInfo(
            Name, Template, Machine, Webmachines, Terminalmachine, DatabaseMachine, BaseDirPath, null, Scheduler, Terminal, EnvironmentUsers);
        });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Thows_When_Scheduler_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new EnvironmentInfo(
            Name, Template, Machine, Webmachines, Terminalmachine, DatabaseMachine, BaseDirPath, Webbasedir, null, Terminal, EnvironmentUsers);
        });
    }

    [Test]
    public void Test_EnvironmentInfoTests_Thows_When_Terminal_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new EnvironmentInfo(
            Name, Template, Machine, Webmachines, Terminalmachine, DatabaseMachine, BaseDirPath, Webbasedir, Scheduler, null, EnvironmentUsers);
        });
    }

    [Test]
    public void Test_GetAppServerNetworkPath_Throws_When_path_null()
    {
      var envInfo = new EnvironmentInfo(
            Name, Template, Machine, Webmachines, Terminalmachine, DatabaseMachine, BaseDirPath, Webbasedir, Scheduler, Terminal, EnvironmentUsers);

      Assert.Throws<ArgumentException>(
        () => envInfo.GetAppServerNetworkPath(null));
    }

    [Test]
    public void Test_GetAppServerNetworkPath_Throws_When_path_startswithbackslashes()
    {
      var envInfo = new EnvironmentInfo(
            Name, Template, Machine, Webmachines, Terminalmachine, DatabaseMachine, BaseDirPath, Webbasedir, Scheduler, Terminal, EnvironmentUsers);

      Assert.Throws<ArgumentException>(
        () => envInfo.GetAppServerNetworkPath(@"\\kasjdkasdj"));
    }

    [Test]
    public void Test_GetAppServerNetworkPath_Throws_When_path_doesntstartwithdriveletter()
    {
      var envInfo = new EnvironmentInfo(
            Name, Template, Machine, Webmachines, Terminalmachine, DatabaseMachine, BaseDirPath, Webbasedir, Scheduler, Terminal, EnvironmentUsers);

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
          Webmachines,
          Terminalmachine,
          DatabaseMachine,
          BaseDirPath,
          Webbasedir,
          Scheduler,
          Terminal,
          EnvironmentUsers);

      Assert.AreEqual("\\\\" + Webmachines[0] + "\\c$\\", envInfo.GetWebServerNetworkPath(Webmachines[0], "c:\\"));
    }
  }
}
