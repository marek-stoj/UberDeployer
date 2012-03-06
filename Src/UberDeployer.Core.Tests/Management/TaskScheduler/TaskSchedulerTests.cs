using System;
using NUnit.Framework;
using UberDeployer.Core.Management.ScheduledTasks;
using UberDeployerTaskScheduler = UberDeployer.Core.Management.ScheduledTasks.TaskScheduler;

namespace UberDeployer.Core.Tests.Management.TaskScheduler
{
  [TestFixture]
  public class TaskSchedulerTests
  {
    private const string _UserName = "some_user";
    private const string _Password = "some_password";

    [Test]
    public void ScheduleTask_WhenMachineNameIsNullOrEmpty_ThrowsArgumentException()
    {
      // Arrange
      var taskScheduler = new UberDeployerTaskScheduler();

      // Act & Assert
      Assert.Throws<ArgumentException>(() => taskScheduler.ScheduleNewTask(string.Empty, null, _UserName, _Password));
    }

    [Test]
    public void ScheduleTask_WhenScheduledTaskSpecificationIsNull_ThrowsArgumentException()
    {
      // Arrange
      var taskScheduler = new UberDeployerTaskScheduler();

      // Act & Assert
      Assert.Throws<ArgumentNullException>(() => taskScheduler.ScheduleNewTask("machineName", null, _UserName, _Password));
    }

    [Test]
    public void UpdateTaskSchedule_WhenMachineNameIsNullOrEmpty_ThrowsArgumentException()
    {
      // Arrange
      var taskScheduler = new UberDeployerTaskScheduler();

      // Act & Assert
      Assert.Throws<ArgumentException>(() => taskScheduler.UpdateTaskSchedule(string.Empty, new ScheduledTaskSpecification("task", "C:\\task.exe", 10, 0, 1), _UserName, _Password));
    }

    [Test]
    public void UpdateTaskSchedule_WhenTaskNameIsNullOrEmpty_ThrowsArgumentException()
    {
      // Arrange
      var taskScheduler = new UberDeployerTaskScheduler();

      // Act & Assert
      Assert.Throws<ArgumentNullException>(() => taskScheduler.UpdateTaskSchedule("machineName", null, _UserName, _Password));
    }
  }
}
