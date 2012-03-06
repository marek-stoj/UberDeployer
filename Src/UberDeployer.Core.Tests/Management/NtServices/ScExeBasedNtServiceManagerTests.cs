using System;
using NUnit.Framework;
using UberDeployer.Core.Management.NtServices;

namespace UberDeployer.Core.Tests.Management.NtServices
{
  [TestFixture]
  public class ScExeBasedNtServiceManagerTests
  {
    [Test]
    public void ScExeBasedNtServiceManagerConstrucotr_WhenScExePathIsNullOrEmpty_ThrowsArgumentException()
    {
      Assert.Throws<ArgumentException>(() => new ScExeBasedNtServiceManager(string.Empty, new TimeSpan(1)));
    }

    [Test]
    public void DoesServiceExist_WhenMachineNameIsNullOrEmpty_ThrowsArgumentExcpetion()
    {
      var ntServiceManager = new ScExeBasedNtServiceManager("c:\\", new TimeSpan(1));

      Assert.Throws<ArgumentException>(() => ntServiceManager.DoesServiceExist(string.Empty, "serviceName"));
    }

    [Test]
    public void DoesServiceExist_WhenServiceNameIsNullOrEmpty_ThrowsArgumentExcpetion()
    {
      var ntServiceManager = new ScExeBasedNtServiceManager("c:\\", new TimeSpan(1));

      Assert.Throws<ArgumentException>(() => ntServiceManager.DoesServiceExist("machineName", string.Empty));
    }

    [Test]
    public void InstallService_WhenMachineNameIsNullOrEmpty_ThrowsArgumentException()
    {
      var ntServiceManager = new ScExeBasedNtServiceManager("c:\\", new TimeSpan(1));

      Assert.Throws<ArgumentException>(() => ntServiceManager.InstallService(string.Empty, null));
    }

    [Test]
    public void InstallService_WhenNtServiceDescriptorIsNull_ThrowsArgumentException()
    {
      var ntServiceManager = new ScExeBasedNtServiceManager("c:\\", new TimeSpan(1));

      Assert.Throws<ArgumentNullException>(() => ntServiceManager.InstallService("machineName", null));
    }

    [Test]
    public void UninstallService_WhenMachineNameIsNullOrEmpty_ThrowsArgumentExcpetion()
    {
      var ntServiceManager = new ScExeBasedNtServiceManager("c:\\", new TimeSpan(1));

      Assert.Throws<ArgumentException>(() => ntServiceManager.UninstallService(string.Empty, "serviceName"));
    }

    [Test]
    public void UninstallService_WhenServiceNameIsNullOrEmpty_ThrowsArgumentExcpetion()
    {
      var ntServiceManager = new ScExeBasedNtServiceManager("c:\\", new TimeSpan(1));

      Assert.Throws<ArgumentException>(() => ntServiceManager.UninstallService("machineName", string.Empty));
    }

    [Test]
    public void StartService_WhenMachineNameIsNullOrEmpty_ThrowsArgumentExcpetion()
    {
      var ntServiceManager = new ScExeBasedNtServiceManager("c:\\", new TimeSpan(1));

      Assert.Throws<ArgumentException>(() => ntServiceManager.StartService(string.Empty, "serviceName"));
    }

    [Test]
    public void StartService_WhenServiceNameIsNullOrEmpty_ThrowsArgumentExcpetion()
    {
      var ntServiceManager = new ScExeBasedNtServiceManager("c:\\", new TimeSpan(1));

      Assert.Throws<ArgumentException>(() => ntServiceManager.StartService("machineName", string.Empty));
    }

    [Test]
    public void StopService_WhenMachineNameIsNullOrEmpty_ThrowsArgumentExcpetion()
    {
      var ntServiceManager = new ScExeBasedNtServiceManager("c:\\", new TimeSpan(1));

      Assert.Throws<ArgumentException>(() => ntServiceManager.StopService(string.Empty, "serviceName"));
    }

    [Test]
    public void StopService_WhenServiceNameIsNullOrEmpty_ThrowsArgumentExcpetion()
    {
      var ntServiceManager = new ScExeBasedNtServiceManager("c:\\", new TimeSpan(1));

      Assert.Throws<ArgumentException>(() => ntServiceManager.StopService("machineName", string.Empty));
    }




  }
}
