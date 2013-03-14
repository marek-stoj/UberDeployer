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
    private Mock<IEnvironmentInfoRepository> _environmentInfoRepository;
    private Mock<IArtifactsRepository> _artifactsRepository;
    private Mock<IIisManager> _iisManager;

    private readonly EnvironmentInfo _environmentInfo = DeploymentDataGenerator.GetEnvironmentInfo();

    [SetUp]
    public virtual void SetUp()
    {
      _msDeploy = new Mock<IMsDeploy>();
      _artifactsRepository = new Mock<IArtifactsRepository>();
      _environmentInfoRepository = new Mock<IEnvironmentInfoRepository>();
      _iisManager = new Mock<IIisManager>();

      _deployWebAppDeploymentTask = new DeployWebAppDeploymentTask(_msDeploy.Object, _environmentInfoRepository.Object,
        _artifactsRepository.Object, _iisManager.Object);

      _environmentInfoRepository.Setup(x => x.GetByName(It.IsAny<string>()))
        .Returns(_environmentInfo);

    }

    [Test]
    [TestCaseSource("GetInvalidWebMachineNames")]
    public void Prepare_should_throw_exception_when_web_machines_are_invalid(List<string> webMachines)
    {
      // Arrange
      ProjectInfo projectInfo = ProjectInfoGenerator.GetWebAppProjectInfo();

      var webInputParams =
        new WebAppInputParams(webMachines);

      DeploymentInfo deploymentInfo = new DeploymentInfo("projectName", "projectConfigurationName", "projectConfigurationBuildId", "targetEnvironmentName",
        projectInfo, webInputParams);

      // Act assert
      Assert.Throws<DeploymentTaskException>(() => _deployWebAppDeploymentTask.Prepare(deploymentInfo));
    }

    [Test]
    public void Prepare_should_add_subtasks_properly()
    {
      // Arrange
      ProjectInfo projectInfo = ProjectInfoGenerator.GetWebAppProjectInfo();

      var webInputParams =
        new WebAppInputParams(
          new[] { _environmentInfo.WebServerMachineNames.First() });

      DeploymentInfo deploymentInfo = new DeploymentInfo("projectName", "projectConfigurationName", "projectConfigurationBuildId", "targetEnvironmentName",
        projectInfo, webInputParams);


      // Act
      _deployWebAppDeploymentTask.Prepare(deploymentInfo);

      // Assert
      Assert.IsTrue(_deployWebAppDeploymentTask.SubTasks.Any(st => st is DownloadArtifactsDeploymentStep));
      //todo marta: asserts for other steps ...
    }

    [Test]
    public void Prepare_should_create_subTask_AppPoolDeploymentStep_when_app_pool_does_not_exist()
    {
      // Arrange
      _iisManager.Setup(x => x.AppPoolExists(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
      var deploymentInfo = CreateDeploymentInfo(new List<string> { _environmentInfo.WebServerMachineNames.First() });

      // Act
      _deployWebAppDeploymentTask.Prepare(deploymentInfo);

      // Assert
      Assert.IsTrue(_deployWebAppDeploymentTask.SubTasks.Any(st => st is CreateAppPoolDeploymentStep));
    }

    private DeploymentInfo CreateDeploymentInfo(List<string> webMachines)
    {
      ProjectInfo projectInfo = ProjectInfoGenerator.GetWebAppProjectInfo();

      var webInputParams =
        new WebAppInputParams(webMachines);

      return
        new DeploymentInfo(
          "projectName",
          "projectConfigurationName",
          "projectConfigurationBuildId",
          "targetEnvironmentName",
          projectInfo,
          webInputParams);
    }

    // ReSharper disable UnusedMethodReturnValue.Local

    private IEnumerable<List<string>> GetInvalidWebMachineNames()
    {
      yield return null;
      yield return new List<string>();
      yield return new List<string> { "incorrectWebmachine" };
    }

    // ReSharper restore UnusedMethodReturnValue.Local
  }
}
