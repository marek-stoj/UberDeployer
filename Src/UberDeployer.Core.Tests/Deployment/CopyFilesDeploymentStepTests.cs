using System;
using NUnit.Framework;
using UberDeployer.Core.Deployment;
using System.IO;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Tests.Generators;

namespace UberDeployer.Core.Tests.Deployment
{
  [TestFixture]
  public class CopyFilesDeploymentStepTests
  {
    [Test]
    public void Test_copying_all_files()
    {
      const string srcDirPath = "TestData\\TestSrcDir";
      const string dstDirPath = "TestData\\TestDstDir";

      try
      {
        var copyFilesDeploymentStep = new CopyFilesDeploymentStep(new Lazy<string>(() =>srcDirPath), new Lazy<string>(() => dstDirPath));

        copyFilesDeploymentStep.PrepareAndExecute(DeploymentInfoGenerator.GetNtServiceDeploymentInfo());

        Assert.IsTrue(Directory.Exists(dstDirPath));

        Assert.AreEqual(
          Directory.GetFiles(srcDirPath, "*.*", SearchOption.AllDirectories).Length,
          Directory.GetFiles(dstDirPath, "*.*", SearchOption.AllDirectories).Length);
      }
      finally
      {
        if (Directory.Exists(dstDirPath))
        {
          Directory.Delete(dstDirPath, true);
        }
      }
    }

    [Test]
    public void Test_copying_all_files_when_dst_exists()
    {
      const string srcDirPath = "TestData\\TestSrcDir";
      const string dstDirPath = "TestData\\TestDstDir";

      try
      {
        Directory.CreateDirectory(dstDirPath);

        var copyFilesDeploymentStep = new CopyFilesDeploymentStep(new Lazy<string>(() =>srcDirPath), new Lazy<string>(() =>dstDirPath));

        copyFilesDeploymentStep.PrepareAndExecute(DeploymentInfoGenerator.GetNtServiceDeploymentInfo());

        Assert.IsTrue(Directory.Exists(dstDirPath));

        Assert.AreEqual(
          Directory.GetFiles(srcDirPath, "*.*", SearchOption.AllDirectories).Length,
          Directory.GetFiles(dstDirPath, "*.*", SearchOption.AllDirectories).Length);
      }
      finally
      {
        if (Directory.Exists(dstDirPath))
        {
          Directory.Delete(dstDirPath, true);
        }
      }
    }


    [Test]
    public void Test_copying_all_files_throws_when_no_src()
    {
      const string srcDirPath = "TestData\\aoisdiasyd";
      const string dstDirPath = "TestData\\TestDstDir";

      try
      {
        var copyFilesDeploymentStep = new CopyFilesDeploymentStep(new Lazy<string>(() => srcDirPath), new Lazy<string>(() =>dstDirPath));

        Assert.Throws<DeploymentTaskException>(() => copyFilesDeploymentStep.PrepareAndExecute(DeploymentInfoGenerator.GetNtServiceDeploymentInfo()));
      }
      finally
      {
        if (Directory.Exists(dstDirPath))
        {
          Directory.Delete(dstDirPath, true);
        }
      }
    }

    [Test]
    public void Test_copying_all_files_throws_when_src_null()
    {
      const string dstDirPath = "TestData\\TestDstDir";

      Assert.Throws<ArgumentNullException>(() => { new CopyFilesDeploymentStep(null, new Lazy<string>(() =>dstDirPath)); });
    }

    [Test]
    public void Test_copying_all_files_throws_when_dst_null()
    {
      const string srcDirPath = "TestData\\aoisdiasyd";

      Assert.Throws<ArgumentNullException>(() => { new CopyFilesDeploymentStep(new Lazy<string>(() => srcDirPath), null); });
    }
  }
}
