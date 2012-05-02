using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain;
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

    private static  readonly NtServiceProjectInfo _ntServiceProjectInfo = new NtServiceProjectInfo("name", "artifactsRepo", "artifactsRepoDir", "serviceName", "serviceDir", "serviceDisplayed", "exeName", "Sample.User");

    private Mock<INtServiceManager> ntServiceManager;
    private Mock<IArtifactsRepository> artifactsRepository;
    private Mock<IEnvironmentInfoRepository> environmentInfoRepository;
    private Mock<IPasswordCollector> passwordCollector;

    private static readonly List<EnvironmentUser> _EnvironmentUsers =
      new List<EnvironmentUser>
        {
          new EnvironmentUser("Sample.User", "some_user@centrala.kaczmarski.pl"),
        };

    [SetUp]
    public void SetUp()
    {
      ntServiceManager = new Mock<INtServiceManager>(MockBehavior.Strict);
      artifactsRepository = new Mock<IArtifactsRepository>(MockBehavior.Strict);
      environmentInfoRepository = new Mock<IEnvironmentInfoRepository>(MockBehavior.Strict);
      passwordCollector = new Mock<IPasswordCollector>(MockBehavior.Strict);
    }

    [Test]
    public void TestCtorArgumentsChecking()
    {
      var ctorTester =
        new CtorTester<DeployNtServiceDeploymentTask>(
          new object[]
            {
              environmentInfoRepository.Object,
              artifactsRepository.Object,
              ntServiceManager.Object,
              passwordCollector.Object,
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
      var ntServiceProjectInfo = new NtServiceProjectInfo("name", artifactsRepo, "artifactsRepoDir", serviceName, serviceDir, "serviceDisplayed", "exeName", "Sample.User");
      var envInfo = new EnvironmentInfo("name", "templates", machine , new[] { "webmachine" }, "terminalmachine", "databasemachine", baseDirPath, "webbasedir", "scheduler", "terminal", _EnvironmentUsers);

      environmentInfoRepository
        .Setup(e => e.GetByName(envName))
        .Returns(envInfo);

      passwordCollector
        .Setup(pc => pc.CollectPasswordForUser(envInfo.Name, envInfo.GetEnvironmentUserByName(ntServiceProjectInfo.NtServiceUserId).UserName))
        .Returns("some password");
      
      ntServiceManager
        .Setup(n => n.DoesServiceExist(machine, serviceName))
        .Returns(false);
      
      artifactsRepository
        .Setup(a => a.GetArtifacts(artifactsRepo,projectName, buildID, It.IsAny<string>()));

      var deployNTService =
        new DeployNtServiceDeploymentTask(
          environmentInfoRepository.Object,
          artifactsRepository.Object,
          ntServiceManager.Object,
          passwordCollector.Object,
          ntServiceProjectInfo,
          projectName,
          buildID,
          envName);

      Assert.Throws<DeploymentTaskException>(deployNTService.PrepareAndExecute);
    }
  }
}
