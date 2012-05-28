using System;
using System.IO;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  // TODO IMM HI: move common code up
  // TODO IMM HI: ABC... versioning
  public class DeployTerminalAppDeploymentTask : DeploymentTask
  {
    private readonly IArtifactsRepository _artifactsRepository;
    private readonly TerminalAppProjectInfo _projectInfo;

    private readonly string _projectConfigurationName;
    private readonly string _projectConfigurationBuildId;

    #region Constructor(s)

    public DeployTerminalAppDeploymentTask(IEnvironmentInfoRepository environmentInfoRepository, IArtifactsRepository artifactsRepository, TerminalAppProjectInfo projectInfo, string projectConfigurationName, string projectConfigurationBuildId, string targetEnvironmentName)
      : base(environmentInfoRepository, targetEnvironmentName)
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

      _artifactsRepository = artifactsRepository;
      _projectInfo = projectInfo;
      _projectConfigurationName = projectConfigurationName;
      _projectConfigurationBuildId = projectConfigurationBuildId;
    }

    #endregion

    #region Overrides of DeploymentTaskBase

    protected override void DoPrepare()
    {
      EnvironmentInfo environmentInfo = GetEnvironmentInfo();

      // create a step for downloading the artifacts
      var downloadArtifactsDeploymentStep =
        new DownloadArtifactsDeploymentStep(
          _artifactsRepository,
          _projectInfo,
          _projectConfigurationName,
          _projectConfigurationBuildId,
          GetTempDirPath());

      AddSubTask(downloadArtifactsDeploymentStep);

      // create a step for extracting the artifacts
      var extractArtifactsDeploymentStep =
        new ExtractArtifactsDeploymentStep(
          environmentInfo,
          _projectInfo,
          _projectConfigurationName,
          _projectConfigurationBuildId,
          downloadArtifactsDeploymentStep.ArtifactsFilePath,
          GetTempDirPath());

      AddSubTask(extractArtifactsDeploymentStep);

      // copy binaries to the target machine
      string targetDirNetworkPath =
        environmentInfo.GetTerminalServerNetworkPath(
          Path.Combine(environmentInfo.TerminalAppsBaseDirPath, _projectInfo.TerminalAppDirName));

      // create a backup step if needed
      if (Directory.Exists(targetDirNetworkPath))
      {
        AddSubTask(new BackupFilesDeploymentStep(targetDirNetworkPath));
      }

      AddSubTask(
        new CopyFilesDeploymentStep(
          extractArtifactsDeploymentStep.BinariesDirPath,
          targetDirNetworkPath));
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Deploy terminal app '{0} ({1}:{2})' to '{3}'.",
            _projectInfo.Name,
            _projectConfigurationName,
            _projectConfigurationBuildId,
            _targetEnvironmentName);
      }
    }

    #endregion

    #region Overrides of DeploymentTask

    public override string ProjectName
    {
      get { return _projectInfo.Name; }
    }

    public override string ProjectConfigurationName
    {
      get { return _projectConfigurationName; }
    }

    #endregion
  }
}
