using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Iis;
using UberDeployer.Core.Management.MsDeploy;

namespace UberDeployer.Core.Tests.Deployment
{
  [TestFixture]
  public class DeployWebAppDeploymentTaskTests
  {
    // SUT
    private DeployWebAppDeploymentTask _deplyWebAppDeploymentTask;
    private MsDeploy _msDeploy;
    private IEnvironmentInfoRepository _environmentInfoRepository;
    private IArtifactsRepository _artifactsRepository;
    private IIisManager _iisManager;

    [SetUp]
    public virtual void SetUp()
    {
      _deplyWebAppDeploymentTask = new DeployWebAppDeploymentTask(_msDeploy, _environmentInfoRepository,
                                                                  _artifactsRepository, _iisManager);
    }

    [Test]
    public void DoPrepare_should_throw_exception_when_web_machine_does_not_exist()
    {
      // Arrange
      

      // Act

      // Assert
    }
  }
}
