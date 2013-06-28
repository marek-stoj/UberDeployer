using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Common;

namespace UberDeployer.Core.Domain
{
  public class EnvironmentInfo
  {
    private static readonly Regex _DriveLetterRegex = new Regex(@"^(?<DriveLetter>[a-z]):\\", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly List<String> _webServerMachines;
    private readonly Dictionary<string, EnvironmentUser> _environmentUsersDict;
    private readonly Dictionary<string, IisAppPoolInfo> _appPoolInfosDict;
    private readonly Dictionary<string, DatabaseServer> _databaseServersDict;
    private readonly Dictionary<string, WebAppProjectConfiguration> _webAppProjectConfigurationsDict;
    private readonly Dictionary<string, ProjectToFailoverClusterGroupMapping> _projectToFailoverClusterGroupMappingsDict;
    private readonly Dictionary<string, DbProjectConfiguration> _dbProjectConfigurationsDict;

    #region Constructor(s)

    public EnvironmentInfo(
      string name,
      string configurationTemplateName,
      string appServerMachineName,
      string failoverClusterMachineName,
      IEnumerable<string> webServerMachineNames,
      string terminalServerMachineName,
      string schedulerServerMachineName,
      string ntServicesBaseDirPath,
      string webAppsBaseDirPath,
      string schedulerAppsBaseDirPath,
      string terminalAppsBaseDirPath,
      bool enableFailoverClusteringForNtServices,
      IEnumerable<EnvironmentUser> environmentUsers,
      IEnumerable<IisAppPoolInfo> appPoolInfos,
      IEnumerable<DatabaseServer> databaseServers,
      IEnumerable<WebAppProjectConfiguration> webAppProjectConfigurations,
      IEnumerable<ProjectToFailoverClusterGroupMapping> projectToFailoverClusterGroupMappings,
      IEnumerable<DbProjectConfiguration> dbProjectConfigurations,
      string terminalAppShortcutFolder
      )
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
      Guard.NotNullNorEmpty(schedulerServerMachineName, "schedulerServerMachineName");
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

      if (webAppProjectConfigurations == null)
      {
        throw new ArgumentNullException("webAppProjectConfigurations");
      }

      if (projectToFailoverClusterGroupMappings == null)
      {
        throw new ArgumentNullException("projectToFailoverClusterGroupMappings");
      }

      if (dbProjectConfigurations == null)
      {
        throw new ArgumentNullException("dbProjectConfigurations");
      }

      Guard.NotNullNorEmpty(name, "terminalAppShortcutPath");

      Name = name;
      ConfigurationTemplateName = configurationTemplateName;
      AppServerMachineName = appServerMachineName;
      FailoverClusterMachineName = failoverClusterMachineName;
      _webServerMachines = new List<string>(webServerMachineNames);
      TerminalServerMachineName = terminalServerMachineName;
      SchedulerServerMachineName = schedulerServerMachineName;
      NtServicesBaseDirPath = ntServicesBaseDirPath;
      WebAppsBaseDirPath = webAppsBaseDirPath;
      SchedulerAppsBaseDirPath = schedulerAppsBaseDirPath;
      TerminalAppsBaseDirPath = terminalAppsBaseDirPath;
      EnableFailoverClusteringForNtServices = enableFailoverClusteringForNtServices;

      _environmentUsersDict = environmentUsers.ToDictionary(e => e.Id);
      _appPoolInfosDict = appPoolInfos.ToDictionary(e => e.Name);
      _databaseServersDict = databaseServers.ToDictionary(e => e.Id);

      _webAppProjectConfigurationsDict = webAppProjectConfigurations.ToDictionary(e => e.ProjectName);
      _projectToFailoverClusterGroupMappingsDict = projectToFailoverClusterGroupMappings.ToDictionary(e => e.ProjectName);
      _dbProjectConfigurationsDict = dbProjectConfigurations.ToDictionary(e => e.ProjectName);

      TerminalAppsShortcutFolder = terminalAppShortcutFolder;
    }

    #endregion

    #region Public methods

    public static string GetNetworkPath(string machineName, string absolutePath)
    {
      Guard.NotNullNorEmpty(machineName, "machineName");
      Guard.NotNullNorEmpty(absolutePath, "absolutePath");

      absolutePath = absolutePath.ExpandVariables(CreatePathVariables(machineName));

      if (absolutePath.StartsWith(@"\\"))
      {
        // the path is already a network path - just return it
        return absolutePath;
      }

      Match driveLetterMatch = _DriveLetterRegex.Match(absolutePath);

      if (!driveLetterMatch.Success)
      {
        throw new ArgumentException(string.Format("The path is not an absolute local path, ie. it doesn't start with a drive letter followed by a colon and a backslash. Absolute local path: '{0}'.", absolutePath), "absolutePath");
      }

      string driveLetter = driveLetterMatch.Groups["DriveLetter"].Value;

      return
        string.Format(
          "\\\\{0}\\{1}$\\{2}",
          machineName,
          driveLetter,
          absolutePath.Substring(driveLetterMatch.Length));
    }

    public string GetAppServerNetworkPath(string absolutePath)
    {
      Guard.NotNullNorEmpty(absolutePath, "absolutePath");

      return GetNetworkPath(AppServerMachineName, absolutePath);
    }

    public string GetSchedulerServerNetworkPath(string absolutePath)
    {
      Guard.NotNullNorEmpty(absolutePath, "absolutePath");

      return GetNetworkPath(SchedulerServerMachineName, absolutePath);
    }

    public string GetWebServerNetworkPath(string webServerMachineName, string absolutePath)
    {
      Guard.NotNullNorEmpty(webServerMachineName, "webServerMachineName");
      Guard.NotNullNorEmpty(absolutePath, "absolutePath");

      return GetNetworkPath(webServerMachineName, absolutePath);
    }

    public string GetTerminalServerNetworkPath(string absolutePath)
    {
      Guard.NotNullNorEmpty(absolutePath, "absolutePath");

      return GetNetworkPath(TerminalServerMachineName, absolutePath);
    }

    public string GetNtServicesBaseDirPath(string machineName)
    {
      Guard.NotNullNorEmpty(machineName, "machineName");

      Dictionary<string, string> pathVariables =
        CreatePathVariables(machineName);

      return NtServicesBaseDirPath.ExpandVariables(pathVariables);
    }

    public string GetWebAppsBaseDirPath(string machineName)
    {
      Guard.NotNullNorEmpty(machineName, "machineName");

      Dictionary<string, string> pathVariables =
        CreatePathVariables(machineName);

      return WebAppsBaseDirPath.ExpandVariables(pathVariables);
    }

    public string GetSchedulerAppsBaseDirPath(string machineName)
    {
      Guard.NotNullNorEmpty(machineName, "machineName");

      Dictionary<string, string> pathVariables =
        CreatePathVariables(machineName);

      return SchedulerAppsBaseDirPath.ExpandVariables(pathVariables);
    }

    public string GetTerminalAppsBaseDirPath(string machineName)
    {
      Guard.NotNullNorEmpty(machineName, "machineName");

      Dictionary<string, string> pathVariables =
        CreatePathVariables(machineName);

      return TerminalAppsBaseDirPath.ExpandVariables(pathVariables);
    }

    public string GetTerminalAppsShortcutFolder(string machineName)
    {
      Guard.NotNullNorEmpty(machineName, "machineName");

      Dictionary<string, string> pathVariables =
        CreatePathVariables(machineName);

      return TerminalAppsShortcutFolder.ExpandVariables(pathVariables);
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

    public WebAppProjectConfiguration GetWebAppProjectConfiguration(string projectName)
    {
      Guard.NotNullNorEmpty(projectName, "projectName");

      WebAppProjectConfiguration configuration;

      if (_webAppProjectConfigurationsDict.TryGetValue(projectName, out configuration))
      {
        return configuration;
      }

      throw new ArgumentException(string.Format("Web app project named '{0}' has no configuration for environment '{1}'.", projectName, Name));
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

    public DbProjectConfiguration GetDbProjectConfiguration(string projectName)
    {
      Guard.NotNullNorEmpty(projectName, "projectName");

      DbProjectConfiguration configuration;

      if (_dbProjectConfigurationsDict.TryGetValue(projectName, out configuration))
      {
        return configuration;
      }

      throw new ArgumentException(string.Format("Db project named '{0}' has no configuration for environment '{1}'.", projectName, Name));
    }

    public IisAppPoolInfo GetAppPoolInfo(string appPoolId)
    {
      Guard.NotNullNorEmpty(appPoolId, "appPoolId");

      IisAppPoolInfo iisAppPoolInfo;

      if (_appPoolInfosDict.TryGetValue(appPoolId, out iisAppPoolInfo))
      {
        return iisAppPoolInfo;
      }

      throw new ArgumentException(string.Format("App pool with id '{0}' is not defined for environment '{1}'.", appPoolId, Name));
    }

    public DatabaseServer GetDatabaseServer(string databaseServerId)
    {
      Guard.NotNullNorEmpty(databaseServerId, "databaseServerId");

      DatabaseServer databaseServer;

      if (_databaseServersDict.TryGetValue(databaseServerId, out databaseServer))
      {
        return databaseServer;
      }

      throw new ArgumentException(string.Format("Database server with id '{0}' is not defined for environment '{1}'.", databaseServerId, Name));
    }

    #endregion

    #region Private methods

    private static Dictionary<string, string> CreatePathVariables(string machineName)
    {
      Guard.NotNullNorEmpty(machineName, "machineName");

      return
        new Dictionary<string, string>
        {
          { "MachineName", machineName },
        };
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

    public string SchedulerServerMachineName { get; private set; }

    public string NtServicesBaseDirPath { get; set; }

    public string WebAppsBaseDirPath { get; set; }

    public string SchedulerAppsBaseDirPath { get; set; }

    public string TerminalAppsBaseDirPath { get; set; }

    public string TerminalAppsShortcutFolder { get; set; }

    public bool EnableFailoverClusteringForNtServices { get; private set; }

    public IEnumerable<EnvironmentUser> EnvironmentUsers
    {
      get { return _environmentUsersDict.Values; }
    }

    public IEnumerable<IisAppPoolInfo> AppPoolInfos
    {
      get { return _appPoolInfosDict.Values; }
    }

    public IEnumerable<DatabaseServer> DatabaseServers
    {
      get { return _databaseServersDict.Values; }
    }

    public IEnumerable<WebAppProjectConfiguration> WebAppProjectConfigurations
    {
      get { return _webAppProjectConfigurationsDict.Values; }
    }

    public IEnumerable<ProjectToFailoverClusterGroupMapping> ProjectToFailoverClusterGroupMappings
    {
      get { return _projectToFailoverClusterGroupMappingsDict.Values; }
    }

    public IEnumerable<DbProjectConfiguration> DbProjectConfigurations
    {
      get { return _dbProjectConfigurationsDict.Values; }
    }

    #endregion
  }
}
