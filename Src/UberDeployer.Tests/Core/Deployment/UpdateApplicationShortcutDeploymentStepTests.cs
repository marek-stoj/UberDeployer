using System;
using System.IO;

using NUnit.Framework;

using UberDeployer.Core.Deployment.Steps;

namespace UberDeployer.Tests.Core.Deployment
{
  [TestFixture]
  public class UpdateApplicationShortcutDeploymentStepTests
  {
    [Test]
    public void shortcut_is_created_and_named_after_terminal_app()
    {
      var step =
        new UpdateApplicationShortcutDeploymentStep(
          "Core/TestData/Shortcuts",
          new Lazy<string>(() => "Core/TestData/VersionedFolders/TestProject/1.0.3.5"),
          "FolderMattersFile.dummy",
          "TestProject");

      step.PrepareAndExecute();

      Assert.IsTrue(File.Exists("Core/TestData/Shortcuts/TestProject.lnk"));
      File.Delete("Core/TestData/ShortCuts/TestProject.lnk");
    }

    [Test]
    public void shortcut_can_be_modified()
    {
      var step =
        new UpdateApplicationShortcutDeploymentStep(
          "Core/TestData/Shortcuts",
          new Lazy<string>(() => "Core/TestData/VersionedFolders/TestProject/1.0.3.5"),
          "FolderMattersFile.dummy",
          "ExistingAppShortcut");

      step.PrepareAndExecute();

      var modifiedDate = File.GetLastWriteTime("Core/TestData/Shortcuts/ExistingAppShortcut.lnk");
      Assert.LessOrEqual((DateTime.Now - modifiedDate).TotalMilliseconds, 10);
    }
  }
}
 