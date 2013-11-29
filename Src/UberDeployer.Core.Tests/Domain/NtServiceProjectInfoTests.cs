using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using UberDeployer.Common.IO;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.FailoverCluster;
using UberDeployer.Core.Management.NtServices;
using UberDeployer.Core.Management.ScheduledTasks;

namespace UberDeployer.Core.Tests.Domain
{
  // TODO IMM HI: formatting; code style
  [TestFixture]
  public class NtServiceProjectInfoTests
  {
    private const string _ProjectName = "name";
    private const string _ArtifactsRepositoryName = "artifRepoName";
    private const string _ArtifactsRepositoryDirName = "artifRepoDirName";
    private const bool _ArtifactsAreNotEnvironmentSpecific = false;
    private const string _NtServiceName = "ntServiceName";
    private const string _NtServiceDirName = "ntServiceDirName";
    private const string _NtServiceDisplayName = "ntServiceDisplayName";
    private const string _NtServiceExeName = "ntServiceExeName";
    private const string _NtServiceUserId = "Sample.User";
    private static readonly string[] _AllowedEnvironmentNames = { "env_name" };

    private Mock<IObjectFactory> _objectFactoryFake;

    [SetUp]
    public void SetUp()
    {
      _objectFactoryFake = new Mock<IObjectFactory>(MockBehavior.Loose);
    }

    [Test]
    public void Test_CreateDeployemntTask_RunsProperly_WhenAllIsWell()
    {
      var objectFactory = new Mock<IObjectFactory>(MockBehavior.Strict);
      var prjInfoRepository = new Mock<IProjectInfoRepository>(MockBehavior.Loose);
      var envInfoRepository = new Mock<IEnvironmentInfoRepository>(MockBehavior.Loose);
      var artifactsRepository = new Mock<IArtifactsRepository>(MockBehavior.Loose);
      var taskScheduler = new Mock<ITaskScheduler>(MockBehavior.Loose);
      var ntServiceManager = new Mock<INtServiceManager>(MockBehavior.Loose);
      var passwordCollector = new Mock<IPasswordCollector>(MockBehavior.Loose);
      var failoverClusterManager = new Mock<IFailoverClusterManager>(MockBehavior.Loose);
      var fileAdapter = new Mock<IFileAdapter>(MockBehavior.Loose);
      var zipFileAdapter = new Mock<IZipFileAdapter>(MockBehavior.Loose);

      var projectInfo =
        new NtServiceProjectInfo(
          _ProjectName,
          _ArtifactsRepositoryName,
          _AllowedEnvironmentNames,
          _ArtifactsRepositoryDirName,
          _ArtifactsAreNotEnvironmentSpecific,
          _NtServiceName,
          _NtServiceDirName,
          _NtServiceDisplayName,
          _NtServiceExeName,
          _NtServiceUserId);

      objectFactory.Setup(o => o.CreateProjectInfoRepository()).Returns(prjInfoRepository.Object);
      objectFactory.Setup(o => o.CreateEnvironmentInfoRepository()).Returns(envInfoRepository.Object);
      objectFactory.Setup(o => o.CreateArtifactsRepository()).Returns(artifactsRepository.Object);
      objectFactory.Setup(o => o.CreateTaskScheduler()).Returns(taskScheduler.Object);
      objectFactory.Setup(o => o.CreateNtServiceManager()).Returns(ntServiceManager.Object);
      objectFactory.Setup(o => o.CreatePasswordCollector()).Returns(passwordCollector.Object);
      objectFactory.Setup(o => o.CreateFailoverClusterManager()).Returns(failoverClusterManager.Object);
      objectFactory.Setup(o => o.CreateFileAdapter()).Returns(fileAdapter.Object);
      objectFactory.Setup(o => o.CreateZipFileAdapter()).Returns(zipFileAdapter.Object);

      projectInfo.CreateDeploymentTask(objectFactory.Object);
    }

    [Test]
    public void Test_GetTargetFolders_RunsProperly_WhenAllIsWell()
    {
      string machine = Environment.MachineName;
      const string baseDirPath = "c:\\basedir";

      var envInfo = 
        new EnvironmentInfo(
          "name",
          "templates",
          machine,
          "failover",
          new[] { "webmachine" },
          "terminalmachine",
          "schedulermachine",
          baseDirPath,
          "webbasedir",
          "c:\\scheduler",
          "terminal",
          false,
          TestData.EnvironmentUsers,
          TestData.AppPoolInfos,
          TestData.DatabaseServers,
          TestData.WebAppProjectConfigurations,
          TestData.ProjectToFailoverClusterGroupMappings,
          TestData.DbProjectConfigurations,
          "terminalAppsShortcutFolder",
          "artifactsDeploymentDirPath");

      var projectInfo =
        new NtServiceProjectInfo(
          _ProjectName,
          _ArtifactsRepositoryName,
          _AllowedEnvironmentNames,
          _ArtifactsRepositoryDirName,
          _ArtifactsAreNotEnvironmentSpecific,
          _NtServiceName,
          _NtServiceDirName,
          _NtServiceDisplayName,
          _NtServiceExeName,
          _NtServiceUserId);

      var targetFolders = projectInfo.GetTargetFolders(_objectFactoryFake.Object, envInfo).ToList();

      Assert.IsNotNull(targetFolders);
      Assert.AreEqual(1, targetFolders.Count);
      Assert.AreEqual("\\\\" + machine + "\\c$\\basedir\\" + _NtServiceDirName, targetFolders[0]);
    }
  }
}
