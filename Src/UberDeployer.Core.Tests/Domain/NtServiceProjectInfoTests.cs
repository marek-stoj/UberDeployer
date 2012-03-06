using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.NtServices;
using UberDeployer.Core.Management.ScheduledTasks;

namespace UberDeployer.Core.Tests.Domain
{
  // TODO IMM HI: formatting; code style
  [TestFixture]
  public class NtServiceProjectInfoTests
  {
    private const string Name = "name";
    private const string ArtifactsRepositoryName = "artifRepoName";
    private const string ArtifactsRepositoryDirName = "artifRepoDirName";
    private const string NtServiceName = "ntServiceName";
    private const string NtServiceDirName = "ntServiceDirName";
    private const string NtServiceDisplayName = "ntServiceDisplayName";
    private const string NtServiceExeName = "ntServiceExeName";
    private const string NtServiceUserId = "Sample.User";

    private static readonly List<EnvironmentUser> _EnvironmentUsers =
      new List<EnvironmentUser>
        {
          new EnvironmentUser("Sample.User", "some_user@centrala.kaczmarski.pl"),
        };

    [Test]
    public void Test_NtServiceProjectInfo_Thows_When_Name_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
          {
            new NtServiceProjectInfo(
              null,
              ArtifactsRepositoryName,
              ArtifactsRepositoryDirName,
              NtServiceName,
              NtServiceDirName,
              NtServiceDisplayName,
              NtServiceExeName,
              NtServiceUserId);
          });
    }

    [Test]
    public void Test_NtServiceProjectInfo_Thows_When_ArtifactsRepoName_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
          {
            new NtServiceProjectInfo(
              Name,
              null,
              ArtifactsRepositoryDirName,
              NtServiceName,
              NtServiceDirName,
              NtServiceDisplayName,
              NtServiceExeName,
              NtServiceUserId);
          });
    }

    [Test]
    public void Test_NtServiceProjectInfo_Thows_When_NtServiceName_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
          {
            new NtServiceProjectInfo(
              Name,
              ArtifactsRepositoryName,
              ArtifactsRepositoryDirName,
              null,
              NtServiceDirName,
              NtServiceDisplayName,
              NtServiceExeName,
              NtServiceUserId);
          });
    }

    [Test]
    public void Test_NtServiceProjectInfo_Thows_When_NtServiceDirName_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new NtServiceProjectInfo(
          Name,
          ArtifactsRepositoryName,
          ArtifactsRepositoryDirName,
          NtServiceName,
          null,
          NtServiceDisplayName,
          NtServiceExeName,
          NtServiceUserId);
        });
    }

    [Test]
    public void Test_NtServiceProjectInfo_Thows_When_NtServiceExeName_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new NtServiceProjectInfo(
          Name,
          ArtifactsRepositoryName,
          ArtifactsRepositoryDirName,
          NtServiceName,
          NtServiceDirName,
          NtServiceDisplayName,
          null,
          NtServiceUserId);
        });
    }

    [Test]
    public void Test_CreateDeploymentTask_Thows_When_ObjectFactory_null()
    {
      var projectInfo = new NtServiceProjectInfo(
          Name,
          ArtifactsRepositoryName,
          ArtifactsRepositoryDirName,
          NtServiceName,
          NtServiceDirName,
          NtServiceDisplayName,
          NtServiceExeName,
          NtServiceUserId);

      Assert.Throws<ArgumentNullException>(
        () => projectInfo.CreateDeploymentTask(null, "configName", "buildID", "targetEnvironmentName"));
    }

    [Test]
    public void Test_CreateDeployemntTask_RunsProperly_WhenAllIsWell()
    {
      var objectFactory = new Mock<IObjectFactory>(MockBehavior.Strict);
      var envInfoRepository = new Mock<IEnvironmentInfoRepository>(MockBehavior.Loose);
      var artifactsRepository = new Mock<IArtifactsRepository>(MockBehavior.Loose);
      var taskScheduler = new Mock<ITaskScheduler>(MockBehavior.Loose);
      var ntServiceManager = new Mock<INtServiceManager>(MockBehavior.Loose);
      var passwordCollector = new Mock<IPasswordCollector>(MockBehavior.Loose);

      var projectInfo = new NtServiceProjectInfo(
          Name,
          ArtifactsRepositoryName,
          ArtifactsRepositoryDirName,
          NtServiceName,
          NtServiceDirName,
          NtServiceDisplayName,
          NtServiceExeName,
          NtServiceUserId);

      objectFactory.Setup(o => o.CreateEnvironmentInfoRepository()).Returns(envInfoRepository.Object);
      objectFactory.Setup(o => o.CreateArtifactsRepository()).Returns(artifactsRepository.Object);
      objectFactory.Setup(o => o.CreateTaskScheduler()).Returns(taskScheduler.Object);
      objectFactory.Setup(o => o.CreateNtServiceManager()).Returns(ntServiceManager.Object);
      objectFactory.Setup(o => o.CreatePasswordCollector()).Returns(passwordCollector.Object);

      projectInfo.CreateDeploymentTask(
          objectFactory.Object, "configName", "buildID", "targetEnvironmentName");
    }


    [Test]
    public void Test_GetTargetFolde_RunsProperly_WhenAllIsWell()
    {
      string machine = Environment.MachineName;
      const string baseDirPath = "c:\\basedir";
      var envInfo = new EnvironmentInfo(
        "name", "templates", machine, "webmachine", "terminalmachine", "databasemachine", baseDirPath, "webbasedir", "c:\\scheduler", "terminal", _EnvironmentUsers);

      var projectInfo = new NtServiceProjectInfo(
          Name,
          ArtifactsRepositoryName,
          ArtifactsRepositoryDirName,
          NtServiceName,
          NtServiceDirName,
          NtServiceDisplayName,
          NtServiceExeName,
          NtServiceUserId);

      var result = projectInfo.GetTargetFolder(envInfo);
      Assert.AreEqual("\\\\" + machine + "\\c$\\basedir\\" + NtServiceDirName, result);
    }

    [Test]
    public void Test_GetTargetFolde_Throws_EnvInfo_null()
    {
      var projectInfo = new NtServiceProjectInfo(
          Name,
          ArtifactsRepositoryName,
          ArtifactsRepositoryDirName,
          NtServiceName,
          NtServiceDirName,
          NtServiceDisplayName,
          NtServiceExeName,
          NtServiceUserId);

      Assert.Throws<ArgumentNullException>(() => projectInfo.GetTargetFolder(null));
    }

  }
}
