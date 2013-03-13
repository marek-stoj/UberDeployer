﻿using System;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.Deployment.Pipeline;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Tests.Deployment.Pipeline
{
  [TestFixture]
  public class DeploymentPipelineTests
  {
    [Test]
    public void AddModule_WhenModuleIsNull_ThrowsArgumentNullException()
    {
      var pipeline = new DeploymentPipeline();

      Assert.Throws<ArgumentNullException>(() => pipeline.AddModule(null));
    }

    [Test]
    public void StartDeployment_WhenDeploymentTaskIsNull_ThrowsArgumentNullException()
    {
      var environmentInfoRepositoryFake = new Mock<IEnvironmentInfoRepository>(MockBehavior.Loose);

      var pipeline = new DeploymentPipeline();

      var deploymentInfoFake = new Mock<DeploymentInfo>();

      Assert.Throws<ArgumentNullException>(() => pipeline.StartDeployment(null, new DummyDeploymentTask(environmentInfoRepositoryFake.Object), new DeploymentContext("requester")));
      Assert.Throws<ArgumentNullException>(() => pipeline.StartDeployment(deploymentInfoFake.Object, null, new DeploymentContext("requester")));
      Assert.Throws<ArgumentNullException>(() => pipeline.StartDeployment(deploymentInfoFake.Object, new DummyDeploymentTask(environmentInfoRepositoryFake.Object), null));
    }
  }
}
