using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Db;
using UberDeployer.Core.Tests.Generators;
using UberDeployer.Core.Tests.TestUtils;

namespace UberDeployer.Core.Tests.Deployment
{
  [TestFixture]
  public class MigrateDbDeploymentTaskTests
  {
    private Mock<IProjectInfoRepository> _projectInfoRepositoryFake;
    private Mock<IEnvironmentInfoRepository> _environmentInfoRepositoryFake;
    private Mock<IArtifactsRepository> _artifactsRepositoryFake;
    private Mock<IDbScriptRunnerFactory> _dbScriptRunnerFactoryFake;
    private Mock<IDbVersionProvider> _dbVersionProviderFake;

    private MigrateDbDeploymentTask _deploymentTask;

    [SetUp]
    public void SetUp()
    {
      _projectInfoRepositoryFake = new Mock<IProjectInfoRepository>(MockBehavior.Loose);
      _environmentInfoRepositoryFake = new Mock<IEnvironmentInfoRepository>(MockBehavior.Loose);
      _artifactsRepositoryFake = new Mock<IArtifactsRepository>(MockBehavior.Loose);
      _dbScriptRunnerFactoryFake = new Mock<IDbScriptRunnerFactory>(MockBehavior.Loose);
      _dbVersionProviderFake = new Mock<IDbVersionProvider>(MockBehavior.Loose);

      _projectInfoRepositoryFake
        .Setup(x => x.FindByName(It.IsAny<string>()))
        .Returns(ProjectInfoGenerator.GetDbProjectInfo());

      _environmentInfoRepositoryFake
        .Setup(x => x.FindByName(It.IsAny<string>()))
        .Returns(DeploymentDataGenerator.GetEnvironmentInfo);

      _dbScriptRunnerFactoryFake
        .Setup(x => x.CreateDbScriptRunner(It.IsAny<string>()))
        .Returns(new Mock<IDbScriptRunner>(MockBehavior.Loose).Object);

      _deploymentTask =
        new MigrateDbDeploymentTask(
          _projectInfoRepositoryFake.Object,
          _environmentInfoRepositoryFake.Object,
          _artifactsRepositoryFake.Object,
          _dbScriptRunnerFactoryFake.Object,
          _dbVersionProviderFake.Object);

      _deploymentTask.Initialize(DeploymentInfoGenerator.GetDbDeploymentInfo());
    }

    [Test]
    [TestCase("environmentInfoRepository", typeof(ArgumentNullException))]
    [TestCase("artifactsRepository", typeof(ArgumentNullException))]
    [TestCase("dbScriptRunnerFactory", typeof(ArgumentNullException))]
    [TestCase("dbVersionProvider", typeof(ArgumentNullException))]    
    public void Constructor_fails_when_parameter_is_null(string nullParamName, Type expectedExceptionType)
    {
      Assert.Throws(
        expectedExceptionType,
        () => ReflectionTestTools.CreateInstance<MigrateDbDeploymentTask>(GetConstructorDefaultParams(), nullParamName));
    }

    [Test]
    public void Description_is_not_empty()
    {
      _deploymentTask.Prepare();

      Assert.IsNotNullOrEmpty(_deploymentTask.Description);
    }    

    [Test]
    public void DoPrepare_calls_environment_info_repository()
    {
      // act
      _deploymentTask.Prepare();

      // assert
      _environmentInfoRepositoryFake.VerifyAll();
    }

    [Test]
    public void DoPrepare_fails_when_environment_info_repository_returns_null()
    {
      // act
      _environmentInfoRepositoryFake
        .Setup(x => x.FindByName(It.IsAny<string>()))
        .Returns((EnvironmentInfo)null);

      // assert
      Assert.Throws<DeploymentTaskException>(() => _deploymentTask.Prepare());
    }

    [Test]
    public void DoPrepare_calls_db_script_runner_factory()
    {
      // act
      _deploymentTask.Prepare();

      // assert
      _dbScriptRunnerFactoryFake.VerifyAll();
    }

    [Test]
    public void DoPrepare_fails_when_db_script_runner_factory_returns_null_script_runner()
    {
      // act
      _dbScriptRunnerFactoryFake
        .Setup(x => x.CreateDbScriptRunner(It.IsAny<string>()))
        .Returns((IDbScriptRunner)null);

      // assert
      Assert.Throws<DeploymentTaskException>(() => _deploymentTask.Prepare());
    }

    [Test]
    [TestCase(typeof(DownloadArtifactsDeploymentStep))]
    [TestCase(typeof(ExtractArtifactsDeploymentStep))]
    [TestCase(typeof(GatherDbScriptsToRunDeploymentStep))]
    [TestCase(typeof(RunDbScriptsDeploymentStep))]
    public void DoPrepare_adds_deployment_step(Type deploymentStepType)
    {
      // act
      _deploymentTask.Prepare();

      // assert
      Assert.IsNotNull(_deploymentTask.SubTasks.Any(x => x.GetType() == deploymentStepType));
    }

    [Test]
    public void DoPrepare_adds_steps_in_appropriate_order()
    {
      // arrange
      Type[] stepsTypesOrder =
        new[]
          {
            typeof(DownloadArtifactsDeploymentStep),
            typeof(ExtractArtifactsDeploymentStep),
            typeof(GatherDbScriptsToRunDeploymentStep),
            typeof(RunDbScriptsDeploymentStep)
          };

      // act
      _deploymentTask.Prepare();

      // assert
      int prevStepIndex = GetIndexOfTaskWithType(_deploymentTask.SubTasks, stepsTypesOrder[0]);

      for (int i = 1; i < stepsTypesOrder.Length; i++)
      {
        int nextStepIndex = GetIndexOfTaskWithType(_deploymentTask.SubTasks, stepsTypesOrder[i]);

        Assert.IsTrue(nextStepIndex > prevStepIndex);

        prevStepIndex = nextStepIndex;
      }
    }

    #region Private helper methods

    private static int GetIndexOfTaskWithType(IEnumerable<DeploymentTaskBase> deploymentTasks, Type taskType)
    {
      int i = 0;

      foreach (var deploymentTask in deploymentTasks)
      {
        if (deploymentTask.GetType() == taskType)
        {
          return i;
        }

        i++;
      }

      return -1;
    }

    private OrderedDictionary GetConstructorDefaultParams()
    {
      return
        new OrderedDictionary
          {
            { "projectInfoRepository", _projectInfoRepositoryFake.Object },
            { "environmentInfoRepository", _environmentInfoRepositoryFake.Object },
            { "artifactsRepository", _artifactsRepositoryFake.Object },
            { "dbScriptRunnerFactory", _dbScriptRunnerFactoryFake.Object },
            { "dbVersionProvider", _dbVersionProviderFake.Object },            
          };
    }

    #endregion
  }
}
