using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using UberDeployer.Common.IO;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.FailoverCluster;
using UberDeployer.Core.Management.NtServices;
using UberDeployer.Core.Tests.Generators;

namespace UberDeployer.Core.Tests.Deployment
{
  [TestFixture]
  public class DeployExtensionProjectDeploymentTaskTests
  {
    private DeployExtensionProjectDeploymentTask _deploymentTask;

    private readonly Mock<IArtifactsRepository> _artifactsRepository = new Mock<IArtifactsRepository>();
    private readonly Mock<IDirectoryAdapter> _directoryAdapter = new Mock<IDirectoryAdapter>();
    private readonly Mock<IFileAdapter> _fileAdapter = new Mock<IFileAdapter>();
    private readonly Mock<IZipFileAdapter> _zipFileAdapter = new Mock<IZipFileAdapter>();
    private readonly Mock<IFailoverClusterManager> _failoverClusterManager = new Mock<IFailoverClusterManager>();
    private readonly Mock<INtServiceManager> _ntServiceManager = new Mock<INtServiceManager>();
    private readonly Mock<IProjectInfoRepository> _projectInfoRepository = new Mock<IProjectInfoRepository>();
    private readonly Mock<IEnvironmentInfoRepository> _environmentInfoRepository = new Mock<IEnvironmentInfoRepository>();

    [SetUp]
    public void Setup()
    {
      _deploymentTask = new DeployExtensionProjectDeploymentTask(
        _projectInfoRepository.Object,
        _environmentInfoRepository.Object,
        _artifactsRepository.Object,
        _directoryAdapter.Object,
        _fileAdapter.Object,
        _zipFileAdapter.Object,
        _failoverClusterManager.Object,
        _ntServiceManager.Object);

      DeploymentInfo di = DeploymentInfoGenerator.GetExtensionProjectDeploymentInfo();
      _deploymentTask.Initialize(di);

      _environmentInfoRepository
        .Setup(x => x.FindByName(It.IsAny<string>()))
        .Returns(DeploymentDataGenerator.GetEnvironmentInfo);

      _projectInfoRepository
        .Setup(x => x.FindByName(DeploymentDataGenerator.GetExtensionProjectInfo().ExtendedProjectName))
        .Returns(DeploymentDataGenerator.GetNtServiceProjectInfo);

      _projectInfoRepository
        .Setup(x => x.FindByName(di.ProjectName))
        .Returns(DeploymentDataGenerator.GetExtensionProjectInfo);
    }

    [Test]
    [TestCase(typeof(DownloadArtifactsDeploymentStep))]
    [TestCase(typeof(ExtractArtifactsDeploymentStep))]
    [TestCase(typeof(CopyFilesDeploymentStep))]
    [TestCase(typeof(StartNtServiceDeploymentStep))]
    [TestCase(typeof(StopNtServiceDeploymentStep))]
    public void DoPrepare_adds_deployment_step(Type deploymentStepType)
    {
      // act
      _deploymentTask.Prepare();

      // assert
      Assert.IsNotNull(_deploymentTask.SubTasks.Any(x => x.GetType() == deploymentStepType));
    }

    [Test]
    public void DoPrepare_adds_cluster_deployment_step_if_needed()
    {
      _environmentInfoRepository
        .Setup(x => x.FindByName(It.IsAny<string>()))
        .Returns(DeploymentDataGenerator.GetClusteredEnvironmentInfo);

      _failoverClusterManager
        .Setup(x => x.GetPossibleNodeNames(It.IsAny<string>(), It.IsAny<string>()))
        .Returns(new[] {"node1", "node2"});

      _failoverClusterManager
        .Setup(x => x.GetCurrentNodeName(It.IsAny<string>(), It.IsAny<string>()))
        .Returns("node1");

      // act
      _deploymentTask.Prepare();
    }

  }
}