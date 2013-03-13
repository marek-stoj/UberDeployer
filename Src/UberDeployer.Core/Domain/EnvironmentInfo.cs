using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Core.Domain
{
  public class EnvironmentInfo
  {
    private static readonly Regex _DriveLetterRegex = new Regex(@"^(?<DriveLetter>[a-z]):\\", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly List<String> _webServerMachines;
    private readonly Dictionary<string, EnvironmentUser> _environmentUsersDict;
    private readonly Dictionary<string, IisAppPoolInfo> _appPoolInfosDict;
    private readonly Dictionary<string, ProjectToWebSiteMapping> _projectToWebSiteMappingsDict;
    private readonly Dictionary<string, ProjectToAppPoolMapping> _projectToAppPoolMappingsDict;
    private readonly Dictionary<string, ProjectToFailoverClusterGroupMapping> _projectToFailoverClusterGroupMappingsDict;

    #region Constructor(s)

    public EnvironmentInfo(
      string name,
      string configurationTemplateName,
      string appServerMachineName,
      string failoverClusterMachineName,
      IEnumerable<string> webServerMachineNames,
      string terminalServerMachineName,
      string databaseServerMachineName,
      string ntServicesBaseDirPath,
      string webAppsBaseDirPath,
      string schedulerAppsBaseDirPath,
      string terminalAppsBaseDirPath,
      bool enableFailoverClusteringForNtServices,
      IEnumerable<EnvironmentUser> environmentUsers,
      IEnumerable<IisAppPoolInfo> appPoolInfos,
      IEnumerable<ProjectToWebSiteMapping> projectToWebSiteMappings,
      IEnumerable<ProjectToAppPoolMapping> projectToAppPoolMappings,
      IEnumerable<ProjectToFailoverClusterGroupMapping> projectToFailoverClusterGroupMappings)
    {
      Guard.NotNullNorEmpty(name, "name");
      Guard.NotNull(configurationTemplateName, "configurationTemplateName");
      Guard.NotNullNorEmpty(appServerMachineName, "appServerMachineName");
      Guard.NotNull(failoverClusterMachineName, "failoverClusterMachineName");
      
      if (webServerMachineNames == null)
      {
        throw new ArgumentNullException("webServerMachineNames");
      }

      Guard.NotNullNorEmpty(terminalServerMachineName, "terminalServerMachineName");
      Guard.NotNullNorEmpty(databaseServerMachineName, "databaseServerMachineName");
      Guard.NotNullNorEmpty(ntServicesBaseDirPath, "ntServicesBaseDirPath");
      Guard.NotNullNorEmpty(webAppsBaseDirPath, "webAppsBaseDirPath");
      Guard.NotNullNorEmpty(schedulerAppsBaseDirPath, "schedulerAppsBaseDirPath");
      Guard.NotNullNorEmpty(terminalAppsBaseDirPath, "terminalAppsBaseDirPath");

      if (environmentUsers == null)
      {
        throw new ArgumentNullException("environmentUsers");
      }

      if (appPoolInfos == null)
      {
        throw new ArgumentNullException("appPoolInfos");
      }

      if (enableFailoverClusteringForNtServices && string.IsNullOrEmpty(failoverClusterMachineName))
      {
        throw new ArgumentException("If enableFailoverClusteringForNtServices is set, failoverClusterMachineName must not be empty.", "enableFailoverClusteringForNtServices");
      }

      if (projectToWebSiteMappings == null)
      {
        throw new ArgumentNullException("projectToWebSiteMappings");
      }

      if (projectToAppPoolMappings == null)
      {
        throw new ArgumentNullException("projectToAppPoolMappings");
      }

      if (projectToFailoverClusterGroupMappings == null)
      {
        throw new ArgumentNullException("projectToFailoverClusterGroupMappings");
      }

      Name = name;
      ConfigurationTemplateName = configurationTemplateName;
      AppServerMachineName = appServerMachineName;
      FailoverClusterMachineName = failoverClusterMachineName;
      _webServerMachines = new List<string>(webServerMachineNames);
      TerminalServerMachineName = terminalServerMachineName;
      DatabaseServerMachineName = databaseServerMachineName;
      NtServicesBaseDirPath = ntServicesBaseDirPath;
      WebAppsBaseDirPath = webAppsBaseDirPath;
      SchedulerAppsBaseDirPath = schedulerAppsBaseDirPath;
      TerminalAppsBaseDirPath = terminalAppsBaseDirPath;
      EnableFailoverClusteringForNtServices = enableFailoverClusteringForNtServices;

      _environmentUsersDict = environmentUsers.ToDictionary(eu => eu.Id);
      _appPoolInfosDict = appPoolInfos.ToDictionary(api => api.Name);

      _projectToWebSiteMappingsDict = projectToWebSiteMappings.ToDictionary(ptfcgm => ptfcgm.ProjectName);
      _projectToAppPoolMappingsDict = projectToAppPoolMappings.ToDictionary(ptfcgm => ptfcgm.ProjectName);
      _projectToFailoverClusterGroupMappingsDict = projectToFailoverClusterGroupMappings.ToDictionary(ptfcgm => ptfcgm.ProjectName);
    }

    #endregion

    #region Public methods

    public static string GetNetworkPath(string machineName, string absoluteLocalPath)
    {
      Guard.NotNullNorEmpty(machineName, "machineName");
      Guard.NotNullNorEmpty(absoluteLocalPath, "absoluteLocalPath");

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

    public string GetAppServerNetworkPath(string absoluteLocalPath)
    {
      Guard.NotNullNorEmpty(absoluteLocalPath, "absoluteLocalPath");

      return GetNetworkPath(AppServerMachineName, absoluteLocalPath);
    }

    public string GetWebServerNetworkPath(string webServerMachineName, string absoluteLocalPath)
    {
      Guard.NotNullNorEmpty(webServerMachineName, "webServerMachineName");
      Guard.NotNullNorEmpty(absoluteLocalPath, "absoluteLocalPath");

      return GetNetworkPath(webServerMachineName, absoluteLocalPath);
    }

    public string GetTerminalServerNetworkPath(string absoluteLocalPath)
    {
      Guard.NotNullNorEmpty(absoluteLocalPath, "absoluteLocalPath");

      return GetNetworkPath(TerminalServerMachineName, absoluteLocalPath);
    }

    public EnvironmentUser GetEnvironmentUserByName(string environmentUserName)
    {
      Guard.NotNullNorEmpty(environmentUserName, "environmentUserName");

      EnvironmentUser environmentUser;

      if (_environmentUsersDict.TryGetValue(environmentUserName, out environmentUser))
      {
        return environmentUser;
      }

      return null;
    }

    public string GetWebSiteNameForProject(string projectName)
    {
      Guard.NotNullNorEmpty(projectName, "projectName");

      ProjectToWebSiteMapping mapping;

      if (_projectToWebSiteMappingsDict.TryGetValue(projectName, out mapping))
      {
        return mapping.WebSiteName;
      }

      return null;
    }

    public IisAppPoolInfo GetAppPoolInfoForProject(string projectName)
    {
      Guard.NotNullNorEmpty(projectName, "projectName");

      ProjectToAppPoolMapping mapping;

      if (_projectToAppPoolMappingsDict.TryGetValue(projectName, out mapping))
      {
        IisAppPoolInfo appPoolInfo;

        if (_appPoolInfosDict.TryGetValue(mapping.AppPoolId, out appPoolInfo))
        {
          return appPoolInfo;
        }
      }

      return null;
    }

    public string GetFailoverClusterGroupNameForProject(string projectName)
    {
      Guard.NotNullNorEmpty(projectName, "projectName");

      ProjectToFailoverClusterGroupMapping mapping;

      if (_projectToFailoverClusterGroupMappingsDict.TryGetValue(projectName, out mapping))
      {
        return mapping.ClusterGroupName;
      }

      return null;
    }

    #endregion

    #region Properties

    public string Name { get; private set; }

    public string ConfigurationTemplateName { get; private set; }

    public string AppServerMachineName { get; private set; }

    public string FailoverClusterMachineName { get; private set; }

    public IEnumerable<string> WebServerMachineNames
    {
      get { return _webServerMachines.AsReadOnly(); }
    }

    public string TerminalServerMachineName { get; private set; }

    public string DatabaseServerMachineName { get; private set; }

    public string NtServicesBaseDirPath { get; private set; }

    public string WebAppsBaseDirPath { get; private set; }

    public string SchedulerAppsBaseDirPath { get; private set; }

    public string TerminalAppsBaseDirPath { get; private set; }

    public bool EnableFailoverClusteringForNtServices { get; private set; }

    public IEnumerable<EnvironmentUser> EnvironmentUsers
    {
      get { return _environmentUsersDict.Values; }
    }

    public IEnumerable<IisAppPoolInfo> AppPoolInfos
    {
      get { return _appPoolInfosDict.Values; }
    }

    public IEnumerable<ProjectToWebSiteMapping> ProjectToWebSiteMappings
    {
      get { return _projectToWebSiteMappingsDict.Values; }
    }

    public IEnumerable<ProjectToAppPoolMapping> ProjectToAppPoolMappings
    {
      get { return _projectToAppPoolMappingsDict.Values; }
    }

    public IEnumerable<ProjectToFailoverClusterGroupMapping> ProjectToFailoverClusterGroupMappings
    {
      get { return _projectToFailoverClusterGroupMappingsDict.Values; }
    }

    #endregion
  }
}
