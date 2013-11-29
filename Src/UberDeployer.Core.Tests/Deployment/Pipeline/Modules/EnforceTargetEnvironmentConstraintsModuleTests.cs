using System;
using Moq;
using NUnit.Framework;
using UberDeployer.Common.IO;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Deployment.Pipeline;
using UberDeployer.Core.Deployment.Pipeline.Modules;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Domain.Input;
using UberDeployer.Core.Tests.Generators;

namespace UberDeployer.Core.Tests.Deployment.Pipeline.Modules
{
  [TestFixture]
  public class EnforceTargetEnvironmentConstraintsModuleTests
  {
    private Mock<IEnvironmentInfoRepository> _environmentInfoRepositoryFake;
    private Mock<IArtifactsRepository> _artifactsRepositoryFake;
    private Mock<IProjectInfoRepository> _projectsInfoRepositoryFake;
    private Mock<IDirectoryAdapter> _directoryAdapterFake;
    private Mock<IFileAdapter> _fileAdapterFake;
    private Mock<IZipFileAdapter> _zipFileAdapterFake;
    private DeploymentContext _deploymentContext;
    private DeployTerminalAppDeploymentTask _deploymentTask;

    private EnforceTargetEnvironmentConstraintsModule _enforceTargetEnvironmentConstraintsModule;

    [SetUp]
    public void SetUp()
    {
      _environmentInfoRepositoryFake = new Mock<IEnvironmentInfoRepository>();
      _artifactsRepositoryFake = new Mock<IArtifactsRepository>();
      _projectsInfoRepositoryFake = new Mock<IProjectInfoRepository>();
      _directoryAdapterFake = new Mock<IDirectoryAdapter>(MockBehavior.Loose);
      _fileAdapterFake = new Mock<IFileAdapter>(MockBehavior.Loose);
      _zipFileAdapterFake = new Mock<IZipFileAdapter>(MockBehavior.Loose);
      _deploymentContext = new DeploymentContext("requester");

      _deploymentTask =
        new DeployTerminalAppDeploymentTask(
          _projectsInfoRepositoryFake.Object,
          _environmentInfoRepositoryFake.Object,
          _artifactsRepositoryFake.Object,
          _directoryAdapterFake.Object,
          _fileAdapterFake.Object,
          _zipFileAdapterFake.Object);

      _enforceTargetEnvironmentConstraintsModule = new EnforceTargetEnvironmentConstraintsModule();
    }

    [Test]
    public void OnDeploymentTaskStarting_WhenEnvironmentIsProductionAndConfigurationIsNot_ThrowsInvalidOperationException()
    {
      DeploymentInfo deploymentInfo =
        new DeploymentInfo(
          Guid.NewGuid(),
          false,
          "project_name",
          "branch",
          "project_configuration_build_id",
          EnforceTargetEnvironmentConstraintsModule.ProductionEnvironmentName,
          new TerminalAppInputParams());

      Assert.Throws<InvalidOperationException>(
        () => _enforceTargetEnvironmentConstraintsModule.OnDeploymentTaskStarting(deploymentInfo, _deploymentTask, _deploymentContext));
    }

    [Test]
    public void OnDeploymentTaskStarting_WhenEnvironmentAndConfigurationIsProduction_DoesNotThrows()
    {
      DeploymentInfo deploymentInfo = DeploymentInfoGenerator.GetTerminalAppDeploymentInfo();

      _environmentInfoRepositoryFake
        .Setup(x => x.FindByName(It.IsAny<string>()))
        .Returns(DeploymentDataGenerator.GetEnvironmentInfo());

      _projectsInfoRepositoryFake
        .Setup(pir => pir.FindByName(deploymentInfo.ProjectName))
        .Returns(DeploymentDataGenerator.GetTerminalAppProjectInfo());

      _deploymentTask.Initialize(deploymentInfo);
      _deploymentTask.Prepare();

      Assert.DoesNotThrow(
        () => _enforceTargetEnvironmentConstraintsModule.OnDeploymentTaskStarting(deploymentInfo, _deploymentTask, _deploymentContext));
    }

    [Test]
    public void OnDeploymentTaskFinished_DoNothing_SoDoesNotThrows()
    {
      DeploymentInfo deploymentInfo = DeploymentInfoGenerator.GetTerminalAppDeploymentInfo();

      Assert.DoesNotThrow(
        () => _enforceTargetEnvironmentConstraintsModule.OnDeploymentTaskFinished(deploymentInfo, _deploymentTask, _deploymentContext));
    }
  }
}
