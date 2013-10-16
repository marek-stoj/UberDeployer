using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.IO;
using System.Text.RegularExpressions;

namespace UberDeployer.Core.Management.NtServices
{
  public class ScExeBasedNtServiceManager : INtServiceManager
  {
    private static readonly Regex _QueryServiceNameRegex = new Regex(@"^SERVICE_NAME:\s*(?<ServiceName>[^\r\n]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

    private readonly TimeSpan _operationsTimeout;
    private readonly string _scExePath;

    #region Constructor(s)

    public ScExeBasedNtServiceManager(string scExePath, TimeSpan operationsTimeout)
    {
      if (string.IsNullOrEmpty(scExePath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "scExePath");
      }

      _scExePath = scExePath;
      _operationsTimeout = operationsTimeout;
    }

    #endregion

    #region INtServiceManager members

    public bool DoesServiceExist(string machineName, string serviceName)
    {
      if (string.IsNullOrEmpty(machineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "machineName");
      }
      
      if (string.IsNullOrEmpty(serviceName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "serviceName");
      }

      string args =
        string.Format(
          "\"\\\\{0}\" query \"{1}\"",
          machineName,
          serviceName);

      string stdOut;
      string stdErr;

      int processExitCode = RunScExe(args, out stdOut, out stdErr);

      if (processExitCode == 1060)
      {
        return false;
      }

      if (processExitCode != 0)
      {
        throw CreateScExeInternalException(processExitCode, stdOut, stdErr);
      }

      stdOut = stdOut.Replace("\r", "");

      return _QueryServiceNameRegex.IsMatch(stdOut);
    }

    public void InstallService(string machineName, NtServiceDescriptor ntServiceDescriptor)
    {
      if (string.IsNullOrEmpty(machineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "machineName");
      }

      if (ntServiceDescriptor == null)
      {
        throw new ArgumentNullException("ntServiceDescriptor");
      }

      string args =
        string.Format(
          "\\\\{0} create \"{1}\" displayName= \"{2}\" binPath= \"{3}\" start= \"{4}\"",
          machineName,
          ntServiceDescriptor.ServiceName,
          ntServiceDescriptor.ServiceDisplayName,
          ntServiceDescriptor.ServiceExecutablePath,
          ConvertSreviceStartModeToString(ntServiceDescriptor.ServiceStartMode));

      if (!string.IsNullOrEmpty(ntServiceDescriptor.ServiceUserName))
      {
        args += string.Format(" obj= \"{0}\"", ntServiceDescriptor.ServiceUserName);
      }

      if (!string.IsNullOrEmpty(ntServiceDescriptor.ServicePassword))
      {
        args += string.Format(" password= \"{0}\"", ntServiceDescriptor.ServicePassword);
      }

      string stdOut;
      string stdErr;

      int processExitCode = RunScExe(args, out stdOut, out stdErr);

      if (processExitCode != 0)
      {
        throw CreateScExeInternalException(processExitCode, stdOut, stdErr);
      }
    }

    public void UninstallService(string machineName, string serviceName)
    {
      if (string.IsNullOrEmpty(machineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "machineName");
      }

      if (string.IsNullOrEmpty(serviceName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "serviceName");
      }

      string args =
        string.Format(
          "\\\\{0} delete \"{1}\"",
          machineName,
          serviceName);

      string stdOut;
      string stdErr;

      int processExitCode = RunScExe(args, out stdOut, out stdErr);

      if (processExitCode != 0)
      {
        throw CreateScExeInternalException(processExitCode, stdOut, stdErr);
      }
    }

    public void StartService(string machineName, string serviceName)
    {
      if (string.IsNullOrEmpty(machineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "machineName");
      }

      if (string.IsNullOrEmpty(serviceName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "serviceName");
      }

      using (var serviceController = CreateServiceController(machineName, serviceName))
      {
        // TODO IMM HI: what about other states?
        if (serviceController.Status != ServiceControllerStatus.Running)
        {
          serviceController.Start();
        }

        serviceController.WaitForStatus(ServiceControllerStatus.Running, _operationsTimeout);
      }
    }

    public void StopService(string machineName, string serviceName)
    {
      if (string.IsNullOrEmpty(machineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "machineName");
      }

      if (string.IsNullOrEmpty(serviceName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "serviceName");
      }

      using (var serviceController = CreateServiceController(machineName, serviceName))
      {
        // TODO IMM HI: what about other states?
        if (serviceController.Status != ServiceControllerStatus.Stopped)
        {
          serviceController.Stop();
        }

        serviceController.WaitForStatus(ServiceControllerStatus.Stopped, _operationsTimeout);
      }
    }

    #endregion

    #region Private helper methods

    private static ServiceController CreateServiceController(string machineName, string serviceName)
    {
      return new ServiceController(serviceName, machineName);
    }

    private Exception CreateScExeInternalException(int processExitCode, string stdOut, string stdErr)
    {
      stdOut = (stdOut ?? "").Trim();
      stdErr = (stdErr ?? "").Trim();

      return
        new InternalException(
          string.Format(
            "Program '{0}' exited with exit code '{1}'.\r\nStandard output:\r\n-----\r\n{2}\r\n-----\r\nStandard error:\r\n-----{3}\r\n-----",
            Path.GetFileName(_scExePath),
            processExitCode,
            stdOut,
            stdErr));
    }

    private static string ConvertSreviceStartModeToString(ServiceStartMode startType)
    {
      switch (startType)
      {
        case ServiceStartMode.Automatic:
          return "auto";

        case ServiceStartMode.Disabled:
          return "disabled";

        case ServiceStartMode.Manual:
          return "manual";

        default:
          throw new NotSupportedException(string.Format("Service start mode '{0}' is not supported.", startType));
      }
    }

    private int RunScExe(string args, out string stdOut, out string stdErr)
    {
      using (var process = new Process())
      {
        var processStartInfo = CreateProcessStartInfo(args);

        process.StartInfo = processStartInfo;
        process.Start();

        stdOut = process.StandardOutput.ReadToEnd();
        stdErr = process.StandardError.ReadToEnd();

        process.WaitForExit();

        return process.ExitCode;
      }
    }

    private ProcessStartInfo CreateProcessStartInfo(string args)
    {
      return
        new ProcessStartInfo(_scExePath, args)
          {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
          };
    }

    #endregion
  }
}
