using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.DbDiff;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Db;
using log4net;
using UberDeployer.Common;
using System.Linq;

namespace UberDeployer.Core.Management.Metadata
{
  public class ProjectMetadataExplorer : IProjectMetadataExplorer
  {
    private static readonly Regex _MachineNameInNetworkPathRegex = new Regex("^\\\\\\\\(?<MachineName>[^\\\\]+)\\\\", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private readonly IObjectFactory _objectFactory;
    private readonly IProjectInfoRepository _projectInfoRepository;
    private readonly IEnvironmentInfoRepository _environmentInfoRepository;
    private readonly IDbVersionProvider _dbVersionProvider;

    public ProjectMetadataExplorer(IObjectFactory objectFactory, IProjectInfoRepository projectInfoRepository, IEnvironmentInfoRepository environmentInfoRepository, IDbVersionProvider dbVersionProvider)
    {
      Guard.NotNull(objectFactory, "objectFactory");
      Guard.NotNull(projectInfoRepository, "projectInfoRepository");
      Guard.NotNull(environmentInfoRepository, "environmentInfoRepository");
      Guard.NotNull(dbVersionProvider, "dbVersionProvider");

      _objectFactory = objectFactory;
      _projectInfoRepository = projectInfoRepository;
      _environmentInfoRepository = environmentInfoRepository;
      _dbVersionProvider = dbVersionProvider;
    }

    public ProjectMetadata GetProjectMetadata(string projectName, string environmentName)
    {
      Guard.NotNullNorEmpty(projectName, "projectName");
      Guard.NotNullNorEmpty(environmentName, "environmentName");

      _log.DebugIfEnabled(() => string.Format("Getting project metadata. Project name: '{0}'. Environment name: '{1}'.", projectName, environmentName));

      ProjectInfo projectInfo =
        _projectInfoRepository.FindByName(projectName);

      if (projectInfo == null)
      {
        throw new ArgumentException(string.Format("Project named '{0}' doesn't exist.", projectName));
      }

      EnvironmentInfo environmentInfo =
        _environmentInfoRepository.FindByName(environmentName);

      if (environmentInfo == null)
      {
        throw new ArgumentException(string.Format("Environment named '{0}' doesn't exist.", environmentName));
      }

      if (projectInfo is DbProjectInfo)
      {
        return GetDbProjectMetadata((DbProjectInfo)projectInfo, environmentInfo);
      }
      else
      {
        return GetOrdinaryProjectMetadata(projectInfo, environmentInfo);
      }
    }

    private ProjectMetadata GetOrdinaryProjectMetadata(ProjectInfo projectInfo, EnvironmentInfo environmentInfo)
    {
      var projectVersions = new List<MachineSpecificProjectVersion>();

      IEnumerable<string> targetFolders =
        projectInfo.GetTargetFolders(_objectFactory, environmentInfo);

      foreach (string targetFolder in targetFolders)
      {
        _log.DebugIfEnabled(() => string.Format("Processing target folder: '{0}'.", targetFolder));

        string machineName;

        if (!TryExtractMachineName(targetFolder, out machineName))
        {
          machineName = "?";
        }

        string projectVersion;

        string assemblyFilePath =
          Path.Combine(targetFolder, projectInfo.GetMainAssemblyFileName());

        // TODO IMM HI: temporary solution for getting version of web projects with main assemblies of form XXX.Web.dll or XXX.WebApp.dll.
        if (projectInfo is WebAppProjectInfo)
        {
          string originalAssemblyFilePath = assemblyFilePath;

          if (!File.Exists(assemblyFilePath))
          {
            assemblyFilePath = AddSuffixToFileName(originalAssemblyFilePath, ".Web");
          }

          if (!File.Exists(assemblyFilePath))
          {
            assemblyFilePath = AddSuffixToFileName(originalAssemblyFilePath, ".WebApp");
          }
        }

        if (File.Exists(assemblyFilePath))
        {
          FileVersionInfo fileVersionInfo =
            FileVersionInfo.GetVersionInfo(assemblyFilePath);

          projectVersion = fileVersionInfo.FileVersion;
        }
        else
        {
          projectVersion = "?";
        }

        projectVersions.Add(
          new MachineSpecificProjectVersion(
            machineName,
            projectVersion));
      }

      return new ProjectMetadata(projectInfo.Name, environmentInfo.Name, projectVersions);
    }

    private ProjectMetadata GetDbProjectMetadata(DbProjectInfo dbProjectInfo, EnvironmentInfo environmentInfo)
    {
      var projectVersions = new List<MachineSpecificProjectVersion>();

      DbProjectConfiguration dbProjectConfiguration =
        environmentInfo.GetDbProjectConfiguration(dbProjectInfo.Name);

      DatabaseServer databaseServer =
        environmentInfo.GetDatabaseServer(dbProjectConfiguration.DatabaseServerId);

      IEnumerable<string> dbVersions =
        _dbVersionProvider.GetVersions(
          dbProjectInfo.DbName,
          databaseServer.MachineName);

      DbVersion latestDbVersion =
        dbVersions
          .Select(DbVersion.FromString)
          .OrderByDescending(v => v)
          .FirstOrDefault();

      if (latestDbVersion != null)
      {
        projectVersions.Add(new MachineSpecificProjectVersion(databaseServer.MachineName, latestDbVersion.ToString()));
      }

      return new ProjectMetadata(dbProjectInfo.Name, environmentInfo.Name, projectVersions);
    }

    private static bool TryExtractMachineName(string targetFolder, out string machineName)
    {
      Guard.NotNullNorEmpty(targetFolder, "targetFolder");

      Match match = _MachineNameInNetworkPathRegex.Match(targetFolder);

      if (match.Success)
      {
        machineName = match.Groups["MachineName"].Value;

        return true;
      }

      machineName = null;

      return false;
    }

    private static string AddSuffixToFileName(string assemblyFilePath, string suffix)
    {
      string dirPath = Path.GetDirectoryName(assemblyFilePath) ?? "";

      return
        Path.Combine(
          dirPath,
          string.Format(
            "{0}{1}{2}",
            Path.GetFileNameWithoutExtension(assemblyFilePath),
            suffix,
            Path.GetExtension(assemblyFilePath)));
    }
  }
}
