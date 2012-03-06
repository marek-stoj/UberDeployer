using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UberDeployer.Core.Management.NtServices;
using System.ServiceProcess;

namespace UberDeployer.Core.Tests.Management.NtServices
{
  [TestFixture]
  public class NtServiceDescriptorTests
  {
    [Test]
    public void NtServiceDescriptorConstructor_WhenServiceNameIsNullOrEmpty_ThrowArgumentException()
    {
      Assert.Throws<ArgumentException>(() => new NtServiceDescriptor(string.Empty,
                                                                     string.Empty,
                                                                     ServiceAccount.LocalService,
                                                                     ServiceStartMode.Automatic));
    }

    [Test]
    public void NtServiceDescriptorConstructor_WhenServiceExecutablePathIsNullOrEmpty_ThrowArgumentException()
    {
      Assert.Throws<ArgumentException>(() => new NtServiceDescriptor("ServiceName",
                                                                     string.Empty,
                                                                     ServiceAccount.LocalService,
                                                                     ServiceStartMode.Automatic));
    }

    [Test]
    public void NtServiceDescriptorConstructor_WhenEverythinkIsOk_AllPropertiesHavaCorrectValues()
    {
      // Arrange
      string serviceName = "serviceName";
      string serviceExecutablePath = "serviceExecutablePath";
      ServiceAccount serviceAccount = ServiceAccount.LocalSystem;
      ServiceStartMode serviceStartMode = ServiceStartMode.Automatic;
      string serviceDisplayName = "serviceDisplayName";
      string serviceUserName = "serviceUserName";
      string servicePassword = "servicePassword";

      // Act
      var ntServiceDescriptor = new NtServiceDescriptor(serviceName, 
                                                        serviceExecutablePath, 
                                                        serviceAccount, 
                                                        serviceStartMode, 
                                                        serviceDisplayName,
                                                        serviceUserName, 
                                                        servicePassword);

      // Assert
      Assert.AreEqual(serviceName, ntServiceDescriptor.ServiceName);
      Assert.AreEqual(serviceExecutablePath, ntServiceDescriptor.ServiceExecutablePath);
      Assert.AreEqual(serviceAccount, ntServiceDescriptor.ServiceAccount);
      Assert.AreEqual(serviceStartMode, ntServiceDescriptor.ServiceStartMode);
      Assert.AreEqual(serviceDisplayName, ntServiceDescriptor.ServiceDisplayName);
      Assert.AreEqual(serviceUserName, ntServiceDescriptor.ServiceUserName);
      Assert.AreEqual(servicePassword, ntServiceDescriptor.ServicePassword);
    }
  }
}
