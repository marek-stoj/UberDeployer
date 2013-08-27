using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Iis;
using UberDeployer.Core.Management.MsDeploy;
using UberDeployer.Core.Management.ScheduledTasks;
using UberDeployer.Core.Tests.Generators;

namespace UberDeployer.Core.Tests.Domain
{
  // TODO IMM HI: formatting; code style
  [TestFixture]
  public class WebAppProjectInfoTests
  {
    private const string _Name = "webappprj";
    private const string _ArtifactsRepositoryName = "repoName";
    private const string _ArtifactsRepositoryDirName = "repoDirName";
    private const bool _ArtifactsAreNotEnvironmentSpecific = false;
    private static readonly string[] _AllowedEnvironmentNames = new[] { "env_name" };

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
          _Name,
          _ArtifactsRepositoryName,
          _AllowedEnvironmentNames,
          _ArtifactsRepositoryDirName,
          _ArtifactsAreNotEnvironmentSpecific);

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
          "C:\\basedir",
          "C:\\basedir",
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
        new WebAppProjectInfo(
          _Name,
          _ArtifactsRepositoryName,
          _AllowedEnvironmentNames,
          _ArtifactsRepositoryDirName,
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
        new WebAppProjectInfo(
          _Name,
          _ArtifactsRepositoryName,
          _AllowedEnvironmentNames,
          _ArtifactsRepositoryDirName,
          _ArtifactsAreNotEnvironmentSpecific);

      List<string> targetUrls =
        projectInfo.GetTargetUrls(envInfo)
          .ToList();

      Assert.IsNotNull(targetUrls);
      Assert.AreEqual(1, targetUrls.Count);
      Assert.AreEqual("http://webmachine/" + "webapp", targetUrls[0]);
    }

    [Test]
    public void Test_WebServiceProjectInfoTests_RunsOK_WhenALLIZWELL()
    {
      new WebServiceProjectInfo(
        _Name,
        _ArtifactsRepositoryName,
        _AllowedEnvironmentNames,
        _ArtifactsRepositoryDirName,
        _ArtifactsAreNotEnvironmentSpecific);
    }

    [Test]
    public void Test_GetType_ReturnsType()
    {
      var info =
        new WebServiceProjectInfo(
          _Name,
          _ArtifactsRepositoryName,
          _AllowedEnvironmentNames,
          _ArtifactsRepositoryDirName,
          _ArtifactsAreNotEnvironmentSpecific);

      Assert.AreEqual(ProjectType.WebService, info.Type);
    }
  }
}
