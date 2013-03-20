using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;
using log4net;
using UberDeployer.Common;

namespace UberDeployer.Core.Management.Metadata
{
  public class ProjectMetadataExplorer : IProjectMetadataExplorer
  {
    private static readonly Regex _MachineNameInNetworkPathRegex = new Regex("^\\\\\\\\(?<MachineName>[^\\\\]+)\\\\", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private readonly IObjectFactory _objectFactory;
    private readonly IProjectInfoRepository _projectInfoRepository;
    private readonly IEnvironmentInfoRepository _environmentInfoRepository;

    public ProjectMetadataExplorer(IObjectFactory objectFactory, IProjectInfoRepository projectInfoRepository, IEnvironmentInfoRepository environmentInfoRepository)
    {
      Guard.NotNull(objectFactory, "objectFactory");
      Guard.NotNull(projectInfoRepository, "projectInfoRepository");
      Guard.NotNull(environmentInfoRepository, "environmentInfoRepository");

      _objectFactory = objectFactory;
      _projectInfoRepository = projectInfoRepository;
      _environmentInfoRepository = environmentInfoRepository;
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

      return new ProjectMetadata(projectName, environmentName, projectVersions);
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
  }
}
