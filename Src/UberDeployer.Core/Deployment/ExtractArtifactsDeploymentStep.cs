using System;
using System.IO;
using Ionic.Zip;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  public class ExtractArtifactsDeploymentStep : DeploymentStep
  {
    private readonly EnvironmentInfo _environmentInfo;   
    private readonly string _artifactsFilePath;
    private readonly string _targetArtifactsDirPath;

    private string _archiveSubPath;

    #region Constructor(s)

    public ExtractArtifactsDeploymentStep(EnvironmentInfo environmentInfo, string artifactsFilePath, string targetArtifactsDirPath)
    {
      if (environmentInfo == null)
      {
        throw new ArgumentNullException("environmentInfo");
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
      _artifactsFilePath = artifactsFilePath;
      _targetArtifactsDirPath = targetArtifactsDirPath;
    }

    #endregion

    #region Overrides of DeploymentStep

    protected override void DoPrepare()
    {
      base.DoPrepare();

      string archiveParentPath = string.Empty;

      if (DeploymentInfo.ProjectInfo.ArtifactsAreEnvironmentSpecific)
      {
        archiveParentPath = string.Format("{0}/", _environmentInfo.ConfigurationTemplateName);
      }

      // eg. when artifacts are enviroment specific: Service/dev2/
      _archiveSubPath =
        !string.IsNullOrEmpty(DeploymentInfo.ProjectInfo.ArtifactsRepositoryDirName)
          ? string.Format("{0}{1}/", archiveParentPath, DeploymentInfo.ProjectInfo.ArtifactsRepositoryDirName)
          : archiveParentPath;
    }

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
          DeploymentInfo.ProjectInfo.ArtifactsRepositoryName,
          DeploymentInfo.ProjectConfigurationName);

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
            DeploymentInfo.ProjectName,
            DeploymentInfo.ProjectConfigurationName,
            DeploymentInfo.ProjectConfigurationBuildId,
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
