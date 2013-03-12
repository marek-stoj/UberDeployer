using System;
using System.IO;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  public class DownloadArtifactsDeploymentStep : DeploymentStep
  {
    private readonly IArtifactsRepository _artifactsRepository;
    private readonly string _targetDirPath;
    
    private readonly string _artifactsFilePath;

    #region Constructor(s)

    public DownloadArtifactsDeploymentStep(IArtifactsRepository artifactsRepository, string targetDirPath)
    {
      if (artifactsRepository == null)
      {
        throw new ArgumentNullException("artifactsRepository");
      }

      if (string.IsNullOrEmpty(targetDirPath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "targetDirPath");
      }

      _artifactsRepository = artifactsRepository;
      _targetDirPath = targetDirPath;

      _artifactsFilePath = Path.Combine(_targetDirPath, "artifacts.zip");
    }

    #endregion

    #region Overrides of DeploymentStep

    protected override void DoExecute()
    {
      _artifactsRepository.GetArtifacts(
        DeploymentInfo.ProjectInfo.ArtifactsRepositoryName,
        DeploymentInfo.ProjectConfigurationName,
        DeploymentInfo.ProjectConfigurationBuildId,
        _artifactsFilePath);
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Download artifacts of project '{0} ({1}:{2})' to '{3}'.'",
            DeploymentInfo.ProjectName,
            DeploymentInfo.ProjectConfigurationName,
            DeploymentInfo.ProjectConfigurationBuildId,
            _targetDirPath);
      }
    }

    #endregion

    #region Properties

    public string ArtifactsFilePath
    {
      get { return _artifactsFilePath; }
    }

    #endregion
  }
}
