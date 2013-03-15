using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.ScheduledTasks;
using UberDeployer.Core.Tests.Generators;

namespace UberDeployer.Core.Tests.Deployment
{
  [TestFixture]
  public class DeploySchedulerAppDeploymentTaskTests
  {
    private DeploySchedulerAppDeploymentTask _deployTask;
    private Mock<IEnvironmentInfoRepository> _environmentInfoRepositoryFake;
    private Mock<IArtifactsRepository> _artifactsRepositoryFake;
    private Mock<ITaskScheduler> _taskSchedulerFake;
    private Mock<IPasswordCollector> _passwordCollectorFake;

    [SetUp]
    public void SetUp()
    {
      _environmentInfoRepositoryFake = new Mock<IEnvironmentInfoRepository>();
      _artifactsRepositoryFake = new Mock<IArtifactsRepository>();
      _taskSchedulerFake = new Mock<ITaskScheduler>();
      _passwordCollectorFake = new Mock<IPasswordCollector>();

      _environmentInfoRepositoryFake
        .Setup(x => x.GetByName(It.IsAny<string>()))
        .Returns(DeploymentDataGenerator.GetEnvironmentInfo());

      _deployTask = new DeploySchedulerAppDeploymentTask(
        _environmentInfoRepositoryFake.Object,
        _artifactsRepositoryFake.Object,
        _taskSchedulerFake.Object,
        _passwordCollectorFake.Object);
    }

    [Test]
    public void Prepare_should_fail_if_task_is_running()
    {
      // arrange  
      DeploymentInfo deploymentInfo = DeploymentInfoGenerator.GetSchedulerAppDeploymentInfo();
      ScheduledTaskDetails runningTaskDetails = GetTaskDetails(true);

      _taskSchedulerFake
        .Setup(x => x.GetScheduledTaskDetails(It.IsAny<string>(), It.IsAny<string>()))
        .Returns(runningTaskDetails);

      // act assert
      Assert.Throws<DeploymentTaskException>(() => _deployTask.Prepare(deploymentInfo));
    }

    private ScheduledTaskDetails GetTaskDetails(bool isRunning)
    {
      return new ScheduledTaskDetails("taskName", isRunning, DateTime.Now, DateTime.Now, "exeAbsolutePath", null, null, 0);
    }
  }
}
