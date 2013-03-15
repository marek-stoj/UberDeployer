using System;
using Moq;
using NUnit.Framework;
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
    [Test]
    public void OnDeploymentTaskStarting_WhenEnvironmentIsProductionAndConfigurationIsNot_ThrowsInvalidOperationException()
    {
      var enforceTargetEnvironmentConstraintsModule = new EnforceTargetEnvironmentConstraintsModule();
      var environmentInfoRepository = new Mock<IEnvironmentInfoRepository>();
      var artifactsRepository = new Mock<IArtifactsRepository>();
      var projectsInfoRepository = new Mock<IProjectInfoRepository>();
      var deploymentTask = new DeployTerminalAppDeploymentTask(environmentInfoRepository.Object, artifactsRepository.Object, projectsInfoRepository.Object);
      var deploymentContext = new DeploymentContext("requester");

      DeploymentInfo deploymentInfo =
        new DeploymentInfo(
          "project_name",
          "branch",
          "project_configuration_build_id",
          EnforceTargetEnvironmentConstraintsModule.ProductionEnvironmentName,
          ProjectInfoGenerator.GetTerminalAppProjectInfo(),
          new TerminalAppInputParams());

      Assert.Throws<InvalidOperationException>(() => enforceTargetEnvironmentConstraintsModule.OnDeploymentTaskStarting(deploymentInfo, deploymentTask, deploymentContext));
    }

    [Test]
    public void OnDeploymentTaskStarting_WhenEnvironmentAndConfigurationIsProduction_DoesNotThrows()
    {
      var enforceTargetEnvironmentConstraintsModule = new EnforceTargetEnvironmentConstraintsModule();
      var environmentInfoRepository = new Mock<IEnvironmentInfoRepository>();
      var artifactsRepository = new Mock<IArtifactsRepository>();
      var projectsInfoRepository = new Mock<IProjectInfoRepository>();
      var deploymentTask = new DeployTerminalAppDeploymentTask(environmentInfoRepository.Object, artifactsRepository.Object, projectsInfoRepository.Object);
      var deploymentContext = new DeploymentContext("requester");
      DeploymentInfo deploymentInfo = DeploymentInfoGenerator.GetTerminalAppDeploymentInfo();

      environmentInfoRepository
        .Setup(x => x.GetByName(It.IsAny<string>()))
        .Returns(DeploymentDataGenerator.GetEnvironmentInfo());

      deploymentTask.Prepare(deploymentInfo);      

      Assert.DoesNotThrow(() => enforceTargetEnvironmentConstraintsModule.OnDeploymentTaskStarting(deploymentInfo, deploymentTask, deploymentContext));
    }

    [Test]
    public void OnDeploymentTaskFinished_DoNothing_SoDoesNotThrows()
    {
      var enforceTargetEnvironmentConstraintsModule = new EnforceTargetEnvironmentConstraintsModule();
      var environmentInfoRepository = new Mock<IEnvironmentInfoRepository>();
      var artifactsRepository = new Mock<IArtifactsRepository>();
      var projectsInfoRepository = new Mock<IProjectInfoRepository>();
      var projectInfo = new TerminalAppProjectInfo("name", "artifactsRepositoryName", "artifactsrepositoryDirName", false, "terminalAppName", "terminalAppDirName", "terminalAppExeName");
      var deploymentTask = new DeployTerminalAppDeploymentTask(environmentInfoRepository.Object, artifactsRepository.Object, projectsInfoRepository.Object);
      var deploymentContext = new DeploymentContext("requester");
      DeploymentInfo deploymentInfo = DeploymentInfoGenerator.GetTerminalAppDeploymentInfo();

      Assert.DoesNotThrow(() => enforceTargetEnvironmentConstraintsModule.OnDeploymentTaskFinished(deploymentInfo, deploymentTask, deploymentContext));
    }
  }
}
