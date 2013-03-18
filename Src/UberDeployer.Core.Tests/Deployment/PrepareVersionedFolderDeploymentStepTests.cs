using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Tests.Generators;

namespace UberDeployer.Core.Tests.Deployment
{
  [TestFixture]
  public class PrepareVersionedFolderDeploymentStepTests
  {
    [Test]
    public void created_folder_is_named_after_version()
    {
      var step = new PrepareVersionedFolderDeploymentStep(
        "TestData/VersionedFolders",
        "TestProject",
        new Lazy<string>(() => "1.0.3.4"));

      step.PrepareAndExecute(DeploymentInfoGenerator.GetDbDeploymentInfo());

      Assert.IsTrue(Directory.Exists("TestData/VersionedFolders/TestProject/1.0.3.4"));

      Directory.Delete("TestData/VersionedFolders/TestProject/1.0.3.4");
    }

    [Test]
    public void created_folder_is_named_after_version_with_suffix_if_folder_exists()
    {
      var step = new PrepareVersionedFolderDeploymentStep(
        "TestData/VersionedFolders",
        "TestProject",
        new Lazy<string>(() => "1.0.3.5"));

      step.PrepareAndExecute(DeploymentInfoGenerator.GetDbDeploymentInfo());

      Assert.IsTrue(Directory.Exists("TestData/VersionedFolders/TestProject/1.0.3.5.1"));

      Directory.Delete("TestData/VersionedFolders/TestProject/1.0.3.5.1");
    }
  }
}
