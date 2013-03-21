using System.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  public class DownloadArtifactsDeploymentStep : DeploymentStep
  {
    private readonly IArtifactsRepository _artifactsRepository;
    private readonly string _targetDirPath;
    
    private readonly string _artifactsFilePath;

    #region Constructor(s)

    public DownloadArtifactsDeploymentStep(ProjectInfo projectInfo, IArtifactsRepository artifactsRepository, string targetDirPath)
      : base(projectInfo)
    {
      Guard.NotNull(artifactsRepository, "artifactsRepository");
      Guard.NotNullNorEmpty(targetDirPath, "targetDirPath");

      _artifactsRepository = artifactsRepository;
      _targetDirPath = targetDirPath;

      _artifactsFilePath = Path.Combine(_targetDirPath, "artifacts.zip");
    }

    #endregion

    #region Overrides of DeploymentStep

    protected override void DoExecute()
    {
      _artifactsRepository.GetArtifacts(
        ProjectInfo.ArtifactsRepositoryName,
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
