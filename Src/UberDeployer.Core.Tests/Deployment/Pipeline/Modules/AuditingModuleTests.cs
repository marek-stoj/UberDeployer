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
  public class AuditingModuleTests
  {
    [Test]
    public void Constructor_WhenDeploymentRequestRepositoryIsNull_ThrowsArgumentNullException()
    {
      Assert.Throws<ArgumentNullException>(() => new AuditingModule(null));
    }

    [Test]
    public void OnDeploymentTaskStarting_DoNothing_SoDoesNotThrow()
    {
      var deploymentRequestRepository = new Mock<IDeploymentRequestRepository>();
      var auditingModule = new AuditingModule(deploymentRequestRepository.Object);

      var environmentInfoRepository = new Mock<IEnvironmentInfoRepository>();
      var artifactsRepository = new Mock<IArtifactsRepository>();
      TerminalAppProjectInfo projectInfo = new TerminalAppProjectInfo("name", "artifactsRepositoryName", "artifactsrepositoryDirName", false, "terminalAppName", "terminalAppDirName", "terminalAppExeName");
      var deploymentTask = new DeployTerminalAppDeploymentTask(environmentInfoRepository.Object, artifactsRepository.Object, projectInfo, "Production", "buildId", "prod");
      var deploymentContext = new DeploymentContext("requester");

      Assert.DoesNotThrow(() => auditingModule.OnDeploymentTaskStarting(deploymentTask, deploymentContext));
    }

    [Test]
    public void OnDeploymentTaskFinished_ExpectAddDeploymnetRequest()
    {
      DateTime dateStarted = DateTime.UtcNow;
      DateTime dateFinished = DateTime.UtcNow;
      string projectName = "projectName";
      string targetEnvironmentName = "targetEnvironmentName";
      var deploymentRequestRepository = new Mock<IDeploymentRequestRepository>(MockBehavior.Strict);
      var auditingModule = new AuditingModule(deploymentRequestRepository.Object);

      var environmentInfoRepository = new Mock<IEnvironmentInfoRepository>();
      var artifactsRepository = new Mock<IArtifactsRepository>();
      TerminalAppProjectInfo projectInfo = new TerminalAppProjectInfo("name", "artifactsRepositoryName", "artifactsrepositoryDirName", false, "terminalAppName", "terminalAppDirName", "terminalAppExeName");
      var deploymentTask = new DeployTerminalAppDeploymentTask(environmentInfoRepository.Object, artifactsRepository.Object, projectInfo, "Production", "buildId", "prod");
      var deploymentContext = new DeploymentContext("requester");

      deploymentRequestRepository
        .Setup(
          drr =>
          drr.AddDeploymentRequest(
            It.Is<DeploymentRequest>(
              r =>
              r.DateStarted == dateStarted
              && r.DateFinished == dateFinished
              && r.TargetEnvironmentName == targetEnvironmentName
              && r.FinishedSuccessfully
              && r.ProjectName == projectName)));

      Assert.DoesNotThrow(() => auditingModule.OnDeploymentTaskFinished(deploymentTask, deploymentContext));
    }
  }
}
