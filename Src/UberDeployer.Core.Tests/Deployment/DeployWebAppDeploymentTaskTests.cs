using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Domain.Input;
using UberDeployer.Core.Management.Iis;
using UberDeployer.Core.Management.MsDeploy;
using UberDeployer.Core.Tests.Generators;

namespace UberDeployer.Core.Tests.Deployment
{
  [TestFixture]
  public class DeployWebAppDeploymentTaskTests
  {
    // SUT
    private DeployWebAppDeploymentTask _deployWebAppDeploymentTask;

    private Mock<IMsDeploy> _msDeploy;
    private Mock<IProjectInfoRepository> _projectInfoRepositoryFake;
    private Mock<IEnvironmentInfoRepository> _environmentInfoRepositoryFake;
    private Mock<IArtifactsRepository> _artifactsRepository;
    private Mock<IIisManager> _iisManager;

    private WebAppProjectInfo _projectInfo;
    private EnvironmentInfo _environmentInfo;

    [SetUp]
    public virtual void SetUp()
    {
      _msDeploy = new Mock<IMsDeploy>();
      _artifactsRepository = new Mock<IArtifactsRepository>();
      _projectInfoRepositoryFake = new Mock<IProjectInfoRepository>(MockBehavior.Loose);
      _environmentInfoRepositoryFake = new Mock<IEnvironmentInfoRepository>();
      _iisManager = new Mock<IIisManager>();

      _projectInfo = ProjectInfoGenerator.GetWebAppProjectInfo();
      _environmentInfo = DeploymentDataGenerator.GetEnvironmentInfo();

      _deployWebAppDeploymentTask =
        new DeployWebAppDeploymentTask(
          _projectInfoRepositoryFake.Object,
          _environmentInfoRepositoryFake.Object,
          _msDeploy.Object,
          _artifactsRepository.Object, _iisManager.Object);

      _deployWebAppDeploymentTask.Initialize(DeploymentInfoGenerator.GetWebAppDeploymentInfo());

      _projectInfoRepositoryFake.Setup(x => x.FindByName(It.IsAny<string>()))
        .Returns(_projectInfo);

      _environmentInfoRepositoryFake.Setup(x => x.FindByName(It.IsAny<string>()))
        .Returns(_environmentInfo);
    }

    [Test]
    [TestCaseSource("GetInvalidWebMachineNames")]
    public void Prepare_should_throw_exception_when_web_machines_are_invalid(List<string> webMachines)
    {
      // Arrange
      var webInputParams =
        new WebAppInputParams(webMachines);

      DeploymentInfo deploymentInfo =
        new DeploymentInfo(
          false,
          "projectName",
          "projectConfigurationName",
          "projectConfigurationBuildId",
          "targetEnvironmentName",
          webInputParams);

      _deployWebAppDeploymentTask.Initialize(deploymentInfo);

      // Act assert
      Assert.Throws<DeploymentTaskException>(() => _deployWebAppDeploymentTask.Prepare());
    }

    [Test]
    public void Prepare_should_create_subTask_AppPoolDeploymentStep_when_app_pool_does_not_exist()
    {
      // Arrange
      _iisManager.Setup(x => x.AppPoolExists(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

      // Act
      _deployWebAppDeploymentTask.Prepare();

      // Assert
      Assert.IsTrue(_deployWebAppDeploymentTask.SubTasks.Any(st => st is CreateAppPoolDeploymentStep));
    }

    // ReSharper disable UnusedMethodReturnValue.Local

    private IEnumerable<List<string>> GetInvalidWebMachineNames()
    {
      yield return new List<string>();
      yield return new List<string> { "incorrectWebmachine" };
    }

    // ReSharper restore UnusedMethodReturnValue.Local
  }
}
