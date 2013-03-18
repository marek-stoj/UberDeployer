using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Tests.Generators;

namespace UberDeployer.Core.Tests.Deployment
{
  [TestFixture]
  public class ExtractVersionDeploymentStepTests
  {
    [Test]
    public void version_is_extracted_from_sample_file()
    {
      var step = new ExtractVersionDeploymentStep(new Lazy<string>(() => "TestData\\TestVersionExtract"), "subst.exe");

      step.PrepareAndExecute(DeploymentInfoGenerator.GetDbDeploymentInfo());

      Assert.AreEqual("6.1.7600.16385",step.Version);
    }
  }
}
