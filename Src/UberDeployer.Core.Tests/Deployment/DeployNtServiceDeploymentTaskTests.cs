using Moq;
using NUnit.Framework;
using UberDeployer.Common.IO;
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

    private static readonly NtServiceProjectInfo _ntServiceProjectInfo =
      new NtServiceProjectInfo(
        "name",
        "artifactsRepo",
        new[] { "env_name" },
        "artifactsRepoDir",
        false,
        "serviceName",
        "serviceDir",
        "serviceDisplayed",
        "exeName",
        "Sample.User");

    private Mock<INtServiceManager> _ntServiceManager;
    private Mock<IArtifactsRepository> _artifactsRepository;
    private Mock<IProjectInfoRepository> _projectInfoRepository;
    private Mock<IEnvironmentInfoRepository> _environmentInfoRepository;
    private Mock<IPasswordCollector> _passwordCollector;
    private Mock<IFailoverClusterManager> _failoverClusterManager;
    private Mock<IDirectoryAdapter> _directoryAdapter;
    private Mock<IFileAdapter> _fileAdapter;
    private Mock<IZipFileAdapter> _zipFileAdapter;

    [SetUp]
    public void SetUp()
    {
      _ntServiceManager = new Mock<INtServiceManager>(MockBehavior.Strict);
      _artifactsRepository = new Mock<IArtifactsRepository>(MockBehavior.Strict);
      _projectInfoRepository = new Mock<IProjectInfoRepository>(MockBehavior.Strict);
      _environmentInfoRepository = new Mock<IEnvironmentInfoRepository>(MockBehavior.Strict);
      _passwordCollector = new Mock<IPasswordCollector>(MockBehavior.Strict);
      _failoverClusterManager = new Mock<IFailoverClusterManager>(MockBehavior.Strict);
      _directoryAdapter = new Mock<IDirectoryAdapter>(MockBehavior.Strict);
      _fileAdapter = new Mock<IFileAdapter>(MockBehavior.Strict);
      _zipFileAdapter = new Mock<IZipFileAdapter>(MockBehavior.Strict);
    }

    [Test]
    public void TestCtorArgumentsChecking()
    {
      var ctorTester =
        new CtorTester<DeployNtServiceDeploymentTask>(
          new object[]
          {
            _projectInfoRepository.Object,
            _environmentInfoRepository.Object,
            _artifactsRepository.Object,
            _ntServiceManager.Object,
            _passwordCollector.Object,
            _failoverClusterManager.Object,
            _directoryAdapter.Object,
            _fileAdapter.Object,
            _zipFileAdapter.Object,
          }
          );

      ctorTester.TestAll();
    }
  }
}
