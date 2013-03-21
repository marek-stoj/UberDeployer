using System;
using NUnit.Framework;
using UberDeployer.Core.Deployment;

namespace UberDeployer.Core.Tests.Deployment
{
  [TestFixture]
  public class ExtractVersionDeploymentStepTests
  {
    [Test]
    public void version_is_extracted_from_sample_file()
    {
      ExtractVersionDeploymentStep step =
        new ExtractVersionDeploymentStep(
          new Lazy<string>(() => "TestData\\TestVersionExtract"),
          "subst.exe");

      step.PrepareAndExecute();

      Assert.AreEqual("6.1.7600.16385",step.Version);
    }
  }
}
