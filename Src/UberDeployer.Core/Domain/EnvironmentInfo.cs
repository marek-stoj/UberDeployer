using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text.RegularExpressions;

namespace UberDeployer.Core.Domain
{
  public class EnvironmentInfo
  {
    private static readonly Regex _DriveLetterRegex = new Regex(@"^(?<DriveLetter>[a-z]):\\", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    private readonly Dictionary<string, EnvironmentUser> _environmentUsersDict;

    #region Constructor(s)

    public EnvironmentInfo(
      string name,
      string configurationTemplateName,
      string appServerMachineName,
      string webServerMachineName,
      string terminalServerMachineName,
      string databaseServerMachineName,
      string ntServicesBaseDirPath,
      string webAppsBaseDirPath,
      string schedulerAppsBaseDirPath,
      string terminalAppsBaseDirPath,
      IEnumerable<EnvironmentUser> environmentUsers)
    {
      if (string.IsNullOrEmpty(name))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "name");
      }

      if (configurationTemplateName == null)
      {
        throw new ArgumentNullException("configurationTemplateName");
      }

      if (string.IsNullOrEmpty(appServerMachineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "appServerMachineName");
      }

      if (string.IsNullOrEmpty(webServerMachineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "webServerMachineName");
      }

      if (string.IsNullOrEmpty(terminalServerMachineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "terminalServerMachineName");
      }

      if (string.IsNullOrEmpty(databaseServerMachineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "databaseServerMachineName");
      }

      if (string.IsNullOrEmpty(ntServicesBaseDirPath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "ntServicesBaseDirPath");
      }

      if (string.IsNullOrEmpty(webAppsBaseDirPath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "webAppsBaseDirPath");
      }

      if (string.IsNullOrEmpty(schedulerAppsBaseDirPath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "schedulerAppsBaseDirPath");
      }

      if (string.IsNullOrEmpty(terminalAppsBaseDirPath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "terminalAppsBaseDirPath");
      }

      if (environmentUsers == null)
      {
        throw new ArgumentNullException("environmentUsers");
      }
      
      Name = name;
      ConfigurationTemplateName = configurationTemplateName;
      AppServerMachineName = appServerMachineName;
      WebServerMachineName = webServerMachineName;
      TerminalServerMachineName = terminalServerMachineName;
      DatabaseServerMachineName = databaseServerMachineName;
      NtServicesBaseDirPath = ntServicesBaseDirPath;
      WebAppsBaseDirPath = webAppsBaseDirPath;
      SchedulerAppsBaseDirPath = schedulerAppsBaseDirPath;
      TerminalAppsBaseDirPath = terminalAppsBaseDirPath;

      _environmentUsersDict = environmentUsers.ToDictionary(eu => eu.Id, eu => eu);
    }

    #endregion

    #region Public methods

    public string GetAppServerNetworkPath(string absoluteLocalPath)
    {
      return GetNetworkPath(AppServerMachineName, absoluteLocalPath);
    }

    public string GetWebServerNetworkPath(string absoluteLocalPath)
    {
      return GetNetworkPath(WebServerMachineName, absoluteLocalPath);
    }

    public string GetTerminalServerNetworkPath(string absoluteLocalPath)
    {
      return GetNetworkPath(TerminalServerMachineName, absoluteLocalPath);
    }

    public EnvironmentUser GetEnvironmentUserByName(string environmentUserName)
    {
      EnvironmentUser environmentUser;

      if (_environmentUsersDict.TryGetValue(environmentUserName, out environmentUser))
      {
        return environmentUser;
      }

      return null;
    }

    #endregion

    #region Private helper methods

    private static string GetNetworkPath(string machineName, string absoluteLocalPath)
    {
      if (string.IsNullOrEmpty(absoluteLocalPath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "absoluteLocalPath");
      }

      if (absoluteLocalPath.StartsWith(@"\\"))
      {
        throw new ArgumentException(string.Format("The path is already a network path. Absolute local path: '{0}'.", absoluteLocalPath), "absoluteLocalPath");
      }

      Match driveLetterMatch = _DriveLetterRegex.Match(absoluteLocalPath);

      if (!driveLetterMatch.Success)
      {
        throw new ArgumentException(string.Format("The path is not an absolute local path, ie. it doesn't start with a drive letter followed by a colon and a backslash. Absolute local path: '{0}'.", absoluteLocalPath), "absoluteLocalPath");
      }

      string driveLetter = driveLetterMatch.Groups["DriveLetter"].Value;

      return
        string.Format(
          "\\\\{0}\\{1}$\\{2}",
          machineName,
          driveLetter,
          absoluteLocalPath.Substring(driveLetterMatch.Length));
    }

    #endregion

    #region Properties

    public string Name { get; private set; }

    public string ConfigurationTemplateName { get; private set; }

    public string AppServerMachineName { get; private set; }

    public string WebServerMachineName { get; private set; }
    
    public string TerminalServerMachineName { get; private set; }

    public string DatabaseServerMachineName { get; private set; }

    public string NtServicesBaseDirPath { get; private set; }

    public string WebAppsBaseDirPath { get; private set; }

    public string SchedulerAppsBaseDirPath { get; private set; }

    public string TerminalAppsBaseDirPath { get; private set; }

    [Browsable(false)]
    public IEnumerable<EnvironmentUser> EnvironmentUsers
    {
      get { return _environmentUsersDict.Values; }
    }

    // TODO IMM HI: that's for UI!
    [TypeConverter(typeof(EnvironmentUsersCollectionConverter))]
    [Editor(typeof(ReadOnlyUITypeEditor), typeof(UITypeEditor))]
    public EnvironmentUsersCollection EnvironmentUsersCollection
    {
      get { return new EnvironmentUsersCollection(EnvironmentUsers); }
    }

    #endregion
  }
}
