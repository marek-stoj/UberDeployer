using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UberDeployer.Core.Deployment.Pipeline;

namespace UberDeployer.Core.Tests.Deployment.Pipeline
{
  [TestFixture]
  public class DeploymentPipelineTests
  {
    [Test]
    public void AddModule_WhenModuleIsNull_ThrowsArgumentNullException()
    {
      var pipeline = new DeploymentPipeline();
      
      Assert.Throws<ArgumentNullException>(()=> pipeline.AddModule(null));
    }

    [Test]
    public void StartDeployment_WhenDeploymentTaskIsNull_ThrowsArgumentNullException()
    {
      var pipeline = new DeploymentPipeline();

      Assert.Throws<ArgumentNullException>(() => pipeline.StartDeployment(null));
    }
  }
}
