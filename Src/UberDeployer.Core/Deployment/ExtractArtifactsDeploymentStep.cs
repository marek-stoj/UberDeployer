using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  public class ExtractArtifactsDeploymentStep : DeploymentStep
  {
    private readonly EnvironmentInfo _environmentInfo;
    private readonly ProjectInfo _projectInfo;
    private readonly string _projectConfigurationName;
    private readonly string _projectConfigurationBuildId;
    private readonly string _artifactsFilePath;
    private readonly string _targetArtifactsDirPath;

    private readonly string _archiveSubPath;

    #region Constructor(s)

    public ExtractArtifactsDeploymentStep(EnvironmentInfo environmentInfo, ProjectInfo projectInfo, string projectConfigurationName, string projectConfigurationBuildId, string artifactsFilePath, string targetArtifactsDirPath)
    {
      if (environmentInfo == null)
      {
        throw new ArgumentNullException("environmentInfo");
      }

      if (projectInfo == null)
      {
        throw new ArgumentNullException("projectInfo");
      }

      if (string.IsNullOrEmpty(projectConfigurationName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "projectConfigurationName");
      }

      if (string.IsNullOrEmpty(projectConfigurationBuildId))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "projectConfigurationBuildId");
      }

      if (string.IsNullOrEmpty(artifactsFilePath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "artifactsFilePath");
      }

      if (string.IsNullOrEmpty(targetArtifactsDirPath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "targetArtifactsDirPath");
      }

      _environmentInfo = environmentInfo;
      _projectInfo = projectInfo;
      _projectConfigurationName = projectConfigurationName;
      _projectConfigurationBuildId = projectConfigurationBuildId;
      _artifactsFilePath = artifactsFilePath;
      _targetArtifactsDirPath = targetArtifactsDirPath;

      string archiveParentPath = string.Empty;

      if (_projectInfo.ArtifactsAreEnvironmentSpecific)
      {
        archiveParentPath = string.Format("{0}/", _environmentInfo.ConfigurationTemplateName);
      }

      // eg. when artifacts are enviroment specific: Service/dev2/
      _archiveSubPath =
        !string.IsNullOrEmpty(_projectInfo.ArtifactsRepositoryDirName)
          ? string.Format("{0}{1}/", archiveParentPath, _projectInfo.ArtifactsRepositoryDirName)
          : archiveParentPath;
    }

    #endregion Constructor(s)

    #region Overrides of DeploymentStep

    protected override void DoExecute()
    {
      // artifacts downloaded by previous executed html api (not REST!) are packed one more time in zip archive :/
      // move to separate step?
      using (var zipFile = new ZipFile(_artifactsFilePath))
      {
        zipFile.ExtractAll(_targetArtifactsDirPath, ExtractExistingFileAction.OverwriteSilently);
      }

      string projectArchiveFileName =
        string.Format(
          @"{0}_{1}.zip",
          _projectInfo.ArtifactsRepositoryName,
          _projectConfigurationName);

      string archivePath = Path.Combine(_targetArtifactsDirPath, projectArchiveFileName);

      // unpacking internal zip package, true is that inside are packed artifacts with name like [Project_BuildConfigName.zip]
      using (var zipFile = new ZipFile(archivePath))
      {
        zipFile.ExtractAll(_targetArtifactsDirPath, ExtractExistingFileAction.OverwriteSilently);
      }
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Extract artifacts of project '{0} ({1}:{2})' on environment '{3}' to '{4}' from '{5}'.'",
            _projectInfo.Name,
            _projectConfigurationName,
            _projectConfigurationBuildId,
            _environmentInfo.Name,
            _targetArtifactsDirPath,
            _artifactsFilePath);
      }
    }

    #endregion Overrides of DeploymentStep

    #region Properties

    public string BinariesDirPath
    {
      get
      {
        return
          Path.Combine(_targetArtifactsDirPath, _archiveSubPath.Replace("/", Path.DirectorySeparatorChar.ToString()))
            .TrimEnd(Path.DirectorySeparatorChar);
      }
    }

    #endregion Properties
  }
}