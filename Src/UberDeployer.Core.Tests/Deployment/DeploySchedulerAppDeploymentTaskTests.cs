using System;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using UberDeployer.Common.IO;
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
    private Mock<IProjectInfoRepository> _projectInfoRepositoryFake;
    private Mock<IEnvironmentInfoRepository> _environmentInfoRepositoryFake;
    private Mock<IArtifactsRepository> _artifactsRepositoryFake;
    private Mock<ITaskScheduler> _taskSchedulerFake;
    private Mock<IPasswordCollector> _passwordCollectorFake;
    private Mock<IDirectoryAdapter> _directoryAdapterFake;

    private EnvironmentInfo _environmentInfo;
    private SchedulerAppProjectInfo _projectInfo;

    [SetUp]
    public void SetUp()
    {
      _projectInfoRepositoryFake = new Mock<IProjectInfoRepository>();
      _environmentInfoRepositoryFake = new Mock<IEnvironmentInfoRepository>();
      _artifactsRepositoryFake = new Mock<IArtifactsRepository>();
      _taskSchedulerFake = new Mock<ITaskScheduler>();
      _passwordCollectorFake = new Mock<IPasswordCollector>();
      _directoryAdapterFake = new Mock<IDirectoryAdapter>();

      _projectInfo = ProjectInfoGenerator.GetSchedulerAppProjectInfo();

      SchedulerAppTask schedulerAppTask1 = _projectInfo.SchedulerTasks[0];
      SchedulerAppTask schedulerAppTask2 = _projectInfo.SchedulerTasks[1];

      _environmentInfo =
        DeploymentDataGenerator.GetEnvironmentInfo(
          new[]
          {
            new EnvironmentUser(schedulerAppTask1.UserId, "user_name_1"),
            new EnvironmentUser(schedulerAppTask2.UserId, "user_name_2"),
          });

      _projectInfoRepositoryFake.Setup(pir => pir.FindByName(It.IsAny<string>()))
        .Returns(_projectInfo);

      ScheduledTaskDetails taskDetails1 =
        GetTaskDetails(
          schedulerAppTask1,
          Path.Combine(
            _environmentInfo.SchedulerAppsBaseDirPath,
            _projectInfo.SchedulerAppDirName,
            schedulerAppTask1.ExecutableName),
          false);

      _taskSchedulerFake
        .Setup(x => x.GetScheduledTaskDetails(It.IsAny<string>(), schedulerAppTask1.Name))
        .Returns(taskDetails1);

      ScheduledTaskDetails taskDetails2 =
        GetTaskDetails(
          schedulerAppTask2,
          Path.Combine(
            _environmentInfo.SchedulerAppsBaseDirPath,
            _projectInfo.SchedulerAppDirName,
            schedulerAppTask2.ExecutableName),
          false);

      _taskSchedulerFake
        .Setup(x => x.GetScheduledTaskDetails(It.IsAny<string>(), schedulerAppTask2.Name))
        .Returns(taskDetails2);

      _environmentInfoRepositoryFake
        .Setup(x => x.FindByName(It.IsAny<string>()))
        .Returns(_environmentInfo);

      _directoryAdapterFake
        .Setup(x => x.Exists(It.IsAny<string>()))
        .Returns(true);

      _deployTask =
        new DeploySchedulerAppDeploymentTask(
          _projectInfoRepositoryFake.Object,
          _environmentInfoRepositoryFake.Object,
          _artifactsRepositoryFake.Object,
          _taskSchedulerFake.Object,
          _passwordCollectorFake.Object,
          _directoryAdapterFake.Object);

      _deployTask.Initialize(DeploymentInfoGenerator.GetSchedulerAppDeploymentInfo());
    }

    [Test]
    public void Prepare_should_fail_if_task_is_running()
    {
      // arrange  
      SchedulerAppTask schedulerAppTask =
        _projectInfo.SchedulerTasks[1];

      ScheduledTaskDetails runningTaskDetails =
        GetTaskDetails(
          schedulerAppTask,
          Path.Combine(
            _environmentInfo.SchedulerAppsBaseDirPath,
            _projectInfo.SchedulerAppDirName,
            schedulerAppTask.ExecutableName),
          true);

      _taskSchedulerFake
        .Setup(x => x.GetScheduledTaskDetails(It.IsAny<string>(), It.IsAny<string>()))
        .Returns(runningTaskDetails);

      // act assert
      Assert.Throws<DeploymentTaskException>(() => _deployTask.Prepare());
    }

    [Test]
    public void Prepare_should_not_create_any_backups_when_target_directory_doesnt_exist()
    {
      // arrange  
      _directoryAdapterFake
        .Setup(x => x.Exists(It.IsAny<string>()))
        .Returns(false);

      // act
      _deployTask.Prepare();

      // assert
      Assert.IsFalse(_deployTask.SubTasks.Any(t => t is BackupFilesDeploymentStep));
    }

    [Test]
    public void Prepare_should_add_step_to_disable_scheduled_task_before_copy_files_step()
    {
      // act
      _deployTask.Prepare();

      // assert
      AssertStepIsBefore(typeof(ToggleSchedulerAppEnabledStep), typeof(CopyFilesDeploymentStep), _deployTask.SubTasks.ToArray());

      var disableTask = _deployTask.SubTasks.First(x => x is ToggleSchedulerAppEnabledStep) as ToggleSchedulerAppEnabledStep;
      Assert.IsNotNull(disableTask);
      Assert.IsFalse(disableTask.Enabled);
    }

    [Test]
    public void Prepare_should_add_step_to_enable_scheduled_task_as_the_last_one()
    {
      // act
      _deployTask.Prepare();

      // assert
      DeploymentTaskBase lastTask = _deployTask.SubTasks.Last();
      Assert.IsInstanceOf<ToggleSchedulerAppEnabledStep>(lastTask);
      Assert.IsTrue(((ToggleSchedulerAppEnabledStep)lastTask).Enabled);
    }

    [Test]
    public void Prepare_should_collect_password_when_task_settings_has_changed()
    {
      // arrange  
      const string exePath = "exe path has changed";
      ScheduledTaskDetails runningTaskDetails = GetTaskDetails(_projectInfo.SchedulerTasks[0], exePath, false);

      _taskSchedulerFake
        .Setup(x => x.GetScheduledTaskDetails(It.IsAny<string>(), It.IsAny<string>()))
        .Returns(runningTaskDetails);

      _passwordCollectorFake
        .Setup(x => x.CollectPasswordForUser(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .Returns("password");

      // act
      _deployTask.Prepare();

      // assert
      _passwordCollectorFake.VerifyAll();
    }

    [Test]
    public void Prepare_should_collect_password_when_task_doesnt_exist()
    {
      // arrange  
      _taskSchedulerFake
        .Setup(x => x.GetScheduledTaskDetails(It.IsAny<string>(), It.IsAny<string>()))
        .Returns((ScheduledTaskDetails)null);

      _passwordCollectorFake
        .Setup(x => x.CollectPasswordForUser(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .Returns("password");

      // act
      _deployTask.Prepare();

      // assert
      _passwordCollectorFake.VerifyAll();
    }

    [Test]
    public void Prepare_should_add_step_to_update_scheduled_task_when_settings_has_changed()
    {
      // arrange
      const string exePath = "exe path has changed";
      ScheduledTaskDetails runningTaskDetails = GetTaskDetails(_projectInfo.SchedulerTasks[0], exePath, false);

      _taskSchedulerFake
        .Setup(x => x.GetScheduledTaskDetails(It.IsAny<string>(), It.IsAny<string>()))
        .Returns(runningTaskDetails);

      _passwordCollectorFake
        .Setup(x => x.CollectPasswordForUser(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .Returns("password");

      // act
      _deployTask.Prepare();

      // assert
      Assert.IsTrue(_deployTask.SubTasks.Any(x => x is UpdateSchedulerTaskDeploymentStep));
    }

    [Test]
    public void Prepare_should_add_step_to_schedule_new_app_when_task_doesnt_exist()
    {
      // arrange  
      _taskSchedulerFake
        .Setup(x => x.GetScheduledTaskDetails(It.IsAny<string>(), It.IsAny<string>()))
        .Returns((ScheduledTaskDetails)null);

      _passwordCollectorFake
        .Setup(x => x.CollectPasswordForUser(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .Returns("password");

      // act
      _deployTask.Prepare();

      // assert
      Assert.IsTrue(_deployTask.SubTasks.Any(x => x is CreateSchedulerTaskDeploymentStep));
    }

    [Test]
    public void Prepare_should_add_step_to_do_a_backup()
    {
      // act
      _deployTask.Prepare();

      // assert      
      Assert.IsTrue(_deployTask.SubTasks.Any(x => x is BackupFilesDeploymentStep));
    }

    [Test]
    public void Prepare_should_add_step_to_copy_application_files()
    {
      // act
      _deployTask.Prepare();

      // assert
      Assert.IsTrue(_deployTask.SubTasks.Any(x => x is CopyFilesDeploymentStep));
    }

    private static ScheduledTaskDetails GetTaskDetails(SchedulerAppTask schedulerAppTask, string exeAbsolutePath, bool isRunning)
    {
      return
      new ScheduledTaskDetails(
        schedulerAppTask.Name,
        isRunning,
        DateTime.Now,
        DateTime.Now,
        exeAbsolutePath,
        schedulerAppTask.ScheduledHour,
        schedulerAppTask.ScheduledMinute,
        schedulerAppTask.ExecutionTimeLimitInMinutes);
    }

    private static void AssertStepIsBefore(Type stepBeforeType, Type stepAfterType, DeploymentTaskBase[] subTasks)
    {
      int i = 0;
      while (i < subTasks.Length && subTasks[i].GetType() != stepBeforeType)
      {
        i++;
      }

      Assert.Less(i, subTasks.Length);

      while (i < subTasks.Length && subTasks[i].GetType() != stepAfterType)
      {
        i++;
      }

      Assert.Less(i, subTasks.Length);
    }
  }
}
