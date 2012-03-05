using System;
using System.IO;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  public class DownloadArtifactsDeploymentStep : DeploymentStep
  {
    private readonly IArtifactsRepository _artifactsRepository;
    private readonly ProjectInfo _projectInfo;
    private readonly string _projectConfigurationName;
    private readonly string _projectConfigurationBuildId;
    private readonly string _targetDirPath;
    
    private readonly string _artifactsFilePath;

    #region Constructor(s)

    public DownloadArtifactsDeploymentStep(IArtifactsRepository artifactsRepository, ProjectInfo projectInfo, string projectConfigurationName, string projectConfigurationBuildId, string targetDirPath)
    {
      if (artifactsRepository == null)
      {
        throw new ArgumentNullException("artifactsRepository");
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

      if (string.IsNullOrEmpty(targetDirPath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "targetDirPath");
      }

      _artifactsRepository = artifactsRepository;
      _projectInfo = projectInfo;
      _projectConfigurationName = projectConfigurationName;
      _projectConfigurationBuildId = projectConfigurationBuildId;
      _targetDirPath = targetDirPath;

      _artifactsFilePath = Path.Combine(_targetDirPath, "artifacts.zip");
    }

    #endregion

    #region Overrides of DeploymentStep

    protected override void DoExecute()
    {
      _artifactsRepository.GetArtifacts(
        _projectInfo.ArtifactsRepositoryName,
        _projectConfigurationName,
        _projectConfigurationBuildId,
        _artifactsFilePath);
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Download artifacts of project '{0} ({1}:{2})' to '{3}'.'",
            _projectInfo.Name,
            _projectConfigurationName,
            _projectConfigurationBuildId,
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
