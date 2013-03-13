using System;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.MsDeploy;

namespace UberDeployer.Core.Tests.Deployment
{
  public class DeployWebDeployPackageDeploymentStepTests
  {
    private MockRepository _mockRepository;
    private Mock<IMsDeploy> _msDeploy;
    private Mock<DeploymentInfo> _deploymentInfoFake;

    private const string _WebServerMachineName = "machine_name";
    private const string _PackageFilePath = "/path";

    [SetUp]
    public void SetUp()
    {
      _mockRepository = new MockRepository(MockBehavior.Strict);
      _msDeploy = _mockRepository.Create<IMsDeploy>();
      _deploymentInfoFake = new Mock<DeploymentInfo>();
    }

    [TearDown]
    public void Final()
    {
      _mockRepository.VerifyAll();
    }

    [Test]
    public void Constructor_NullForMsDeploy_ThrowsException()
    {
      Assert.Throws<ArgumentException>(() => new DeployWebDeployPackageDeploymentStep(null, _PackageFilePath, _WebServerMachineName));
    }

    [Test]
    public void Constructor_NullForPackageFilePath_ThrowsException()
    {
      Assert.Throws<ArgumentException>(() => new DeployWebDeployPackageDeploymentStep(_msDeploy.Object, null, _WebServerMachineName));
    }

    [Test]
    public void Constructor_NullForWebServerMachineName_ThrowsException()
    {
      Assert.Throws<ArgumentException>(() => new DeployWebDeployPackageDeploymentStep(_msDeploy.Object, _PackageFilePath, null));
    }

    [Test]
    public void DoExecute_RunsMsDeploy()
    {
      var deployWebDeployPackageDeploymentStep = new DeployWebDeployPackageDeploymentStep(_msDeploy.Object, _PackageFilePath, _WebServerMachineName);

      string outString;
      _msDeploy.Setup(mD => mD.Run(It.IsAny<string[]>(), out outString));

      deployWebDeployPackageDeploymentStep.PrepareAndExecute(_deploymentInfoFake.Object);
    }
  }
}
