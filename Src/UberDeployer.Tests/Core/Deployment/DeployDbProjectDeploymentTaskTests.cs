using System;
using System.Collections.Generic;
using System.Linq;

using Moq;

using NUnit.Framework;

using UberDeployer.Common.IO;
using UberDeployer.Core.Deployment.Steps;
using UberDeployer.Core.Deployment.Tasks;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Db;
using UberDeployer.Tests.Core.Generators;

namespace UberDeployer.Tests.Core.Deployment
{
  [TestFixture]
  public class DeployDbProjectDeploymentTaskTests
  {
    private Mock<IProjectInfoRepository> _projectInfoRepositoryFake;
    private Mock<IEnvironmentInfoRepository> _environmentInfoRepositoryFake;
    private Mock<IArtifactsRepository> _artifactsRepositoryFake;
    private Mock<IDbScriptRunnerFactory> _dbScriptRunnerFactoryFake;
    private Mock<IDbVersionProvider> _dbVersionProviderFake;
    private Mock<IFileAdapter> _fileAdapterFake;
    private Mock<IZipFileAdapter> _zipFileAdapterFake;
    private Mock<IScriptsToRunWebSelector> _scriptsToRunWebSelectorFake;

    private DeployDbProjectDeploymentTask _deploymentTask;

    [SetUp]
    public void SetUp()
    {
      _projectInfoRepositoryFake = new Mock<IProjectInfoRepository>(MockBehavior.Loose);
      _environmentInfoRepositoryFake = new Mock<IEnvironmentInfoRepository>(MockBehavior.Loose);
      _artifactsRepositoryFake = new Mock<IArtifactsRepository>(MockBehavior.Loose);
      _dbScriptRunnerFactoryFake = new Mock<IDbScriptRunnerFactory>(MockBehavior.Loose);
      _dbVersionProviderFake = new Mock<IDbVersionProvider>(MockBehavior.Loose);
      _fileAdapterFake = new Mock<IFileAdapter>();
      _zipFileAdapterFake = new Mock<IZipFileAdapter>();
      _scriptsToRunWebSelectorFake = new Mock<IScriptsToRunWebSelector>();

      _projectInfoRepositoryFake
        .Setup(x => x.FindByName(It.IsAny<string>()))
        .Returns(ProjectInfoGenerator.GetDbProjectInfo());

      _environmentInfoRepositoryFake
        .Setup(x => x.FindByName(It.IsAny<string>()))
        .Returns(DeploymentDataGenerator.GetEnvironmentInfo);

      _dbScriptRunnerFactoryFake
        .Setup(x => x.CreateDbScriptRunner(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>()))
        .Returns(new Mock<IDbScriptRunner>(MockBehavior.Loose).Object);

      _deploymentTask =
        new DeployDbProjectDeploymentTask(
          _projectInfoRepositoryFake.Object,
          _environmentInfoRepositoryFake.Object,
          _artifactsRepositoryFake.Object,
          _dbScriptRunnerFactoryFake.Object,
          _dbVersionProviderFake.Object,
          _fileAdapterFake.Object,
          _zipFileAdapterFake.Object,
          _scriptsToRunWebSelectorFake.Object);

      _deploymentTask.Initialize(DeploymentInfoGenerator.GetDbDeploymentInfo());
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
        .Setup(x => x.CreateDbScriptRunner(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>()))
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
  }
}