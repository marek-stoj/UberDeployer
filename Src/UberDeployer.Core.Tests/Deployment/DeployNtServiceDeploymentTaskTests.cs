using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.FailoverCluster;
using UberDeployer.Core.Management.NtServices;

namespace UberDeployer.Core.Tests.Deployment
{
  // TODO IMM HI: formatting; code style
  [TestFixture]
  public class DeployNtServiceDeploymentTaskTests
  {
    private const string _ProjectName = "projectName";
    private const string _BuildId = "id";
    private const string _EnvironmentName = "envName";

    private static  readonly NtServiceProjectInfo _ntServiceProjectInfo =
      new NtServiceProjectInfo(
        "name",
        "artifactsRepo",
        "artifactsRepoDir",
        false,
        "serviceName",
        "serviceDir",
        "serviceDisplayed",
        "exeName",
        "Sample.User");

    private Mock<INtServiceManager> _ntServiceManager;
    private Mock<IArtifactsRepository> _artifactsRepository;
    private Mock<IEnvironmentInfoRepository> _environmentInfoRepository;
    private Mock<IPasswordCollector> _passwordCollector;
    private Mock<IFailoverClusterManager> _failoverClusterManager;

    private static readonly List<EnvironmentUser> _EnvironmentUsers =
      new List<EnvironmentUser>
        {
          new EnvironmentUser("Sample.User", "some_user@centrala.kaczmarski.pl"),
        };

    private static readonly List<ProjectToFailoverClusterGroupMapping> _ProjectToFailoverClusterGroupMappings =
      new List<ProjectToFailoverClusterGroupMapping>
        {
          new ProjectToFailoverClusterGroupMapping("prj1", "cg1"),
        };

    [SetUp]
    public void SetUp()
    {
      _ntServiceManager = new Mock<INtServiceManager>(MockBehavior.Strict);
      _artifactsRepository = new Mock<IArtifactsRepository>(MockBehavior.Strict);
      _environmentInfoRepository = new Mock<IEnvironmentInfoRepository>(MockBehavior.Strict);
      _passwordCollector = new Mock<IPasswordCollector>(MockBehavior.Strict);
      _failoverClusterManager = new Mock<IFailoverClusterManager>(MockBehavior.Strict);
    }

    [Test]
    public void TestCtorArgumentsChecking()
    {
      var ctorTester =
        new CtorTester<DeployNtServiceDeploymentTask>(
          new object[]
            {
              _environmentInfoRepository.Object,
              _artifactsRepository.Object,
              _ntServiceManager.Object,
              _passwordCollector.Object,
              _ntServiceProjectInfo,
              _ProjectName,
              _BuildId,
              _EnvironmentName,
            }
          );

      ctorTester.TestAll();
    }

    // TODO IMM HI: fix
    [Test]
    public void Test_InstallNtServiceDeploymentStepServiceDirPathDoesntExist()
    {
      const string serviceName = "serviceName";
      const string serviceDir = "serviceDir";
      const string artifactsRepo = "artifactsRepo";
      const string projectName = "projectName";
      const string buildID = "id";
      const string envName = "envName";
      const string baseDirPath = "c:\\basedir";

      string machine = Environment.MachineName;

      var ntServiceProjectInfo =
        new NtServiceProjectInfo(
          "name",
          artifactsRepo,
          "artifactsRepoDir",
          false,
          serviceName,
          serviceDir,
          "serviceDisplayed",
          "exeName",
          "Sample.User");

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
          "scheduler",
          "terminal",
          false,
          _EnvironmentUsers,
          _ProjectToFailoverClusterGroupMappings);

      _environmentInfoRepository
        .Setup(e => e.GetByName(envName))
        .Returns(envInfo);

      _passwordCollector
        .Setup(pc => pc.CollectPasswordForUser(envInfo.Name, envInfo.AppServerMachineName, envInfo.GetEnvironmentUserByName(ntServiceProjectInfo.NtServiceUserId).UserName))
        .Returns("some password");
      
      _ntServiceManager
        .Setup(n => n.DoesServiceExist(machine, serviceName))
        .Returns(false);
      
      _artifactsRepository
        .Setup(a => a.GetArtifacts(artifactsRepo,projectName, buildID, It.IsAny<string>()));

      var deployNTService =
        new DeployNtServiceDeploymentTask(
          _environmentInfoRepository.Object,
          _artifactsRepository.Object,
          _ntServiceManager.Object,
          _passwordCollector.Object,
          _failoverClusterManager.Object,
          ntServiceProjectInfo,
          projectName,
          buildID,
          envName);

      Assert.Throws<DeploymentTaskException>(deployNTService.PrepareAndExecute);
    }
  }
}
