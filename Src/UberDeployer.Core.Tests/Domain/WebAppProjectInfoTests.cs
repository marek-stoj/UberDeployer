using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using UberDeployer.Common.IO;
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
    private const string _Name = "webappprj";
    private const string _ArtifactsRepositoryName = "repoName";
    private const string _ArtifactsRepositoryDirName = "repoDirName";
    private const bool _ArtifactsAreNotEnvironmentSpecific = false;
    private const string _AppPoolId = "app_pool_id";
    private const string _WebSiteName = "web_site_name";
    private const string _WebAppDirName = "web_app_dir_name";
    private const string _WebAppName = "web_app_name";

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
      var prjInfoRepository = new Mock<IProjectInfoRepository>(MockBehavior.Strict);
      var envInfoRepository = new Mock<IEnvironmentInfoRepository>(MockBehavior.Strict);
      var artifactsRepository = new Mock<IArtifactsRepository>(MockBehavior.Strict);
      var taskScheduler = new Mock<ITaskScheduler>(MockBehavior.Strict);
      var imsDeploy = new Mock<IMsDeploy>(MockBehavior.Strict);
      var iisManager = new Mock<IIisManager>(MockBehavior.Strict);
      var fileAdapter = new Mock<IFileAdapter>(MockBehavior.Loose);
      var zipFileAdapter = new Mock<IZipFileAdapter>(MockBehavior.Loose);

      var projectInfo =
        new WebAppProjectInfo(
          _Name,
          _ArtifactsRepositoryName,
          _AllowedEnvironmentNames,
          _ArtifactsRepositoryDirName,
          _ArtifactsAreNotEnvironmentSpecific,
          _AppPoolId,
          _WebSiteName,
          _WebAppDirName,
          _WebAppName);

      objectFactory.Setup(o => o.CreateProjectInfoRepository()).Returns(prjInfoRepository.Object);
      objectFactory.Setup(o => o.CreateEnvironmentInfoRepository()).Returns(envInfoRepository.Object);
      objectFactory.Setup(o => o.CreateArtifactsRepository()).Returns(artifactsRepository.Object);
      objectFactory.Setup(o => o.CreateTaskScheduler()).Returns(taskScheduler.Object);
      objectFactory.Setup(o => o.CreateIMsDeploy()).Returns(imsDeploy.Object);
      objectFactory.Setup(o => o.CreateIIisManager()).Returns(iisManager.Object);
      objectFactory.Setup(o => o.CreateFileAdapter()).Returns(fileAdapter.Object);
      objectFactory.Setup(o => o.CreateZipFileAdapter()).Returns(zipFileAdapter.Object);

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
          new[] { "schedulerServerTasksMachineName1", "schedulerServerTasksMachineName2", },
          new[] { "schedulerServerBinariesMachineName1", "schedulerServerBinariesMachineName2", },
          "C:\\basedir",
          "C:\\basedir",
          "c:\\scheduler",
          "terminal",
          false,
          TestData.EnvironmentUsers,
          TestData.AppPoolInfos,
          TestData.DatabaseServers,
          TestData.ProjectToFailoverClusterGroupMappings,
          TestData.WebAppProjectConfigurationOverrides,
          TestData.DbProjectConfigurationOverrides,
          "terminalAppsShortcutFolder",
          "artifactsDeploymentDirPath");

      var projectInfo =
        new WebAppProjectInfo(
          _Name,
          _ArtifactsRepositoryName,
          _AllowedEnvironmentNames,
          _ArtifactsRepositoryDirName,
          false,
          _AppPoolId,
          _WebSiteName,
          _WebAppDirName,
          _WebAppName);

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
          new[] { "schedulerServerTasksMachineName1", "schedulerServerTasksMachineName2", },
          new[] { "schedulerServerBinariesMachineName1", "schedulerServerBinariesMachineName2", },
          baseDirPath,
          "webbasedir",
          "c:\\scheduler",
          "terminal",
          false,
          TestData.EnvironmentUsers,
          TestData.AppPoolInfos,
          TestData.DatabaseServers,
          TestData.ProjectToFailoverClusterGroupMappings,
          TestData.WebAppProjectConfigurationOverrides,
          TestData.DbProjectConfigurationOverrides,
          "terminalAppsShortcutFolder",
          "artifactsDeploymentDirPath");

      var projectInfo =
        new WebAppProjectInfo(
          _Name,
          _ArtifactsRepositoryName,
          _AllowedEnvironmentNames,
          _ArtifactsRepositoryDirName,
          _ArtifactsAreNotEnvironmentSpecific,
          _AppPoolId,
          _WebSiteName,
          _WebAppDirName,
          _WebAppName);

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
        _ArtifactsAreNotEnvironmentSpecific,
        _AppPoolId,
        _WebSiteName,
        _WebAppDirName,
        _WebAppName);
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
          _ArtifactsAreNotEnvironmentSpecific,
          _AppPoolId,
          _WebSiteName,
          _WebAppDirName,
          _WebAppName);

      Assert.AreEqual(ProjectType.WebService, info.Type);
    }
  }
}
