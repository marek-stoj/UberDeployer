using System;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Deployment.Pipeline.Modules;
using UberDeployer.Core.Domain;

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
      TerminalAppProjectInfo projectInfo = new TerminalAppProjectInfo("name", "artifactsRepositoryName", "artifactsrepositoryDirName", "terminalAppName", "terminalAppDirName", "terminalAppExeName");
      var deploymentTask = new DeployTerminalAppDeploymentTask(environmentInfoRepository.Object, artifactsRepository.Object, projectInfo, "trunk", "buildId", "prod");

      Assert.Throws<InvalidOperationException>(() => enforceTargetEnvironmentConstraintsModule.OnDeploymentTaskStarting(deploymentTask));
    }

    [Test]
    public void OnDeploymentTaskStarting_WhenEnvironmentAndConfigurationIsProduction_DoesNotThrows()
    {
      var enforceTargetEnvironmentConstraintsModule = new EnforceTargetEnvironmentConstraintsModule();
      var environmentInfoRepository = new Mock<IEnvironmentInfoRepository>();
      var artifactsRepository = new Mock<IArtifactsRepository>();
      TerminalAppProjectInfo projectInfo = new TerminalAppProjectInfo("name", "artifactsRepositoryName", "artifactsrepositoryDirName", "terminalAppName", "terminalAppDirName", "terminalAppExeName");
      var deploymentTask = new DeployTerminalAppDeploymentTask(environmentInfoRepository.Object, artifactsRepository.Object, projectInfo, "Production", "buildId", "prod");

      Assert.DoesNotThrow(() => enforceTargetEnvironmentConstraintsModule.OnDeploymentTaskStarting(deploymentTask));
    }

    [Test]
    public void OnDeploymentTaskFinished_DoNothing_SoDoesNotThrows()
    {
      var enforceTargetEnvironmentConstraintsModule = new EnforceTargetEnvironmentConstraintsModule();
      var environmentInfoRepository = new Mock<IEnvironmentInfoRepository>();
      var artifactsRepository = new Mock<IArtifactsRepository>();
      TerminalAppProjectInfo projectInfo = new TerminalAppProjectInfo("name", "artifactsRepositoryName", "artifactsrepositoryDirName", "terminalAppName", "terminalAppDirName", "terminalAppExeName");
      var deploymentTask = new DeployTerminalAppDeploymentTask(environmentInfoRepository.Object, artifactsRepository.Object, projectInfo, "Production", "buildId", "prod");

      Assert.DoesNotThrow(() => enforceTargetEnvironmentConstraintsModule.OnDeploymentTaskFinished(deploymentTask, DateTime.Now, "Abbot", "Abbot", true));
    }
  }
}
