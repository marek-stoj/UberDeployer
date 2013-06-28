using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using UberDeployer.Common.IO;
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
    private const bool _ArtifactsAreNotEnvirionmentSpecific = false;
    private const string _TerminalAppName = "terminalAppName";
    private const string _TerminalAppDirName = "terminalAppDirName";
    private const string _TerminalAppExeName = "terminalAppExeName";

    private Mock<IObjectFactory> _objectFactoryFake;
    private Mock<IDirectoryAdapter> _directoryAdapterFake;

    [SetUp]
    public void SetUp()
    {
      _objectFactoryFake = new Mock<IObjectFactory>(MockBehavior.Loose);

      _directoryAdapterFake = new Mock<IDirectoryAdapter>(MockBehavior.Loose);

      _objectFactoryFake.Setup(of => of.CreateDirectoryAdapter())
        .Returns(_directoryAdapterFake.Object);
    }

    [Test]
    public void Test_TerminalAppProjectInfo_Throws_When_Name_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
          {
            new TerminalAppProjectInfo(
            null,
            _ArtifactsRepositoryName,
            _ArtifactsRepositoryDirName,
            _ArtifactsAreNotEnvirionmentSpecific,
            _TerminalAppName,
            _TerminalAppDirName,
            _TerminalAppExeName);
          });
    }

    [Test]
    public void Test_TerminalAppProjectInfo_Throws_When_ArtifactsRepositoryName_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new TerminalAppProjectInfo(
          _Name,
          null,
          _ArtifactsRepositoryDirName,
          _ArtifactsAreNotEnvirionmentSpecific,
          _TerminalAppName,
          _TerminalAppDirName,
          _TerminalAppExeName);
        });
    }

    [Test]
    public void Test_TerminalAppProjectInfo_Throws_When_TerminalAppName_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new TerminalAppProjectInfo(
          _Name,
          _ArtifactsRepositoryName,
          _ArtifactsRepositoryDirName,
          _ArtifactsAreNotEnvirionmentSpecific,
          null,
          _TerminalAppDirName,
          _TerminalAppExeName);
        });
    }

    [Test]
    public void Test_TerminalAppProjectInfo_Throws_When_TerminalAppDirName_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new TerminalAppProjectInfo(
          _Name,
          _ArtifactsRepositoryName,
          _ArtifactsRepositoryDirName,
          _ArtifactsAreNotEnvirionmentSpecific,
          _TerminalAppName,
          null,
          _TerminalAppExeName);
        });
    }

    [Test]
    public void Test_TerminalAppProjectInfo_Throws_When_TerminalAppExeName_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
        {
          new TerminalAppProjectInfo(
          _Name,
          _ArtifactsRepositoryName,
          _ArtifactsRepositoryDirName,
          _ArtifactsAreNotEnvirionmentSpecific,
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
            _ArtifactsAreNotEnvirionmentSpecific,
            _TerminalAppName,
            _TerminalAppDirName,
            _TerminalAppExeName);

      Assert.Throws<ArgumentNullException>(()=>projectInfo.CreateDeploymentTask(null));
    }

    [Test]
    public void Test_CreateDeployemntTask_RunsProperly_WhenAllIsWell()
    {
      var objectFactory = new Mock<IObjectFactory>(MockBehavior.Strict);
      var envInfoRepository = new Mock<IEnvironmentInfoRepository>(MockBehavior.Strict);
      var artifactsRepository = new Mock<IArtifactsRepository>(MockBehavior.Strict);
      var taskScheduler = new Mock<ITaskScheduler>(MockBehavior.Strict);
      var ntServiceManager = new Mock<INtServiceManager>(MockBehavior.Strict);
      var projInfoRepository = new Mock<IProjectInfoRepository>(MockBehavior.Strict);

      var projectInfo = new TerminalAppProjectInfo(
            _Name,
            _ArtifactsRepositoryName,
            _ArtifactsRepositoryDirName,
            _ArtifactsAreNotEnvirionmentSpecific,
            _TerminalAppName,
            _TerminalAppDirName,
            _TerminalAppExeName);

      objectFactory.Setup(o => o.CreateEnvironmentInfoRepository()).Returns(envInfoRepository.Object);
      objectFactory.Setup(o => o.CreateArtifactsRepository()).Returns(artifactsRepository.Object);
      objectFactory.Setup(o => o.CreateTaskScheduler()).Returns(taskScheduler.Object);
      objectFactory.Setup(o => o.CreateNtServiceManager()).Returns(ntServiceManager.Object);
      objectFactory.Setup(o => o.CreateProjectInfoRepository()).Returns(projInfoRepository.Object);

      projectInfo.CreateDeploymentTask(objectFactory.Object);
    }

    [Test]
    public void Test_GetTargetFolders_RunsProperly_WhenAllIsWell()
    {
      string machine = Environment.MachineName;
      const string baseDirPath = "c:\\basedir";
      string terminalmachine = "terminalmachine";

      var envInfo =
        new EnvironmentInfo(
          "name",
          "templates",
          machine,
          "failover",
          new[] { "webmachine" },
          terminalmachine,
          "schedulermachine",
          baseDirPath,
          "webbasedir",
          "c:\\scheduler",
          "c:\\terminal",
          false,
          TestData._EnvironmentUsers,
          TestData._AppPoolInfos,
          TestData._DatabaseServers,
          TestData._WebAppProjectConfigurations,
          TestData._ProjectToFailoverClusterGroupMappings,
          TestData._DbProjectConfigurations,
          "terminalAppsShortcutFolder");

      var projectInfo =
        new TerminalAppProjectInfo(
          _Name,
          _ArtifactsRepositoryName,
          _ArtifactsRepositoryDirName,
          _ArtifactsAreNotEnvirionmentSpecific,
          _TerminalAppName,
          _TerminalAppDirName,
          _TerminalAppExeName);

      string terminalServerNetworkPath =
        envInfo.GetTerminalServerNetworkPath(
          string.Format("{0}{1}\\1.0.0.0", envInfo.GetTerminalAppsBaseDirPath(envInfo.TerminalServerMachineName), projectInfo.TerminalAppDirName));

      _directoryAdapterFake.Setup(
        da => da.GetDirectories(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>()))
        .Returns(new[] { terminalServerNetworkPath });

      // act
      List<string> targetFolders =
        projectInfo.GetTargetFolders(_objectFactoryFake.Object, envInfo)
          .ToList();

      Assert.IsNotNull(targetFolders);
      Assert.AreEqual(1, targetFolders.Count);
      Assert.AreEqual(terminalServerNetworkPath, targetFolders[0]);
    }

    [Test]
    public void Test_GetTargetFolders_Throws_EnvInfo_null()
    {
      var projectInfo = new TerminalAppProjectInfo(
            _Name,
            _ArtifactsRepositoryName,
            _ArtifactsRepositoryDirName,
            _ArtifactsAreNotEnvirionmentSpecific,
            _TerminalAppName,
            _TerminalAppDirName,
            _TerminalAppExeName);

      Assert.Throws<ArgumentNullException>(() => projectInfo.GetTargetFolders(_objectFactoryFake.Object, null));
    }
  }
}
