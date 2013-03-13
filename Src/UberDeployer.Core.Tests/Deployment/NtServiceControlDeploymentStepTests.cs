using System;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.NtServices;
using UberDeployer.Core.Tests.Generators;

namespace UberDeployer.Core.Tests.Deployment
{
  [TestFixture]
  public class NtServiceControlDeploymentStepTests
  {
    private DeploymentInfo _deploymentInfo;

    [SetUp]
    public void SetUp()
    {
      _deploymentInfo = DeploymentInfoGenerator.GetNtServiceDeploymentInfo();
    }

    [Test]
    public void Test_StartNtServiceDeploymentStep_Thows_When_Service_null()
    {
      const string machineName = "machine";
      const string serviceName = "service";

      Assert.Throws<ArgumentNullException>(
        () =>
          { new StartNtServiceDeploymentStep(null, machineName, serviceName); });
    }

    [Test]
    public void Test_StartNtServiceDeploymentStep_Thows_When_MachineName_null()
    {
      const string scPath = "path";
      var time = new TimeSpan(1000);
      const string serviceName = "service";

      Assert.Throws<ArgumentException>(
        () =>
        { new StartNtServiceDeploymentStep(new ScExeBasedNtServiceManager(scPath, time), null, serviceName); });
    }

    [Test]
    public void Test_StartNtServiceDeploymentStep_Thows_When_ServiceName_null()
    {
      const string scPath = "path";
      var time = new TimeSpan(1000);
      const string machineName = "machine";

      Assert.Throws<ArgumentException>(
        () =>
        { new StartNtServiceDeploymentStep(new ScExeBasedNtServiceManager(scPath, time), machineName, null); });
    }

    [Test]
    public void Test_StartNtServiceDeploymentStep()
    {
      var ntServiceManager = new Mock<INtServiceManager>(MockBehavior.Strict); 

      const string machineName = "machine";
      const string serviceName = "service";
      
      var startNTServiceStep = new StartNtServiceDeploymentStep(ntServiceManager.Object, machineName, serviceName);

      ntServiceManager.Setup(k => k.StartService(machineName, serviceName));

      startNTServiceStep.PrepareAndExecute(_deploymentInfo);
    }

    [Test]
    public void Test_StopNtServiceDeploymentStep()
    {
      var ntServiceManager = new Mock<INtServiceManager>(MockBehavior.Strict);

      const string machineName = "machine";
      const string serviceName = "service";

      var startNTServiceStep = new StopNtServiceDeploymentStep(ntServiceManager.Object, machineName, serviceName);

      ntServiceManager.Setup(k => k.StopService(machineName, serviceName));

      startNTServiceStep.PrepareAndExecute(_deploymentInfo);
    }
  }
}
