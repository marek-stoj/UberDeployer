using System;
using System.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  // TODO IMM HI: move common code up
  // TODO IMM HI: ABC... versioning
  public class DeployTerminalAppDeploymentTask : DeploymentTask
  {
    private readonly IArtifactsRepository _artifactsRepository;

    private readonly IProjectInfoRepository _projectInfoRepository;

    private TerminalAppProjectInfo _projectInfo
    {
      get { return (TerminalAppProjectInfo)DeploymentInfo.ProjectInfo; }
    }

    #region Constructor(s)

    public DeployTerminalAppDeploymentTask(
      IEnvironmentInfoRepository environmentInfoRepository,
      IArtifactsRepository artifactsRepository,
      IProjectInfoRepository projectInfoRepository)
      : base(environmentInfoRepository)
    {
      Guard.NotNull(artifactsRepository, "artifactsRepository");
      Guard.NotNull(projectInfoRepository, "projectInfoRepository");

      _artifactsRepository = artifactsRepository;
      _projectInfoRepository = projectInfoRepository;
    }

    #endregion

    #region Overrides of DeploymentTaskBase

    protected override void DoPrepare()
    {
      EnvironmentInfo environmentInfo = GetEnvironmentInfo();
      var projectInfo = GetProjectInfo<TerminalAppProjectInfo>();

      // create a step for downloading the artifacts
      var downloadArtifactsDeploymentStep =
        new DownloadArtifactsDeploymentStep(
          _artifactsRepository,
          GetTempDirPath());

      AddSubTask(downloadArtifactsDeploymentStep);

      // create a step for extracting the artifacts
      var extractArtifactsDeploymentStep =
        new ExtractArtifactsDeploymentStep(
          environmentInfo,
          downloadArtifactsDeploymentStep.ArtifactsFilePath,
          GetTempDirPath());

      AddSubTask(extractArtifactsDeploymentStep);

      if (_projectInfo.ArtifactsAreEnvironmentSpecific)
      {
        var binariesConfiguratorStep = new ConfigureBinariesStep(
          environmentInfo.ConfigurationTemplateName, GetTempDirPath());

        AddSubTask(binariesConfiguratorStep);
      }

      var extractVersionDeploymentStep = new ExtractVersionDeploymentStep(
        new Lazy<string>(() => extractArtifactsDeploymentStep.BinariesDirPath),
          projectInfo.TerminalAppExeName
        );

      AddSubTask(extractVersionDeploymentStep);

      var prepareVersionedFolderDeploymentStep = new PrepareVersionedFolderDeploymentStep(
        environmentInfo.GetTerminalServerNetworkPath(environmentInfo.TerminalAppsBaseDirPath),
        DeploymentInfo.ProjectName,
        new Lazy<string>(() => extractVersionDeploymentStep.Version));
      AddSubTask(prepareVersionedFolderDeploymentStep);

      AddSubTask(
        new CopyFilesDeploymentStep(
          new Lazy<string>(() => extractArtifactsDeploymentStep.BinariesDirPath),
          new Lazy<string>(() => prepareVersionedFolderDeploymentStep.VersionDeploymentDirPath)));

      AddSubTask(
        new UpdateApplicationShortcutDeploymentStep(
          environmentInfo.GetTerminalServerNetworkPath(environmentInfo.TerminalAppsShortcutFolder),
          new Lazy<string>(() => prepareVersionedFolderDeploymentStep.VersionDeploymentDirPath),
          projectInfo.TerminalAppExeName,
          projectInfo.Name));
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Deploy terminal app '{0} ({1}:{2})' to '{3}'.",
            _projectInfo.Name,
            DeploymentInfo.ProjectConfigurationName,
            DeploymentInfo.ProjectConfigurationBuildId,
            DeploymentInfo.TargetEnvironmentName);
      }
    }

    #endregion Overrides of DeploymentTaskBase

    protected T GetProjectInfo<T>()
      where T : ProjectInfo
    {
      ProjectInfo projectInfo = _projectInfoRepository.FindByName(DeploymentInfo.ProjectName);

      if (projectInfo == null)
      {
        throw new DeploymentTaskException(string.Format("Project named '{0}' doesn't exist.", DeploymentInfo.ProjectName));
      }
      if (!(projectInfo is T))
      {
        throw new DeploymentTaskException(string.Format("Project named '{0}' is not requsted project type", DeploymentInfo.ProjectName));
      }

      return (T)projectInfo;
    }

  }
}
