using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.NtServices;
using UberDeployer.Core.Management.ScheduledTasks;

namespace UberDeployer.Core.Tests.Domain
{
  // TODO IMM HI: formatting; code style
  [TestFixture]
  public class TerminalAppProjectInfoTests
  {
    private const string _Name = "name";
    private const string _ArtifactsRepositoryName = "artifRepoName";
    private const string _ArtifactsRepositoryDirName = "artifRepoDirName";
    private const string _TerminalAppName = "terminalAppName";
    private const string _TerminalAppDirName = "terminalAppDirName";
    private const string _TerminalAppExeName = "terminalAppExeName";

    private static readonly List<EnvironmentUser> _EnvironmentUsers =
      new List<EnvironmentUser>
        {
          new EnvironmentUser("Sample.User", "some_user@centrala.kaczmarski.pl"),
        };

    [Test]
    public void Test_TerminalAppProjectInfo_Thows_When_Name_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
          {
            new TerminalAppProjectInfo(
            null,
            _ArtifactsRepositoryName,
            _ArtifactsRepositoryDirName,
            _TerminalAppName,
            _TerminalAppDirName,
            _TerminalAppExeName);
          });
    }

    [Test]
    public void Test_TerminalAppProjectInfo_Thows_When_ArtifactsRepositoryName_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new TerminalAppProjectInfo(
          _Name,
          null,
          _ArtifactsRepositoryDirName,
          _TerminalAppName,
          _TerminalAppDirName,
          _TerminalAppExeName);
        });
    }

    [Test]
    public void Test_TerminalAppProjectInfo_Thows_When_TerminalAppName_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new TerminalAppProjectInfo(
          _Name,
          _ArtifactsRepositoryName,
          _ArtifactsRepositoryDirName,
          null,
          _TerminalAppDirName,
          _TerminalAppExeName);
        });
    }

    [Test]
    public void Test_TerminalAppProjectInfo_Thows_When_TerminalAppDirName_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new TerminalAppProjectInfo(
          _Name,
          _ArtifactsRepositoryName,
          _ArtifactsRepositoryDirName,
          _TerminalAppName,
          null,
          _TerminalAppExeName);
        });
    }

    [Test]
    public void Test_TerminalAppProjectInfo_Thows_When_TerminalAppExeName_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new TerminalAppProjectInfo(
          _Name,
          _ArtifactsRepositoryName,
          _ArtifactsRepositoryDirName,
          _TerminalAppName,
          _TerminalAppDirName,
          null);
        });
    }

    [Test]
    public void Test_CreateDeployemntTask_Throws_WhenObjectFactory_null()
    {
      var projectInfo = new TerminalAppProjectInfo(
            _Name,
            _ArtifactsRepositoryName,
            _ArtifactsRepositoryDirName,
            _TerminalAppName,
            _TerminalAppDirName,
            _TerminalAppExeName);

      Assert.Throws<ArgumentNullException>(()=>projectInfo.CreateDeploymentTask(
          null, "configName", "buildID", "targetEnvironmentName"));
    }

    [Test]
    public void Test_CreateDeployemntTask_RunsProperly_WhenAllIsWell()
    {
      var objectFactory = new Mock<IObjectFactory>(MockBehavior.Strict);
      var envInfoRepository = new Mock<IEnvironmentInfoRepository>(MockBehavior.Strict);
      var artifactsRepository = new Mock<IArtifactsRepository>(MockBehavior.Strict);
      var taskScheduler = new Mock<ITaskScheduler>(MockBehavior.Strict);
      var ntServiceManager = new Mock<INtServiceManager>(MockBehavior.Strict);

      var projectInfo = new TerminalAppProjectInfo(
            _Name,
            _ArtifactsRepositoryName,
            _ArtifactsRepositoryDirName,
            _TerminalAppName,
            _TerminalAppDirName,
            _TerminalAppExeName);

      objectFactory.Setup(o => o.CreateEnvironmentInfoRepository()).Returns(envInfoRepository.Object);
      objectFactory.Setup(o => o.CreateArtifactsRepository()).Returns(artifactsRepository.Object);
      objectFactory.Setup(o => o.CreateTaskScheduler()).Returns(taskScheduler.Object);
      objectFactory.Setup(o => o.CreateNtServiceManager()).Returns(ntServiceManager.Object);

      projectInfo.CreateDeploymentTask(
          objectFactory.Object, "configName", "buildID", "targetEnvironmentName");
    }

    [Test]
    public void Test_GetTargetFolde_RunsProperly_WhenAllIsWell()
    {
      string machine = Environment.MachineName;
      const string baseDirPath = "c:\\basedir";
      string terminalmachine = "terminalmachine";
      var envInfo = new EnvironmentInfo(
        "name", "templates", machine, new[] { "webmachine" }, terminalmachine, "databasemachine", baseDirPath, "webbasedir", "c:\\scheduler", "c:\\terminal", _EnvironmentUsers);

      var projectInfo = new TerminalAppProjectInfo(
            _Name,
            _ArtifactsRepositoryName,
            _ArtifactsRepositoryDirName,
            _TerminalAppName,
            _TerminalAppDirName,
            _TerminalAppExeName);

      var result = projectInfo.GetTargetFolders(envInfo);
      Assert.AreEqual("\\\\" + terminalmachine + "\\c$\\terminal\\" + _TerminalAppDirName, result);
    }

    [Test]
    public void Test_GetTargetFolde_Throws_EnvInfo_null()
    {
      var projectInfo = new TerminalAppProjectInfo(
            _Name,
            _ArtifactsRepositoryName,
            _ArtifactsRepositoryDirName,
            _TerminalAppName,
            _TerminalAppDirName,
            _TerminalAppExeName);

      Assert.Throws<ArgumentNullException>(() => projectInfo.GetTargetFolders(null));
    }
  }
}
