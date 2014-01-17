using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UberDeployer.Common;
using UberDeployer.Common.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  public class DeployUberDeployerAgentUsingConsoleAppDeploymentStep : DeploymentStep
  {
    private const string _ConsoleAppExeName = "UberDeployer.ConsoleApp.exe";

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
        Path.Combine(consoleAppDstPath, _ConsoleAppExeName);

      string consoleAppArgs =
        string.Format(
          "deploy \"{0}\" \"{1}\" \"{2}\" \"{3}\" \"{4}\"",
          _deploymentInfo.ProjectName,
          _deploymentInfo.ProjectConfigurationName,
          _deploymentInfo.ProjectConfigurationBuildId,
          _deploymentInfo.TargetEnvironmentName,
          _deploymentInfo.IsSimulation ? "simulate" : "");

      var processSecurityAttributes = new Win32Api.SECURITY_ATTRIBUTES();
      var threadSecurityAttributes = new Win32Api.SECURITY_ATTRIBUTES();
      var startupInfo = new Win32Api.STARTUPINFO();
      Win32Api.PROCESS_INFORMATION processInformation;

      processSecurityAttributes.nLength = Marshal.SizeOf(processSecurityAttributes);
      threadSecurityAttributes.nLength = Marshal.SizeOf(threadSecurityAttributes);

      Win32Api.CreateProcess(
        consoleAppExePath,
        " " + consoleAppArgs,
        ref processSecurityAttributes,
        ref threadSecurityAttributes,
        false,
        Win32Api.NORMAL_PRIORITY_CLASS,
        IntPtr.Zero,
        consoleAppDstPath,
        ref startupInfo,
        out processInformation);
    }

    public override string Description
    {
      get { return "Deploy UberDeployer.Agent using UberDeployer.ConsoleApp."; }
    }

    #endregion
  }
}
