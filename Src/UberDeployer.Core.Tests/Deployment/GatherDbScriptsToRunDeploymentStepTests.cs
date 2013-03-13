using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Db;
using System.Linq;
using UberDeployer.Core.Tests.Generators;
using UberDeployer.Core.Tests.TestUtils;

namespace UberDeployer.Core.Tests.Deployment
{
  [TestFixture]
  public class GatherDbScriptsToRunDeploymentStepTests
  {
    private GatherDbScriptsToRunDeploymentStep _deploymentStep;
    private const string _ScriptPath = "TestData/TestSqlScripts";
    private const string _SqlServerName = "sqlServerName";
    private const string _Environment = "env";

    private Mock<IDbVersionProvider> _dbVersionProviderFake;
    private DeploymentInfo _deploymentInfo;

    [SetUp]
    public void SetUp()
    {
      _dbVersionProviderFake = new Mock<IDbVersionProvider>(MockBehavior.Loose);
      _deploymentStep = new GatherDbScriptsToRunDeploymentStep(new Lazy<string>(() => _ScriptPath), _SqlServerName, _Environment, _dbVersionProviderFake.Object);
      _deploymentInfo = DeploymentInfoGenerator.GetDbDeploymentInfo();
    }

    [Test]
    [TestCase("scriptsDirectoryPath", typeof(ArgumentNullException))]
    [TestCase("sqlServerName", typeof(ArgumentException))]
    [TestCase("environmentName", typeof(ArgumentException))]
    [TestCase("dbVersionProvider", typeof(ArgumentNullException))]
    public void Constructor_fails_when_parameter_is_null(string nullParamName, Type expectedExceptionType)
    {
      Assert.Throws(
        expectedExceptionType,
        () => ReflectionTestTools.CreateInstance<GatherDbScriptsToRunDeploymentStep>(GetDefaultConstructorParams(), nullParamName));
    }
    
    [Test]
    public void Description_is_not_empty()
    {      
      _deploymentStep.Prepare(_deploymentInfo);

      Assert.IsNotNullOrEmpty(_deploymentStep.Description);
    }

    [Test]
    public void DoExecute_calls_DbVersionProvider()
    {
      // arrange
      _dbVersionProviderFake
        .Setup(x => x.GetVersions(It.IsAny<string>(), It.IsAny<string>())).
        Returns(new List<string>() { "1.2", "1.3" });

      // act
      _deploymentStep.PrepareAndExecute(_deploymentInfo);

      // assert
      _dbVersionProviderFake.VerifyAll();
    }

    [Test]
    public void DoExecute_gathers_not_executed_scripts()
    {
      // arrange
      string[] executedScriptsVersion = new[] { "1.2", "1.3" };
      const string notExecutedScript = "1.4.sql";

      _dbVersionProviderFake
        .Setup(x => x.GetVersions(It.IsAny<string>(), It.IsAny<string>())).
        Returns(executedScriptsVersion);

      // act
      _deploymentStep.PrepareAndExecute(_deploymentInfo);

      // assert
      Assert.AreEqual(1, _deploymentStep.ScriptsToRun.Count());
      Assert.IsTrue(_deploymentStep.ScriptsToRun.Any(x => Path.GetFileName(x) == notExecutedScript));
    }

    [Test]
    public void DoExecute_not_gathers_older_scripts_than_current()
    {
      // arrange
      string[] executedScriptsVersion = new[] { "1.3" };
      const string scriptOlderThanCurrent = "1.2.sql";

      _dbVersionProviderFake
        .Setup(x => x.GetVersions(It.IsAny<string>(), It.IsAny<string>())).
        Returns(executedScriptsVersion);

      // act
      _deploymentStep.PrepareAndExecute(_deploymentInfo);

      // assert
      Assert.IsFalse(_deploymentStep.ScriptsToRun.Any(x => Path.GetFileName(x) == scriptOlderThanCurrent));
    }

    [Test]
    public void DoExecute_not_gathers_scripts_with_not_supported_name()
    {
      // arrange
      string[] executedScriptsVersion = new[] { "1.2" };
      const string notSupportedScript = "1.3a.sql";

      _dbVersionProviderFake
        .Setup(x => x.GetVersions(It.IsAny<string>(), It.IsAny<string>())).
        Returns(executedScriptsVersion);

      // act
      _deploymentStep.PrepareAndExecute(_deploymentInfo);

      // assert
      Assert.IsFalse(_deploymentStep.ScriptsToRun.Any(x => Path.GetFileName(x) == notSupportedScript));
    }

    private OrderedDictionary GetDefaultConstructorParams()
    {
      return
        new OrderedDictionary
          {
            { "scriptsDirectoryPath", new Lazy<string>(() => _ScriptPath) },
            { "sqlServerName", _SqlServerName },
            { "environmentName", _Environment },
            { "dbVersionProvider", _dbVersionProviderFake.Object }
          };
    }
  }
}
