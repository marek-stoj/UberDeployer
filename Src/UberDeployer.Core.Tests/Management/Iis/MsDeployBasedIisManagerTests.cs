using System;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Iis;
using UberDeployer.Core.Management.MsDeploy;

namespace UberDeployer.Core.Tests.Management.Iis
{
  [TestFixture]
  public class MsDeployBasedIisManagerTests
  {
    private MockRepository _moqRepository;
    private Mock<IMsDeploy> _iMsDeploy;
    private MsDeployBasedIisManager _msDeployBasedIisManager;

    [SetUp]
    public void SetUp()
    {
      _moqRepository = new MockRepository(MockBehavior.Strict);
      _iMsDeploy = _moqRepository.Create<IMsDeploy>();
      _msDeployBasedIisManager = new MsDeployBasedIisManager(_iMsDeploy.Object);
    }

    #region Throwing Exception

    [Test]
    public void MsDeployBasedIisManagerConstructor_WhenIMsDeployIsNull_ThrowArgumentExcpetion()
    {
      Assert.Throws<ArgumentNullException>(() => new MsDeployBasedIisManager(null));
    }

    [Test]
    public void GetAppPools_WhenMachineNameIsNullOrEmpty_ThrowArgumentExcpetion()
    {
      // Act & Assert
      Assert.Throws<ArgumentException>(() => _msDeployBasedIisManager.GetAppPools(string.Empty));
    }

    [Test]
    public void AppPoolExists_WhenMachineNameIsNullOrEmpty_ThrowArgumentExcpetion()
    {
      // Act & Assert
      Assert.Throws<ArgumentException>(() => _msDeployBasedIisManager.AppPoolExists(string.Empty, "appPoolName"));
    }

    [Test]
    public void AppPoolExists_WhenAppPoolNameIsNullOrEmpty_ThrowArgumentExcpetion()
    {
      // Act & Assert
      Assert.Throws<ArgumentException>(() => _msDeployBasedIisManager.AppPoolExists("machineName", string.Empty));
    }

    [Test]
    public void CreateAppPool_WhenMachineNameIsNullOrEmpty_ThrowArgumentExcpetion()
    {
      // Act & Assert
      Assert.Throws<ArgumentException>(() => _msDeployBasedIisManager.CreateAppPool(string.Empty, new IisAppPoolInfo("test", IisAppPoolVersion.V4_0,IisAppPoolMode.Classic)));
    }

    [Test]
    public void CreateAppPool_WhenAppPoolInfoIsNull_ThrowArgumentNullException()
    {
      // Act & Assert
      Assert.Throws<ArgumentNullException>(() => _msDeployBasedIisManager.CreateAppPool("machineName", null));
    }

    [Test]
    public void SetAppPool_WhenMachineNameIsNullOrEmpty_ThrowArgumentExcpetion()
    {
      // Act & Assert
      Assert.Throws<ArgumentException>(() => _msDeployBasedIisManager.SetAppPool(string.Empty, "fullWebAppName", "appPoolName"));
    }

    [Test]
    public void SetAppPool_WhenFullWebAppNameIsNullOrEmpty_ThrowArgumentExcpetion()
    {
      // Act & Assert
      Assert.Throws<ArgumentException>(() => _msDeployBasedIisManager.SetAppPool("machineName", string.Empty, "appPoolName"));
    }

    [Test]
    public void SetAppPool_WhenAppPoolNameIsNullOrEmpty_ThrowArgumentExcpetion()
    {
      // Act & Assert
      Assert.Throws<ArgumentException>(() => _msDeployBasedIisManager.SetAppPool("machineName", "fullWebAppName", string.Empty));
    }

    [Test]
    public void GetWebApplicationPath_MachineNameIsNull_ThrowArgumentException()
    {
      Assert.Throws<ArgumentException>(() => _msDeployBasedIisManager.GetWebApplicationPath(string.Empty, "fullWebAppName"));
    }

    [Test]
    public void GetWebApplicationPath_FullWebAppNameIsNull_ThrowArgumentException()
    {
      Assert.Throws<ArgumentException>(() => _msDeployBasedIisManager.GetWebApplicationPath("machineName", string.Empty));
    }

    #endregion Throwing Exception
    
    [Test]
    public void GetWebApplicationPath_ApplicationNotExistOnServer_ReturnsNull()
    {
      const string consoleError = "Error: Object of type 'appHostConfig' and path 'KRD/test' cannot be created."
                                  + "Error: Application '/test' does not exist in site 'KRD'."
                                  + "Error count: 1.";
      string console;

      _iMsDeploy.Setup(imsd => imsd.Run(
        It.IsAny<string[]>(), 
        out console)).Throws(new MsDeployException(consoleError));
      
      Assert.IsNull( _msDeployBasedIisManager.GetWebApplicationPath("machineName", "fullWebApplicationName"));
    }

    [Test]
    public void GetWebApplicationPath_SiteNotExistOnServer_ReturnsNull()
    {
      const string consoleError = "Error: Object of type 'appHostConfig' and path 'KRD/test' cannot be created."
                                  + "Error: Site 'KsRD' does not exist."
                                  + "Error count: 1.";
      string console;

      _iMsDeploy.Setup(imsd => imsd.Run(
        It.IsAny<string[]>(),
        out console)).Throws(new MsDeployException(consoleError));

      Assert.IsNull(_msDeployBasedIisManager.GetWebApplicationPath("machineName", "fullWebApplicationName"));
    }

  }
}
