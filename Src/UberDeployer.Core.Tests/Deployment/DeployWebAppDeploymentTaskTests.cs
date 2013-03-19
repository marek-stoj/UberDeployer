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

    private EnvironmentInfo _environmentInfo;

    [SetUp]
    public virtual void SetUp()
    {
      _msDeploy = new Mock<IMsDeploy>();
      _artifactsRepository = new Mock<IArtifactsRepository>();
      _environmentInfoRepository = new Mock<IEnvironmentInfoRepository>();
      _iisManager = new Mock<IIisManager>();

      _environmentInfo = DeploymentDataGenerator.GetEnvironmentInfo();

      _deployWebAppDeploymentTask = new DeployWebAppDeploymentTask(_msDeploy.Object, _environmentInfoRepository.Object,
        _artifactsRepository.Object, _iisManager.Object);

      _environmentInfoRepository.Setup(x => x.FindByName(It.IsAny<string>()))
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
    public void Prepare_should_create_subTask_AppPoolDeploymentStep_when_app_pool_does_not_exist()
    {
      // Arrange
      _iisManager.Setup(x => x.AppPoolExists(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

      ProjectInfo projectInfo = ProjectInfoGenerator.GetWebAppProjectInfo();

      var webInputParams =
        new WebAppInputParams(new List<string> { _environmentInfo.WebServerMachineNames.First() });

      var deploymentInfo =
        new DeploymentInfo(
          projectInfo.Name,
          "projectConfigurationName",
          "projectConfigurationBuildId",
          "targetEnvironmentName",
          projectInfo,
          webInputParams);

      // Act
      _deployWebAppDeploymentTask.Prepare(deploymentInfo);

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
