using System;
using NUnit.Framework;
using UberDeployer.Core.Management.Db;

namespace UberDeployer.Core.Tests.Deployment
{
  [TestFixture]
  public class MsSqlDbScriptRunnerTests
  {
    [Test]
    public void Ctor_checks_its_arguments()
    {
      Assert.Throws<ArgumentException>(() => new MsSqlDbScriptRunner(null));
    }

    [Test]
    public void Execute_checks_its_arguments()
    {
      var msSqlDbScriptRunner = new MsSqlDbScriptRunner("dbserver");

      Assert.Throws<ArgumentException>(() => msSqlDbScriptRunner.Execute(null));
    }
  }
}
