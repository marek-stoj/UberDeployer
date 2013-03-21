using System;
using System.ServiceProcess;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Management.NtServices;

namespace UberDeployer.Core.Tests.Deployment
{
  [TestFixture]
  public class InstallNtServiceDeploymentStepTests
  {
    [Test]
    public void Test_InstallNtServiceDeploymentStep_Thows_When_Service_null()
    {
      const string machineName = "machine";

      var ntServiceDescriptor = new Mock<NtServiceDescriptor>(MockBehavior.Strict);

      Assert.Throws<ArgumentException>(
        () =>
        { new InstallNtServiceDeploymentStep(null, machineName, ntServiceDescriptor.Object); });
    }

    [Test]
    public void Test_InstallNtServiceDeploymentStep_Thows_When_MachineName_null()
    {
      var ntServiceDescriptor = new NtServiceDescriptor(
        "serviceName", "serviceExecutablePath", new ServiceAccount(), ServiceStartMode.Automatic);
      var ntServiceManager = new Mock<INtServiceManager>(MockBehavior.Strict);

      Assert.Throws<ArgumentException>(
        () =>
        { new InstallNtServiceDeploymentStep(ntServiceManager.Object, null, ntServiceDescriptor); });
    }

    [Test]
    public void Test_InstallNtServiceDeploymentStep_Thows_When_Descriptor_null()
    {
      const string machineName = "machine";
      var ntServiceManager = new Mock<INtServiceManager>(MockBehavior.Strict);

      Assert.Throws<ArgumentNullException>(
        () =>
        { new InstallNtServiceDeploymentStep(ntServiceManager.Object, machineName, null); });
    }

    [Test]
    public void Test_InstallNtServiceDeploymentStep()
    {
      const string machineName = "machine";
      var ntServiceDescriptor = new NtServiceDescriptor(
        "serviceName", "serviceExecutablePath", new ServiceAccount(), ServiceStartMode.Automatic);
      var ntServiceManager = new Mock<INtServiceManager>(MockBehavior.Strict);

      var installNTServiceStep = new InstallNtServiceDeploymentStep(ntServiceManager.Object, machineName, ntServiceDescriptor);

      ntServiceManager.Setup(k => k.InstallService(machineName, ntServiceDescriptor));

      installNTServiceStep.PrepareAndExecute();
    }
  }
}
