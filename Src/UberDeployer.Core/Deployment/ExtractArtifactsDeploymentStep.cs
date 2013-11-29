using System;
using System.IO;
using UberDeployer.Common.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  public class ExtractArtifactsDeploymentStep : DeploymentStep
  {
    private readonly ProjectInfo _projectInfo;
    private readonly EnvironmentInfo _environmentInfo;
    private readonly DeploymentInfo _deploymentInfo;
    private readonly string _artifactsFilePath;
    private readonly string _targetArtifactsDirPath;
    private readonly IFileAdapter _fileAdapter;
    private readonly IZipFileAdapter _zipFileAdapter;

    private string _archiveSubPath;

    #region Constructor(s)

    public ExtractArtifactsDeploymentStep(ProjectInfo projectInfo, EnvironmentInfo environmentInfo, DeploymentInfo deploymentInfo, string artifactsFilePath, string targetArtifactsDirPath, IFileAdapter fileAdapter, IZipFileAdapter zipFileAdapter)
    {
      Guard.NotNull(projectInfo, "projectInfo");
      Guard.NotNull(environmentInfo, "environmentInfo");
      Guard.NotNull(deploymentInfo, "deploymentInfo");
      Guard.NotNullNorEmpty(artifactsFilePath, "artifactsFilePath");
      Guard.NotNullNorEmpty(targetArtifactsDirPath, "targetArtifactsDirPath");
      Guard.NotNull(fileAdapter, "fileAdapter");
      Guard.NotNull(zipFileAdapter, "zipFileAdapter");

      _projectInfo = projectInfo;
      _environmentInfo = environmentInfo;
      _deploymentInfo = deploymentInfo;
      _artifactsFilePath = artifactsFilePath;
      _targetArtifactsDirPath = targetArtifactsDirPath;
      _fileAdapter = fileAdapter;
      _zipFileAdapter = zipFileAdapter;
    }

    #endregion

    #region Overrides of DeploymentStep

    protected override void DoPrepare()
    {
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

    protected override void DoExecute()
    {
      // artifacts downloaded by previous executed html api (not REST!) are packed one more time in zip archive :/ move to separate step?
      if (!_fileAdapter.Exists(_artifactsFilePath))
      {
        throw new InvalidOperationException(string.Format("Couldn't extract ZIP archive because it doesn't exist: '{0}'.", _artifactsFilePath));
      }

      _zipFileAdapter.ExtractAll(_artifactsFilePath, _targetArtifactsDirPath, true);

      string projectArchiveFileName =
        string.Format(
          @"{0}_{1}.zip",
          _projectInfo.ArtifactsRepositoryName,
          _deploymentInfo.ProjectConfigurationName);

      string archivePath = Path.Combine(_targetArtifactsDirPath, projectArchiveFileName);

      // unpacking internal zip package, true is that inside are packed artifacts with name like [Project_BuildConfigName.zip]
      if (!_fileAdapter.Exists(archivePath))
      {
        throw new InvalidOperationException(string.Format("Couldn't extract internal ZIP archive because it doesn't exist: '{0}'.", archivePath));
      }

      _zipFileAdapter.ExtractAll(archivePath, _targetArtifactsDirPath, true);
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Extract artifacts of project '{0} ({1}:{2})' on environment '{3}' to '{4}' from '{5}'.'",
            _deploymentInfo.ProjectName,
            _deploymentInfo.ProjectConfigurationName,
            _deploymentInfo.ProjectConfigurationBuildId,
            _environmentInfo.Name,
            _targetArtifactsDirPath,
            _artifactsFilePath);
      }
    }

    #endregion

    #region Properties
    
    public string BinariesDirPath
    {
      get
      {
        if (!IsPrepared)
        {
          throw new InvalidOperationException("Step has not been prepared yet.");
        }

        return
          Path.Combine(_targetArtifactsDirPath, _archiveSubPath.Replace("/", Path.DirectorySeparatorChar.ToString()))
            .TrimEnd(Path.DirectorySeparatorChar);
      }
    }    

    #endregion
  }
}
