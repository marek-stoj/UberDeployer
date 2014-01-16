using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UberDeployer.Common;
using UberDeployer.Common.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  public class DeployUberDeployerAgentUsingConsoleAppDeploymentStep : DeploymentStep
  {
    private readonly DeploymentInfo _deploymentInfo;
    private readonly IDirectoryAdapter _directoryAdapter;

    #region Constructor(s)

    public DeployUberDeployerAgentUsingConsoleAppDeploymentStep(DeploymentInfo deploymentInfo, IDirectoryAdapter directoryAdapter)
    {
      Guard.NotNull(deploymentInfo, "deploymentInfo");
      Guard.NotNull(directoryAdapter, "directoryAdapter");

      _deploymentInfo = deploymentInfo;
      _directoryAdapter = directoryAdapter;
    }

    #endregion

    #region Overrides of DeploymentStep

    protected override void DoExecute()
    {
      string tempDirName = Guid.NewGuid().ToString("N");
      string tempDirPath = Path.Combine(Path.GetTempPath(), tempDirName);

      _directoryAdapter.CreateDirectory(tempDirPath);

      string currentPath =
        Path.GetDirectoryName(
          ReflectionUtils.GetCodeBaseFilePath(Assembly.GetExecutingAssembly()));

      if (string.IsNullOrEmpty(currentPath))
      {
        throw new InternalException("Couldn't get current path.");
      }

      string consoleAppSrcPath = Path.Combine(currentPath, "UberDeployer.ConsoleApp");
      string consoleAppDstPath = tempDirPath;

      _directoryAdapter.CopyAll(consoleAppSrcPath, consoleAppDstPath);

      string consoleAppExePath =
        Path.Combine(consoleAppDstPath, "UberDeployer.ConsoleApp.exe");

      string consoleAppArgs =
        string.Format(
          "deploy {0} {1} {2} {3} {4}",
          _deploymentInfo.ProjectName,
          _deploymentInfo.ProjectConfigurationName,
          _deploymentInfo.ProjectConfigurationBuildId,
          _deploymentInfo.TargetEnvironmentName,
          _deploymentInfo.IsSimulation ? "simulate" : "");

      ProcessStartInfo processStartInfo =
        new ProcessStartInfo(
          consoleAppExePath,
          consoleAppArgs);

      processStartInfo.CreateNoWindow = true;
      processStartInfo.UseShellExecute = false;
      processStartInfo.WorkingDirectory = consoleAppDstPath;

      Process.Start(processStartInfo);
    }

    public override string Description
    {
      get { return "Deploy UberDeployer.Agent using UberDeployer.ConsoleApp."; }
    }

    #endregion
  }
}
