using System;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Deployment.Pipeline;
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
      TerminalAppProjectInfo projectInfo = new TerminalAppProjectInfo("name", "artifactsRepositoryName", "artifactsrepositoryDirName", false, "terminalAppName", "terminalAppDirName", "terminalAppExeName");
      var deploymentTask = new DeployTerminalAppDeploymentTask(environmentInfoRepository.Object, artifactsRepository.Object);
      var deploymentContext = new DeploymentContext("requester");

      Assert.Throws<InvalidOperationException>(() => enforceTargetEnvironmentConstraintsModule.OnDeploymentTaskStarting(deploymentTask, deploymentContext));
    }

    [Test]
    public void OnDeploymentTaskStarting_WhenEnvironmentAndConfigurationIsProduction_DoesNotThrows()
    {
      var enforceTargetEnvironmentConstraintsModule = new EnforceTargetEnvironmentConstraintsModule();
      var environmentInfoRepository = new Mock<IEnvironmentInfoRepository>();
      var artifactsRepository = new Mock<IArtifactsRepository>();
      TerminalAppProjectInfo projectInfo = new TerminalAppProjectInfo("name", "artifactsRepositoryName", "artifactsrepositoryDirName", false, "terminalAppName", "terminalAppDirName", "terminalAppExeName");
      var deploymentTask = new DeployTerminalAppDeploymentTask(environmentInfoRepository.Object, artifactsRepository.Object);
      var deploymentContext = new DeploymentContext("requester");

      Assert.DoesNotThrow(() => enforceTargetEnvironmentConstraintsModule.OnDeploymentTaskStarting(deploymentTask, deploymentContext));
    }

    [Test]
    public void OnDeploymentTaskFinished_DoNothing_SoDoesNotThrows()
    {
      var enforceTargetEnvironmentConstraintsModule = new EnforceTargetEnvironmentConstraintsModule();
      var environmentInfoRepository = new Mock<IEnvironmentInfoRepository>();
      var artifactsRepository = new Mock<IArtifactsRepository>();
      TerminalAppProjectInfo projectInfo = new TerminalAppProjectInfo("name", "artifactsRepositoryName", "artifactsrepositoryDirName", false, "terminalAppName", "terminalAppDirName", "terminalAppExeName");
      var deploymentTask = new DeployTerminalAppDeploymentTask(environmentInfoRepository.Object, artifactsRepository.Object);
      var deploymentContext = new DeploymentContext("requester");

      Assert.DoesNotThrow(() => enforceTargetEnvironmentConstraintsModule.OnDeploymentTaskFinished(deploymentTask, deploymentContext));
    }
  }
}
