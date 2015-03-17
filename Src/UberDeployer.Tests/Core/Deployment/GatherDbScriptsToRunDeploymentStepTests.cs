using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Moq;

using NUnit.Framework;

using UberDeployer.Core.Deployment;
using UberDeployer.Core.Deployment.Steps;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Db;
using UberDeployer.Tests.Core.Generators;

namespace UberDeployer.Tests.Core.Deployment
{
  [TestFixture]
  public class GatherDbScriptsToRunDeploymentStepTests
  {
    private GatherDbScriptsToRunDeploymentStep _deploymentStep;

    private const string _ScriptPath = "Core/TestData/TestSqlScripts";

    private const string _SqlServerName = "sqlServerName";

    private const string _Environment = "env";

    private Mock<IDbVersionProvider> _dbVersionProviderFake;

    private Mock<IScriptsToRunWebSelector> _scriptsToRunWebSelector;

    [SetUp]
    public void SetUp()
    {
      _dbVersionProviderFake = new Mock<IDbVersionProvider>(MockBehavior.Loose);
      _scriptsToRunWebSelector = new Mock<IScriptsToRunWebSelector>(MockBehavior.Loose);

      DbProjectInfo dbProjectInfo = ProjectInfoGenerator.GetDbProjectInfo();

      DeploymentInfo di = DeploymentInfoGenerator.GetDbDeploymentInfo();

      _deploymentStep = new GatherDbScriptsToRunDeploymentStep(
        dbProjectInfo.DbName,
        new Lazy<string>(() => _ScriptPath),
        _SqlServerName,
        _Environment,
        di,
        _dbVersionProviderFake.Object,
        _scriptsToRunWebSelector.Object);
    }

    [Test]
    public void Description_is_not_empty()
    {
      _deploymentStep.Prepare();

      Assert.IsNotNullOrEmpty(_deploymentStep.Description);
    }

    [Test]
    public void DoExecute_calls_DbVersionProvider()
    {
      // arrange
      _dbVersionProviderFake
        .Setup(x => x.GetVersions(It.IsAny<string>(), It.IsAny<string>())).
        Returns(
          new List<DbVersionInfo>
          {
            new DbVersionInfo()
            {
              Version = "1.2",
              IsMigrated = true
            },
            new DbVersionInfo
            {
              Version = "1.3",
              IsMigrated = true
            }
          });

      _scriptsToRunWebSelector.Setup(x => x.GetSelectedScriptsToRun(It.IsAny<Guid>(), It.IsAny<string[]>())).Returns(
        new DbScriptsToRunSelection
        {
          DatabaseScriptToRunSelectionType = DatabaseScriptToRunSelectionType.LastVersion,
          SelectedScripts = new[] { "1.4" }
        });

      // act
      _deploymentStep.PrepareAndExecute();

      // assert
      _dbVersionProviderFake.VerifyAll();
    }

    [Test]
    public void DoExecute_gathers_not_executed_scripts()
    {
      // arrange
      const string notExecutedScript = "1.4.sql";

      _dbVersionProviderFake
        .Setup(x => x.GetVersions(It.IsAny<string>(), It.IsAny<string>())).
        Returns(
          new List<DbVersionInfo>
          {
            new DbVersionInfo()
            {
              Version = "1.2",
              IsMigrated = true
            },
            new DbVersionInfo
            {
              Version = "1.3",
              IsMigrated = true
            }
          });

      _scriptsToRunWebSelector.Setup(x => x.GetSelectedScriptsToRun(It.IsAny<Guid>(), It.IsAny<string[]>())).Returns(
        new DbScriptsToRunSelection
        {
          DatabaseScriptToRunSelectionType = DatabaseScriptToRunSelectionType.LastVersion,
          SelectedScripts = new[] { "1.4" }
        });

      // act
      _deploymentStep.PrepareAndExecute();

      // assert
      Assert.IsTrue(_deploymentStep.ScriptsToRun.Any(x => Path.GetFileName(x.ScriptPath) == notExecutedScript));
    }

    [Test]
    public void DoExecute_not_gathers_older_scripts_than_current()
    {
      // arrange
      const string scriptOlderThanCurrent = "1.3.sql";

      _scriptsToRunWebSelector.Setup(x => x.GetSelectedScriptsToRun(It.IsAny<Guid>(), It.IsAny<string[]>())).Returns(
        new DbScriptsToRunSelection
        {
          DatabaseScriptToRunSelectionType = DatabaseScriptToRunSelectionType.LastVersion,
          SelectedScripts = new[] { "1.2" }
        });

      // act
      _deploymentStep.PrepareAndExecute();

      // assert
      Assert.IsFalse(_deploymentStep.ScriptsToRun.Any(x => Path.GetFileName(x.ScriptPath) == scriptOlderThanCurrent));
    }

    [Test]
    public void DoExecute_not_gathers_scripts_with_not_supported_name()
    {
      // arrange
      const string notSupportedScript = "1.3a.sql";

      _scriptsToRunWebSelector.Setup(x => x.GetSelectedScriptsToRun(It.IsAny<Guid>(), It.IsAny<string[]>())).Returns(
        new DbScriptsToRunSelection
        {
          DatabaseScriptToRunSelectionType = DatabaseScriptToRunSelectionType.LastVersion,
          SelectedScripts = new[] { "1.2" }
        });

      // act
      _deploymentStep.PrepareAndExecute();

      // assert
      Assert.IsFalse(_deploymentStep.ScriptsToRun.Any(x => Path.GetFileName(x.ScriptPath) == notSupportedScript));
    }

    [Test]
    public void DoExecute_gathers_scripts_marked_as_non_transactional()
    {
      // arrange
      const string nonTransactionalScriptToExecute = "1.3.notrans.sql";
      
      _scriptsToRunWebSelector.Setup(x => x.GetSelectedScriptsToRun(It.IsAny<Guid>(), It.IsAny<string[]>())).Returns(
        new DbScriptsToRunSelection
        {
          DatabaseScriptToRunSelectionType = DatabaseScriptToRunSelectionType.LastVersion,
          SelectedScripts = new[] { "1.4" }
        });

      // act
      _deploymentStep.PrepareAndExecute();

      // assert
      Assert.IsTrue(_deploymentStep.ScriptsToRun.Any(x => Path.GetFileName(x.ScriptPath) == nonTransactionalScriptToExecute));
    }
  }
}