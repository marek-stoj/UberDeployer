using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.ScheduledTasks;

namespace UberDeployer.Core.Tests.Domain
{
  // TODO IMM HI: formatting; code style
  [TestFixture]
  public class SchedulerAppProjectInfoTests
  {
    private const int ExecutionTimeLimitInMinutes = 1;
    private const string Name = "name";
    private const string ArtifactsRepositoryName = "repoName";
    private const string ArtifactsRepositoryDirName = "repoDirName";
    private const string SchedulerAppName = "appName";
    private const string SchedulerAppDirName = "appDirName";
    private const string SchedulerAppExeName = "appExeName";
    private const string SchedulerAppUserId = "appUser";
    private const int ScheduledHour = 1;
    private const int ScheduledMinute = 1;

    private static readonly List<EnvironmentUser> _EnvironmentUsers =
      new List<EnvironmentUser>
        {
          new EnvironmentUser("Sample.User", "some_user@centrala.kaczmarski.pl"),
        };

    [Test]
    public void Test_SchedulerAppProjectInfoTests_Throws_When_Name_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new SchedulerAppProjectInfo(
            null,
            ArtifactsRepositoryName,
            ArtifactsRepositoryDirName,
            SchedulerAppName,
            SchedulerAppDirName,
            SchedulerAppExeName,
            SchedulerAppUserId,
            ScheduledHour,
            ScheduledMinute,
            ExecutionTimeLimitInMinutes);
        });
    }

    [Test]
    public void Test_SchedulerAppProjectInfoTests_Throws_When_ArtifactsRepositoryName_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new SchedulerAppProjectInfo(
            Name,
            null,
            ArtifactsRepositoryDirName,
            SchedulerAppName,
            SchedulerAppDirName,
            SchedulerAppExeName,
            SchedulerAppUserId,
            ScheduledHour,
            ScheduledMinute,
            ExecutionTimeLimitInMinutes);
        });
    }

    [Test]
    public void Test_SchedulerAppProjectInfoTests_Throws_When_ScheduledHour_LessThan0()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new SchedulerAppProjectInfo(
            Name,
            ArtifactsRepositoryName,
            ArtifactsRepositoryDirName,
            SchedulerAppName,
            SchedulerAppDirName,
            SchedulerAppExeName,
            SchedulerAppUserId,
            -1,
            ScheduledMinute,
            ExecutionTimeLimitInMinutes);
        });
    }

    [Test]
    public void Test_SchedulerAppProjectInfoTests_Throws_When_ScheduledHour_GreaterThan23()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new SchedulerAppProjectInfo(
            Name,
            ArtifactsRepositoryName,
            ArtifactsRepositoryDirName,
            SchedulerAppName,
            SchedulerAppDirName,
            SchedulerAppExeName,
            SchedulerAppUserId,
            24,
            ScheduledMinute,
            ExecutionTimeLimitInMinutes);
        });
    }

    [Test]
    public void Test_SchedulerAppProjectInfoTests_Throws_When_ScheduledMinute_GreaterThan59()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new SchedulerAppProjectInfo(
            Name,
            ArtifactsRepositoryName,
            ArtifactsRepositoryDirName,
            SchedulerAppName,
            SchedulerAppDirName,
            SchedulerAppExeName,
            SchedulerAppUserId,
            ScheduledHour,
            60,
            ExecutionTimeLimitInMinutes);
        });
    }

    [Test]
    public void Test_SchedulerAppProjectInfoTests_Throws_When_ScheduledMinute_LessThan0()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new SchedulerAppProjectInfo(
            Name,
            ArtifactsRepositoryName,
            ArtifactsRepositoryDirName,
            SchedulerAppName,
            SchedulerAppDirName,
            SchedulerAppExeName,
            SchedulerAppUserId,
            ScheduledHour,
            -1,
            ExecutionTimeLimitInMinutes);
        });
    }

    [Test]
    public void Test_SchedulerAppProjectInfoTests_Throws_When_ExecutionTime_LessThan0()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new SchedulerAppProjectInfo(
            Name,
            ArtifactsRepositoryName,
            ArtifactsRepositoryDirName,
            SchedulerAppName,
            SchedulerAppDirName,
            SchedulerAppExeName,
            SchedulerAppUserId,
            ScheduledHour,
            ScheduledMinute,
            -1);
        });
    }

    [Test]
    public void Test_SchedulerAppProjectInfoTests_Throws_When_SchedulerAppName_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new SchedulerAppProjectInfo(
            Name,
            ArtifactsRepositoryName,
            ArtifactsRepositoryDirName,
            null,
            SchedulerAppDirName,
            SchedulerAppExeName,
            SchedulerAppUserId,
            ScheduledHour,
            ScheduledMinute,
            ExecutionTimeLimitInMinutes);
        });
    }

    [Test]
    public void Test_SchedulerAppProjectInfoTests_Throws_When_SchedulerAppDirName_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new SchedulerAppProjectInfo(
            Name,
            ArtifactsRepositoryName,
            ArtifactsRepositoryDirName,
            SchedulerAppName,
            null,
            SchedulerAppExeName,
            SchedulerAppUserId,
            ScheduledHour,
            ScheduledMinute,
            ExecutionTimeLimitInMinutes);
        });
    }

    [Test]
    public void Test_SchedulerAppProjectInfoTests_Throws_When_SchedulerAppExeName_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new SchedulerAppProjectInfo(
            Name,
            ArtifactsRepositoryName,
            ArtifactsRepositoryDirName,
            SchedulerAppName,
            SchedulerAppDirName,
            null,
            SchedulerAppUserId,
            ScheduledHour,
            ScheduledMinute,
            ExecutionTimeLimitInMinutes);
        });
    }

    [Test]
    public void Test_CreateDeployemntTask_Throws_When_ObjectFactory_null()
    {
      var schedulerAppProjectInfo = new SchedulerAppProjectInfo(
            Name,
            ArtifactsRepositoryName,
            ArtifactsRepositoryDirName,
            SchedulerAppName,
            SchedulerAppDirName,
            SchedulerAppExeName,
            SchedulerAppUserId,
            ScheduledHour,
            ScheduledMinute,
            ExecutionTimeLimitInMinutes);

      Assert.Throws<ArgumentNullException>(
        () => schedulerAppProjectInfo.CreateDeploymentTask(
          null, "configName", "buildID", "targetEnvironmentName"));
    }

    [Test]
    public void Test_CreateDeployemntTask_RunsProperly_WhenAllIsWell()
    {
      var objectFactory = new Mock<IObjectFactory>(MockBehavior.Strict);
      var envInfoRepository = new Mock<IEnvironmentInfoRepository>(MockBehavior.Loose);
      var artifactsRepository = new Mock<IArtifactsRepository>(MockBehavior.Loose);
      var taskScheduler = new Mock<ITaskScheduler>(MockBehavior.Loose);
      var passwordCollector = new Mock<IPasswordCollector>(MockBehavior.Loose);

      var schedulerAppProjectInfo = new SchedulerAppProjectInfo(
            Name,
            ArtifactsRepositoryName,
            ArtifactsRepositoryDirName,
            SchedulerAppName,
            SchedulerAppDirName,
            SchedulerAppExeName,
            SchedulerAppUserId,
            ScheduledHour,
            ScheduledMinute,
            ExecutionTimeLimitInMinutes);

      objectFactory.Setup(o => o.CreateEnvironmentInfoRepository()).Returns(envInfoRepository.Object);
      objectFactory.Setup(o => o.CreateArtifactsRepository()).Returns(artifactsRepository.Object);
      objectFactory.Setup(o => o.CreateTaskScheduler()).Returns(taskScheduler.Object);
      objectFactory.Setup(o => o.CreatePasswordCollector()).Returns(passwordCollector.Object);

      schedulerAppProjectInfo.CreateDeploymentTask(
          objectFactory.Object, "configName", "buildID", "targetEnvironmentName");
    }

    [Test]
    public void Test_GetTargetFolde_RunsProperly_WhenAllIsWell()
    {
      string machine = Environment.MachineName;
      const string baseDirPath = "c:\\basedir";
      var envInfo = new EnvironmentInfo(
        "name", "templates", machine, "webmachine", "terminalmachine", "databasemachine", baseDirPath, "webbasedir", "c:\\scheduler", "terminal", _EnvironmentUsers);

      var schedulerAppProjectInfo = new SchedulerAppProjectInfo(
            Name,
            ArtifactsRepositoryName,
            ArtifactsRepositoryDirName,
            SchedulerAppName,
            SchedulerAppDirName,
            SchedulerAppExeName,
            SchedulerAppUserId,
            ScheduledHour,
            ScheduledMinute,
            ExecutionTimeLimitInMinutes);

      var result = schedulerAppProjectInfo.GetTargetFolder(envInfo);
      
      Assert.AreEqual("\\\\" + machine + "\\c$\\scheduler\\" + SchedulerAppDirName, result);
    }

    [Test]
    public void Test_GetTargetFolde_Throws_EnvInfo_null()
    {
      var schedulerAppProjectInfo = new SchedulerAppProjectInfo(
            Name,
            ArtifactsRepositoryName,
            ArtifactsRepositoryDirName,
            SchedulerAppName,
            SchedulerAppDirName,
            SchedulerAppExeName,
            SchedulerAppUserId,
            ScheduledHour,
            ScheduledMinute,
            ExecutionTimeLimitInMinutes);

      Assert.Throws<ArgumentNullException>(() => schedulerAppProjectInfo.GetTargetFolder(null));
    }
  }
}
