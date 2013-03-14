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
    private const string WebAppName = "WebAppName";
    private const string WebAppDirName = "WebAppDirName";
    private const string Name = "name";
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

    private static readonly List<ProjectToWebSiteMapping> _ProjectToWebSiteMappings =
      new List<ProjectToWebSiteMapping>
      {
        new ProjectToWebSiteMapping("prj1", "website"),
      };

    private static readonly List<ProjectToAppPoolMapping> _ProjectToAppPoolMappings =
      new List<ProjectToAppPoolMapping>
      {
        new ProjectToAppPoolMapping("prj1", "apppool"),
      };

    private static readonly List<ProjectToFailoverClusterGroupMapping> _ProjectToFailoverClusterGroupMappings =
      new List<ProjectToFailoverClusterGroupMapping>
        {
          new ProjectToFailoverClusterGroupMapping("prj1", "cg1"),
        };

    private readonly IisAppPoolInfo AppPoolInfo = new IisAppPoolInfo(Name, IisAppPoolVersion.V4_0, IisAppPoolMode.Classic);

    [Test]
    public void Test_WebAppProjectInfoTests_Thows_When_Name_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
          {
            new WebAppProjectInfo(
              null,
              ArtifactsRepositoryName,
              ArtifactsRepositoryDirName,
              ArtifactsAreNotEnvironmentSpecific,
              WebAppName,
              WebAppDirName);
          });
    }

    [Test]
    public void Test_WebAppProjectInfoTests_Thows_When_ArtifactsRepositoryName_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
          {
            new WebAppProjectInfo(
              Name,
              null,
              ArtifactsRepositoryDirName,
              ArtifactsAreNotEnvironmentSpecific,
              WebAppName,
              WebAppDirName);
          });
    }

    [Test]
    public void Test_WebAppProjectInfoTests_Thows_When_WebAppName_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
          {
            new WebAppProjectInfo(
              Name,
              ArtifactsRepositoryName,
              ArtifactsRepositoryDirName,
              ArtifactsAreNotEnvironmentSpecific,
              null,
              WebAppDirName);
          });
    }

    [Test]
    public void Test_WebAppProjectInfoTests_Thows_When_WebAppDirName_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
          {
            new WebAppProjectInfo(
              Name,
              ArtifactsRepositoryName,
              ArtifactsRepositoryDirName,
              ArtifactsAreNotEnvironmentSpecific,
              WebAppName,
              null);
          });
    }

    [Test]
    public void Test_CreateDeployemntTask_Thows_When_ObjectFactory_null()
    {
      var projectInfo =
        new WebAppProjectInfo(
          Name,
          ArtifactsRepositoryName,
          ArtifactsRepositoryDirName,
          ArtifactsAreNotEnvironmentSpecific,
          WebAppName,
          WebAppDirName);

      Assert.Throws<ArgumentNullException>(
        () => projectInfo.CreateDeploymentTask(null));
    }

    [Test]
    public void Test_CreateDeployemntTask_RunsProperly_WhenAllIsWell()
    {
      var objectFactory = new Mock<IObjectFactory>(MockBehavior.Strict);
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
          ArtifactsAreNotEnvironmentSpecific,
          WebAppName,
          WebAppDirName);

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
          "databasemachine",
          "C:\\basedir",
          "C:\\basedir",
          "c:\\scheduler",
          "terminal",
          false,
          _EnvironmentUsers,
          _AppPoolInfos,
          _ProjectToWebSiteMappings,
          _ProjectToAppPoolMappings,
          _ProjectToFailoverClusterGroupMappings);

      var projectInfo =
        new WebAppProjectInfo(
          Name,
          ArtifactsRepositoryName,
          ArtifactsRepositoryDirName,
          false,
          WebAppName,
          WebAppDirName);

      Assert.IsNotNullOrEmpty(projectInfo.GetTargetFolders(envInfo).FirstOrDefault());
    }

    [Test]
    public void Test_GetTargetFolders_Throws_EnvInfo_null()
    {
      var projectInfo =
        new WebAppProjectInfo(
          Name,
          ArtifactsRepositoryName,
          ArtifactsRepositoryDirName,
          ArtifactsAreNotEnvironmentSpecific,
          WebAppName,
          WebAppDirName);

      Assert.Throws<ArgumentNullException>(() => projectInfo.GetTargetFolders(null));
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
          "databasemachine",
          baseDirPath,
          "webbasedir",
          "c:\\scheduler",
          "terminal",
          false,
          _EnvironmentUsers,
          _AppPoolInfos,
          _ProjectToWebSiteMappings,
          _ProjectToAppPoolMappings,
          _ProjectToFailoverClusterGroupMappings);

      var projectInfo =
        new WebAppProjectInfo(
          Name,
          ArtifactsRepositoryName,
          ArtifactsRepositoryDirName,
          ArtifactsAreNotEnvironmentSpecific,
          WebAppName,
          WebAppDirName);

      List<string> targetUrls =
        projectInfo.GetTargetUrls(envInfo)
          .ToList();

      Assert.IsNotNull(targetUrls);
      Assert.AreEqual(1, targetUrls.Count);
      Assert.AreEqual("http://webmachine/" + WebAppName, targetUrls[0]);
    }

    [Test]
    public void Test_GetTargetUrls_Throws_EnvInfo_null()
    {

      var projectInfo =
        new WebAppProjectInfo(
          Name,
          ArtifactsRepositoryName,
          ArtifactsRepositoryDirName,
          ArtifactsAreNotEnvironmentSpecific,
          WebAppName,
          WebAppDirName);

      Assert.Throws<ArgumentNullException>(() => projectInfo.GetTargetUrls(null));
    }

    [Test]
    public void Test_WebServiceProjectInfoTests_RunsOK_WhenALLIZWELL()
    {
      new WebServiceProjectInfo(
        Name,
        ArtifactsRepositoryName,
        ArtifactsRepositoryDirName,
        ArtifactsAreNotEnvironmentSpecific,
        WebAppName,
        WebAppDirName);
    }

    [Test]
    public void Test_GetType_ReturnsType()
    {
      var info =
        new WebServiceProjectInfo(
          Name,
          ArtifactsRepositoryName,
          ArtifactsRepositoryDirName,
          ArtifactsAreNotEnvironmentSpecific,
          WebAppName,
          WebAppDirName);

      Assert.AreEqual(ProjectType.WebService, info.Type);
    }
  }
}
