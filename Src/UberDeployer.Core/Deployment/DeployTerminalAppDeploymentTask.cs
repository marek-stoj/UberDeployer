using System;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  // TODO IMM HI: move common code up
  public class DeployTerminalAppDeploymentTask : DeploymentTask
  {
    private readonly IArtifactsRepository _artifactsRepository;
    
    #region Constructor(s)

    public DeployTerminalAppDeploymentTask(
      IProjectInfoRepository projectInfoRepository,
      IEnvironmentInfoRepository environmentInfoRepository,
      IArtifactsRepository artifactsRepository)
      : base(projectInfoRepository, environmentInfoRepository)
    {
      Guard.NotNull(artifactsRepository, "artifactsRepository");
      Guard.NotNull(projectInfoRepository, "projectInfoRepository");

      _artifactsRepository = artifactsRepository;
    }

    #endregion

    #region Overrides of DeploymentTaskBase

    protected override void DoPrepare()
    {
      EnvironmentInfo environmentInfo = GetEnvironmentInfo();
      TerminalAppProjectInfo projectInfo = GetProjectInfo<TerminalAppProjectInfo>();

      // create a step for downloading the artifacts
      var downloadArtifactsDeploymentStep =
        new DownloadArtifactsDeploymentStep(
          projectInfo,
          _artifactsRepository,
          GetTempDirPath());

      AddSubTask(downloadArtifactsDeploymentStep);

      // create a step for extracting the artifacts
      var extractArtifactsDeploymentStep =
        new ExtractArtifactsDeploymentStep(
          projectInfo,
          environmentInfo,
          downloadArtifactsDeploymentStep.ArtifactsFilePath,
          GetTempDirPath());

      AddSubTask(extractArtifactsDeploymentStep);

      if (projectInfo.ArtifactsAreEnvironmentSpecific)
      {
        var binariesConfiguratorStep =
          new ConfigureBinariesStep(
            projectInfo,
            environmentInfo.ConfigurationTemplateName,
            GetTempDirPath());

        AddSubTask(binariesConfiguratorStep);
      }

      var extractVersionDeploymentStep =
        new ExtractVersionDeploymentStep(
          projectInfo,
          new Lazy<string>(() => extractArtifactsDeploymentStep.BinariesDirPath),
          projectInfo.TerminalAppExeName
          );

      AddSubTask(extractVersionDeploymentStep);

      var prepareVersionedFolderDeploymentStep =
        new PrepareVersionedFolderDeploymentStep(
          projectInfo,
        environmentInfo.GetTerminalServerNetworkPath(environmentInfo.TerminalAppsBaseDirPath),
        DeploymentInfo.ProjectName,
        new Lazy<string>(() => extractVersionDeploymentStep.Version));

      AddSubTask(prepareVersionedFolderDeploymentStep);

      AddSubTask(
        new CopyFilesDeploymentStep(
          projectInfo,
          new Lazy<string>(() => extractArtifactsDeploymentStep.BinariesDirPath),
          new Lazy<string>(() => prepareVersionedFolderDeploymentStep.VersionDeploymentDirPath)));

      AddSubTask(
        new UpdateApplicationShortcutDeploymentStep(
          projectInfo,
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
            DeploymentInfo.ProjectName,
            DeploymentInfo.ProjectConfigurationName,
            DeploymentInfo.ProjectConfigurationBuildId,
            DeploymentInfo.TargetEnvironmentName);
      }
    }

    #endregion Overrides of DeploymentTaskBase
  }
}
