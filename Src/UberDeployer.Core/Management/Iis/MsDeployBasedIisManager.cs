using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Web.Administration;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;
using System.Text.RegularExpressions;
using UberDeployer.Core.Management.MsDeploy;

namespace UberDeployer.Core.Management.Iis
{
  public class MsDeployBasedIisManager : IIisManager
  {
    private const string _RemoteAppCmdExePath = "C:\\Windows\\system32\\inetsrv\\appcmd.exe";

    private static readonly Regex _ExitCodeRegex = new Regex("exited with code '(?<ExitCode>0x[0-9a-fA-F]+)'", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex _SiteDoesNotExistRegex = new Regex(@"(Application '[^']+' does not exist in site '[^']+'\.)|(Site '[^']+' does not exist\.)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly IMsDeploy _msDeploy;

    #region Constructor(s)

    public MsDeployBasedIisManager(IMsDeploy msDeploy)
    {
      if (msDeploy == null)
      {
        throw new ArgumentNullException("msDeploy", "Argument can't be null.");
      }

      _msDeploy = msDeploy;
    }

    #endregion

    #region IIisManager Members

    public IDictionary<string, IisAppPoolInfo> GetAppPools(string machineName)
    {
      if (string.IsNullOrEmpty(machineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "machineName");
      }

      using (ServerManager serverManager = ServerManager.OpenRemote(machineName))
      {
        return
          serverManager
            .ApplicationPools
            .Where(ap => !string.IsNullOrEmpty(ap.ManagedRuntimeVersion))
            .Select(
              ap =>
              new IisAppPoolInfo(
                ap.Name,
                GetIisAppPoolVersion(ap.ManagedRuntimeVersion),
                GetIisAppPoolMode(ap.ManagedPipelineMode)))
            .ToDictionary(
              iapi => iapi.Name,
              iapi => iapi);
      }
    }

    public bool AppPoolExists(string machineName, string appPoolName)
    {
      if (string.IsNullOrEmpty(machineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "machineName");
      }

      if (string.IsNullOrEmpty(appPoolName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "appPoolName");
      }

      IDictionary<string, IisAppPoolInfo> iisAppPoolInfos = GetAppPools(machineName);

      return iisAppPoolInfos.ContainsKey(appPoolName);
    }

    public void CreateAppPool(string machineName, IisAppPoolInfo appPoolInfo)
    {
      if (string.IsNullOrEmpty(machineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "machineName");
      }

      if (appPoolInfo == null)
      {
        throw new ArgumentNullException("appPoolInfo");
      }

      string appCmdCommand =
        string.Format(
          "add apppool /name:\"{0}\" -managedRuntimeVersion:\"{1}\" -managedPipelineMode:\"{2}\"",
          appPoolInfo.Name,
          appPoolInfo.Version.ToString().Replace("_", "."),
          appPoolInfo.Mode);

      RunAppCmd(machineName, appCmdCommand);
    }

    public void SetAppPool(string machineName, string fullWebAppName, string appPoolName)
    {
      Guard.NotNullNorEmpty(machineName, "machineName");
      Guard.NotNullNorEmpty(fullWebAppName, "fullWebAppName");
      Guard.NotNullNorEmpty(appPoolName, "appPoolName");

      if (!AppPoolExists(machineName, appPoolName))
      {
        throw new ArgumentException(string.Format("Couldn't set app's application pool to '{0}' because it doesn't exist on '{1}'.", appPoolName, machineName), "appPoolName");
      }

      string appCmdCommand;

      if (fullWebAppName.Contains("/"))
      {
        appCmdCommand =
          string.Format(
            "set app /app.name:\"{0}\" -applicationPool:\"{1}\"",
            fullWebAppName,
            appPoolName);
      }
      else
      {
        appCmdCommand =
          string.Format(
            "set site /site.name:\"{0}\" /[path='/'].applicationPool:\"{1}\"",
            fullWebAppName,
            appPoolName);
      }

      RunAppCmd(machineName, appCmdCommand);
    }

    public string GetWebApplicationPath(string machineName, string fullWebAppName)
    {
      if (string.IsNullOrEmpty(machineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "machineName");
      }

      if (string.IsNullOrEmpty(fullWebAppName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "fullWebAppName");
      }

      var msDeployArgs =
        new[]
          {
            "-verb:dump",
            string.Format("-source:appHostConfig=\"{0}\",computerName=\"{1}\"",  fullWebAppName, machineName),
            "-xml"
          };

      try
      {
        string stdout;

        _msDeploy.Run(msDeployArgs, out stdout);

        XAttribute attribute =
          XDocument.Parse(stdout)
            .Descendants("virtualDirectory")
            .Single()
            .Descendants("dirPath")
            .Single()
            .Attribute("path");

        if (attribute == null)
        {
          throw new InternalException("Couldn't get 'path' attribute.");
        }

        return attribute.Value;
      }
      catch (MsDeployException e)
      {
        if (!string.IsNullOrEmpty(e.ConsoleError) && _SiteDoesNotExistRegex.IsMatch(e.ConsoleError))
        {
          return null;
        }

        throw;
      }
    }

    #endregion

    #region Private helper methods

    private static void HandleAppCmdExitCode(string exitCodeString, string machineName, string stdout)
    {
      if (exitCodeString == "0xB7") // app pool already exists
      {
        throw new InvalidOperationException(string.Format("Error while running appcmd.exe. Can't create an application pool because it already exists on the target machine.\r\nStandard output:\r\n{0}", stdout));
      }

      if (exitCodeString == "0x32") // site doesn't exist
      {
        throw new InvalidOperationException(string.Format("Error while running appcmd.exe. Can't set an app's application pool because the app doesn't exist on the target machine.\r\nStandard output:\r\n{0}", stdout));
      }

      if (exitCodeString != "0x0") // generic error
      {
        throw new InternalException(string.Format("Error while running appcmd.exe.\r\nStandard output:\r\n{0}\r\nAlso make sure that the file '{1}' is present on the target machine ('{2}').", stdout, _RemoteAppCmdExePath, machineName));
      }
    }

    private static IisAppPoolMode GetIisAppPoolMode(ManagedPipelineMode managedPipelineMode)
    {
      switch (managedPipelineMode)
      {
        case ManagedPipelineMode.Integrated:
          return IisAppPoolMode.Integrated;

        case ManagedPipelineMode.Classic:
          return IisAppPoolMode.Classic;

        default:
          throw new NotSupportedException(string.Format("Unknown managed pipeline mode: '{0}'.", managedPipelineMode));
      }
    }

    private static IisAppPoolVersion GetIisAppPoolVersion(string managedRuntimeVersionString)
    {
      switch (managedRuntimeVersionString)
      {
        case "v1.1":
          return IisAppPoolVersion.V1_1;

        case "v2.0":
          return IisAppPoolVersion.V2_0;

        case "v4.0":
          return IisAppPoolVersion.V4_0;

        default:
          throw new NotSupportedException(string.Format("Unknown managed runtime version string: '{0}'.", managedRuntimeVersionString));
      }
    }

    private void RunAppCmd(string machineName, string appCmdArgs)
    {
      var msDeployArgs =
        new[]
          {
            "-verb:sync",
            string.Format("-source:runCommand=\"{0} {1}\"", _RemoteAppCmdExePath, appCmdArgs),
            string.Format("-dest:auto,computerName=\"{0}\"", machineName),
          };

      string stdout;

      _msDeploy.Run(msDeployArgs, out stdout);

      Match exitCodeMatch = _ExitCodeRegex.Match(stdout);
      string exitCodeString = "?";

      if (exitCodeMatch.Success)
      {
        exitCodeString = exitCodeMatch.Groups["ExitCode"].Value;
      }

      HandleAppCmdExitCode(exitCodeString, machineName, stdout);
    }

    #endregion
  }
}
