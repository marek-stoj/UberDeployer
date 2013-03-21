using System.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  public class DownloadArtifactsDeploymentStep : DeploymentStep
  {
    private readonly IArtifactsRepository _artifactsRepository;
    private readonly DeploymentInfo _deploymentInfo;
    private readonly string _targetDirPath;
    
    private readonly string _artifactsFilePath;

    #region Constructor(s)

    public DownloadArtifactsDeploymentStep(ProjectInfo projectInfo, DeploymentInfo deploymentInfo, string targetDirPath, IArtifactsRepository artifactsRepository)
      : base(projectInfo)
    {
      Guard.NotNull(deploymentInfo, "deploymentInfo");
      Guard.NotNullNorEmpty(targetDirPath, "targetDirPath");
      Guard.NotNull(artifactsRepository, "artifactsRepository");

      _deploymentInfo = deploymentInfo;
      _targetDirPath = targetDirPath;
      _artifactsRepository = artifactsRepository;

      _artifactsFilePath = Path.Combine(_targetDirPath, "artifacts.zip");
    }

    #endregion

    #region Overrides of DeploymentStep

    protected override void DoExecute()
    {
      _artifactsRepository.GetArtifacts(
        ProjectInfo.ArtifactsRepositoryName,
        _deploymentInfo.ProjectConfigurationName,
        _deploymentInfo.ProjectConfigurationBuildId,
        _artifactsFilePath);
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Download artifacts of project '{0} ({1}:{2})' to '{3}'.'",
            _deploymentInfo.ProjectName,
            _deploymentInfo.ProjectConfigurationName,
            _deploymentInfo.ProjectConfigurationBuildId,
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
