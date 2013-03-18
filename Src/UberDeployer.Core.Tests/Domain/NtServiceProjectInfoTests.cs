using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
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

    private static readonly List<EnvironmentUser> _EnvironmentUsers =
      new List<EnvironmentUser>
        {
          new EnvironmentUser("Sample.User", "some_user@centrala.kaczmarski.pl"),
        };

    private static readonly List<IisAppPoolInfo> _AppPoolInfos =
      new List<IisAppPoolInfo>()
      {
        new IisAppPoolInfo("apppool", IisAppPoolVersion.V4_0, IisAppPoolMode.Integrated),
      };

    private static readonly List<WebAppProjectConfiguration> _WebAppProjectConfigurations =
      new List<WebAppProjectConfiguration>
      {
        new WebAppProjectConfiguration("prj1", "website", "apppool", "prj1"),
      };


    private static readonly List<ProjectToFailoverClusterGroupMapping> _ProjectToFailoverClusterGroupMappings =
      new List<ProjectToFailoverClusterGroupMapping>
        {
          new ProjectToFailoverClusterGroupMapping("prj1", "cg1"),
        };

    [Test]
    public void Test_CreateDeployemntTask_RunsProperly_WhenAllIsWell()
    {
      var objectFactory = new Mock<IObjectFactory>(MockBehavior.Strict);
      var envInfoRepository = new Mock<IEnvironmentInfoRepository>(MockBehavior.Loose);
      var artifactsRepository = new Mock<IArtifactsRepository>(MockBehavior.Loose);
      var taskScheduler = new Mock<ITaskScheduler>(MockBehavior.Loose);
      var ntServiceManager = new Mock<INtServiceManager>(MockBehavior.Loose);
      var passwordCollector = new Mock<IPasswordCollector>(MockBehavior.Loose);
      var failoverClusterManager = new Mock<IFailoverClusterManager>(MockBehavior.Loose);

      var projectInfo =
        new NtServiceProjectInfo(
          _ProjectName,
          _ArtifactsRepositoryName,
          _ArtifactsRepositoryDirName,
          _ArtifactsAreNotEnvironmentSpecific,
          _NtServiceName,
          _NtServiceDirName,
          _NtServiceDisplayName,
          _NtServiceExeName,
          _NtServiceUserId);

      objectFactory.Setup(o => o.CreateEnvironmentInfoRepository()).Returns(envInfoRepository.Object);
      objectFactory.Setup(o => o.CreateArtifactsRepository()).Returns(artifactsRepository.Object);
      objectFactory.Setup(o => o.CreateTaskScheduler()).Returns(taskScheduler.Object);
      objectFactory.Setup(o => o.CreateNtServiceManager()).Returns(ntServiceManager.Object);
      objectFactory.Setup(o => o.CreatePasswordCollector()).Returns(passwordCollector.Object);
      objectFactory.Setup(o => o.CreateFailoverClusterManager()).Returns(failoverClusterManager.Object);

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
          "databasemachine",
          baseDirPath,
          "webbasedir",
          "c:\\scheduler",
          "terminal",
          false,
          _EnvironmentUsers,
          _AppPoolInfos,
          _WebAppProjectConfigurations,
          _ProjectToFailoverClusterGroupMappings,
          "terminalAppsShortcutFolder");

      var projectInfo =
        new NtServiceProjectInfo(
          _ProjectName,
          _ArtifactsRepositoryName,
          _ArtifactsRepositoryDirName,
          _ArtifactsAreNotEnvironmentSpecific,
          _NtServiceName,
          _NtServiceDirName,
          _NtServiceDisplayName,
          _NtServiceExeName,
          _NtServiceUserId);

      var targetFolders = projectInfo.GetTargetFolders(envInfo).ToList();

      Assert.IsNotNull(targetFolders);
      Assert.AreEqual(1, targetFolders.Count);
      Assert.AreEqual("\\\\" + machine + "\\c$\\basedir\\" + _NtServiceDirName, targetFolders[0]);
    }
  }
}
