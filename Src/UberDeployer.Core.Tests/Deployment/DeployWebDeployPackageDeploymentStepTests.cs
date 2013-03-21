using System;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Management.MsDeploy;

namespace UberDeployer.Core.Tests.Deployment
{
  public class DeployWebDeployPackageDeploymentStepTests
  {
    private MockRepository _mockRepository;
    private Mock<IMsDeploy> _msDeploy;

    private const string _WebServerMachineName = "machine_name";
    private const string _PackageFilePath = "/path";

    [SetUp]
    public void SetUp()
    {
      _mockRepository = new MockRepository(MockBehavior.Strict);
      _msDeploy = _mockRepository.Create<IMsDeploy>();
    }

    [TearDown]
    public void Final()
    {
      _mockRepository.VerifyAll();
    }

    [Test]
    public void Constructor_NullForMsDeploy_ThrowsException()
    {
      Assert.Throws<ArgumentNullException>(() => new DeployWebDeployPackageDeploymentStep(null, _WebServerMachineName, new Lazy<string>(() => _PackageFilePath)));
    }

    [Test]
    public void Constructor_NullForPackageFilePathProvider_ThrowsException()
    {
      Assert.Throws<ArgumentNullException>(() => new DeployWebDeployPackageDeploymentStep(_msDeploy.Object, _WebServerMachineName, null));
    }

    [Test]
    public void Constructor_NullForWebServerMachineName_ThrowsException()
    {
      Assert.Throws<ArgumentException>(() => new DeployWebDeployPackageDeploymentStep(_msDeploy.Object, null, new Lazy<string>(() => _PackageFilePath)));
    }

    [Test]
    public void DoExecute_RunsMsDeploy()
    {
      var deployWebDeployPackageDeploymentStep = new DeployWebDeployPackageDeploymentStep(_msDeploy.Object, _WebServerMachineName, new Lazy<string>(() => _PackageFilePath));

      string outString;
      _msDeploy.Setup(mD => mD.Run(It.IsAny<string[]>(), out outString));

      deployWebDeployPackageDeploymentStep.PrepareAndExecute();
    }
  }
}
