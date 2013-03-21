using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Iis;
using UberDeployer.Core.Management.MsDeploy;
using UberDeployer.Core.Management.ScheduledTasks;

namespace UberDeployer.Core.Tests.Domain
{
  // TODO IMM HI: formatting; code style
  [TestFixture]
  public class WebAppProjectInfoTests
  {
    private const string Name = "prj1";
    private const string ArtifactsRepositoryName = "repoName";
    private const string ArtifactsRepositoryDirName = "repoDirName";
    private const bool ArtifactsAreNotEnvironmentSpecific = false;

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
        new WebAppProjectConfiguration("prj1", "website", "apppool", "dir", "prj1"),
      };

    private static readonly List<ProjectToFailoverClusterGroupMapping> _ProjectToFailoverClusterGroupMappings =
      new List<ProjectToFailoverClusterGroupMapping>
        {
          new ProjectToFailoverClusterGroupMapping("prj1", "cg1"),
        };

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
      var prjInfoRepository = new Mock<IProjectInfoRepository>(MockBehavior.Strict);
      var envInfoRepository = new Mock<IEnvironmentInfoRepository>(MockBehavior.Strict);
      var artifactsRepository = new Mock<IArtifactsRepository>(MockBehavior.Strict);
      var taskScheduler = new Mock<ITaskScheduler>(MockBehavior.Strict);
      var imsDeploy = new Mock<IMsDeploy>(MockBehavior.Strict);
      var iisManager = new Mock<IIisManager>(MockBehavior.Strict);

      var projectInfo =
        new WebAppProjectInfo(
          Name,
          ArtifactsRepositoryName,
          ArtifactsRepositoryDirName,
          ArtifactsAreNotEnvironmentSpecific);

      objectFactory.Setup(o => o.CreateProjectInfoRepository()).Returns(prjInfoRepository.Object);
      objectFactory.Setup(o => o.CreateEnvironmentInfoRepository()).Returns(envInfoRepository.Object);
      objectFactory.Setup(o => o.CreateArtifactsRepository()).Returns(artifactsRepository.Object);
      objectFactory.Setup(o => o.CreateTaskScheduler()).Returns(taskScheduler.Object);
      objectFactory.Setup(o => o.CreateIMsDeploy()).Returns(imsDeploy.Object);
      objectFactory.Setup(o => o.CreateIIisManager()).Returns(iisManager.Object);

      projectInfo.CreateDeploymentTask(objectFactory.Object);
    }

    [Test]
    public void Test_GetTargetFolders_RunsProperly_WhenAllIsWell()
    {
      string machine = Environment.MachineName;

      var envInfo =
        new EnvironmentInfo(
          "name",
          "templates",
          machine,
          "failover",
          new[] { "webmachine" },
          "terminalmachine",
          "schedulermachine",
          "databasemachine",
          "C:\\basedir",
          "C:\\basedir",
          "c:\\scheduler",
          "terminal",
          false,
          _EnvironmentUsers,
          _AppPoolInfos,
          _WebAppProjectConfigurations,
          _ProjectToFailoverClusterGroupMappings,
          "terminalAppsShortcutFolder");

      var projectInfo =
        new WebAppProjectInfo(
          Name,
          ArtifactsRepositoryName,
          ArtifactsRepositoryDirName,
          false);

      Assert.IsNotNullOrEmpty(projectInfo.GetTargetFolders(_objectFactoryFake.Object, envInfo).FirstOrDefault());
    }

    [Test]
    public void Test_GetTargetUrls_RunsProperly_WhenAllIsWell()
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
        new WebAppProjectInfo(
          Name,
          ArtifactsRepositoryName,
          ArtifactsRepositoryDirName,
          ArtifactsAreNotEnvironmentSpecific);

      List<string> targetUrls =
        projectInfo.GetTargetUrls(envInfo)
          .ToList();

      Assert.IsNotNull(targetUrls);
      Assert.AreEqual(1, targetUrls.Count);
      Assert.AreEqual("http://webmachine/" + "prj1", targetUrls[0]);
    }

    [Test]
    public void Test_WebServiceProjectInfoTests_RunsOK_WhenALLIZWELL()
    {
      new WebServiceProjectInfo(
        Name,
        ArtifactsRepositoryName,
        ArtifactsRepositoryDirName,
        ArtifactsAreNotEnvironmentSpecific);
    }

    [Test]
    public void Test_GetType_ReturnsType()
    {
      var info =
        new WebServiceProjectInfo(
          Name,
          ArtifactsRepositoryName,
          ArtifactsRepositoryDirName,
          ArtifactsAreNotEnvironmentSpecific);

      Assert.AreEqual(ProjectType.WebService, info.Type);
    }
  }
}
