using System;
using System.IO;
using System.Threading;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Tests.Deployment
{
  [TestFixture]
  public class BackupFilesDeploymentStepTests
  {
    private string _workingDir;
    private const string DstSubDir = "TestDstDir";
    private string _dstDir;

    private Mock<DeploymentInfo> _deploymentInfoFake;

    [SetUp]
    public void SetUp()
    {
      _workingDir = Path.Combine(Environment.CurrentDirectory, Guid.NewGuid().ToString());
      _dstDir = string.Format("{0}{1}{2}", _workingDir, Path.DirectorySeparatorChar, DstSubDir);
      
      Directory.CreateDirectory(_dstDir);

      _deploymentInfoFake = new Mock<DeploymentInfo>();
    }

    [TearDown]
    public void Finish()
    {
      Directory.Delete(_workingDir, true);
    }

    [Test]
    public void Constructor_EmptyPath_Throws()
    {
      Assert.Throws<ArgumentException>(() => new BackupFilesDeploymentStep(""));
    }

    [Test]
    public void Execute_PathDoesNotExist_Throws()
    {
      Assert.Throws<InvalidOperationException>(
        () =>
          new BackupFilesDeploymentStep(Path.Combine(_workingDir, "28947289374298"))
            .Execute());
    }

    [Test]
    public void Execute_NoPreviousBackup_CreatesZippedFileWithNameCreatedFromLastDir()
    {
      string zippedBackupFileName = Path.Combine(Environment.CurrentDirectory, string.Format("{0}\\{1}.bak.000.zip", _workingDir, DstSubDir));
      var backupFilesDeploymentStep = new BackupFilesDeploymentStep(_dstDir);

      backupFilesDeploymentStep.Prepare(_deploymentInfoFake.Object);
      backupFilesDeploymentStep.Execute();

      var fileInfo = new FileInfo(zippedBackupFileName);

      Assert.IsTrue(fileInfo.Exists);
    }

    [Test]
    public void Execute_PreviousBackupExistNoRotationFunctionality_OverridesZippedFileWithNameCreatedFromLastDir()
    {
      string zippedBackupFileName = Path.Combine(Environment.CurrentDirectory, string.Format("{0}\\{1}.bak.000.zip", _workingDir, DstSubDir));
      var backupFilesDeploymentStep = new BackupFilesDeploymentStep(_dstDir);

      backupFilesDeploymentStep.Prepare(_deploymentInfoFake.Object);
      backupFilesDeploymentStep.Execute();
      backupFilesDeploymentStep.Execute();

      var fileInfo = new FileInfo(zippedBackupFileName);

      Assert.IsTrue(fileInfo.Exists);
    }

    [Test]
    public void Execute_PreviousBackupExistAndFilesRotationOn_CreatesNewBackupAndMovingOldOne()
    {
      const int maxBackupCount = 4;
      var backupFilesDeploymentStep = new BackupFilesDeploymentStep(_dstDir, maxBackupCount);

      backupFilesDeploymentStep.Prepare(_deploymentInfoFake.Object);

      for (int i = 0; i < maxBackupCount + 1; i++)
      {
        backupFilesDeploymentStep.Execute();

        Thread.Sleep(50);
      }

      string[] files = Directory.GetFiles(_workingDir);
      Array.Sort(files);
    
      for (int fileIndex = 0; fileIndex < files.Length - 2; fileIndex++)
      {
        Assert.Greater(new FileInfo(files[fileIndex]).LastWriteTime, new FileInfo(files[fileIndex + 1]).LastWriteTime);
      }

      Assert.AreEqual(4, files.Length);
    }
  }
}
