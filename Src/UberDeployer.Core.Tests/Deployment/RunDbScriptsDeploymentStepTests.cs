using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Management.Db;

namespace UberDeployer.Core.Tests.Deployment
{
  [TestFixture]
  public class RunDbScriptsDeploymentStepTests
  {
    private const string _DatabaseServerName = "server_name";

    private static readonly IEnumerable<string> _ScriptsToRun =
      new List<string>
        {
          "TestData/TestSqlScripts/1.3.sql",
          "TestData/TestSqlScripts/1.4.sql"
        };

    private Mock<IDbScriptRunner> _dbScriptRunnerFake;
    private RunDbScriptsDeploymentStep _deploymentStep;

    [SetUp]
    public void SetUp()
    {
      _dbScriptRunnerFake = new Mock<IDbScriptRunner>(MockBehavior.Loose);
      _deploymentStep = new RunDbScriptsDeploymentStep(_dbScriptRunnerFake.Object, _DatabaseServerName, _ScriptsToRun);
    }

    [Test]
    public void Constructor_checks_arguments()
    {
      Assert.Throws<ArgumentNullException>(() => new RunDbScriptsDeploymentStep(null, _DatabaseServerName, _ScriptsToRun));
      Assert.Throws<ArgumentException>(() => new RunDbScriptsDeploymentStep(_dbScriptRunnerFake.Object, null, _ScriptsToRun));
      Assert.Throws<ArgumentNullException>(() => new RunDbScriptsDeploymentStep(_dbScriptRunnerFake.Object, _DatabaseServerName, null));
    }

    [Test]
    public void Description_is_not_empty()
    {
      Assert.IsNotNullOrEmpty(_deploymentStep.Description);
    }

    [Test]
    public void DoExecute_calls_script_runner_for_each_script()
    {
      // act
      _deploymentStep.PrepareAndExecute();

      // assert
      _dbScriptRunnerFake.Verify(
        x => x.Execute(It.IsAny<string>()),
        Times.Exactly(_ScriptsToRun.Count()));
    }

    [Test]
    public void DoExecute_fails_when_script_runner_fails()
    {
      // arrange
      _dbScriptRunnerFake
        .Setup(x => x.Execute(It.IsAny<string>()))
        .Throws(new DbScriptRunnerException(It.IsAny<string>(), new Exception("message")));

      // act, assert
      Assert.Throws<DeploymentTaskException>(() => _deploymentStep.PrepareAndExecute());
    }

    [Test]
    public void DoExecute_fails_on_not_existing_script()
    {
      // arrange
      IEnumerable<string> notExistingScripts = new List<string>() { "someScript.sql" };

      _deploymentStep = new RunDbScriptsDeploymentStep(_dbScriptRunnerFake.Object, _DatabaseServerName, notExistingScripts);

      // act, assert
      Assert.Throws<FileNotFoundException>(() => _deploymentStep.PrepareAndExecute());
    }
  }
}
