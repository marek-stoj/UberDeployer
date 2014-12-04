using System;
using UberDeployer.Common.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  // TODO IMM HI: move common code up
  public class DeployTerminalAppDeploymentTask : DeploymentTask
  {
    private readonly IArtifactsRepository _artifactsRepository;
    private readonly IDirectoryAdapter _directoryAdapter;
    private readonly IFileAdapter _fileAdapter;
    private readonly IZipFileAdapter _zipFileAdapter;

    #region Constructor(s)

    public DeployTerminalAppDeploymentTask(
      IProjectInfoRepository projectInfoRepository,
      IEnvironmentInfoRepository environmentInfoRepository,
      IArtifactsRepository artifactsRepository,
      IDirectoryAdapter directoryAdapter,
      IFileAdapter fileAdapter,
      IZipFileAdapter zipFileAdapter)
      : base(projectInfoRepository, environmentInfoRepository)
    {
      Guard.NotNull(artifactsRepository, "artifactsRepository");
      Guard.NotNull(projectInfoRepository, "projectInfoRepository");
      Guard.NotNull(directoryAdapter, "directoryAdapter");
      Guard.NotNull(fileAdapter, "fileAdapter");
      Guard.NotNull(zipFileAdapter, "zipFileAdapter");

      _artifactsRepository = artifactsRepository;
      _directoryAdapter = directoryAdapter;
      _fileAdapter = fileAdapter;
      _zipFileAdapter = zipFileAdapter;
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
          DeploymentInfo,
          GetTempDirPath(),
          _artifactsRepository);

      AddSubTask(downloadArtifactsDeploymentStep);

      // create a step for extracting the artifacts
      var extractArtifactsDeploymentStep =
        new ExtractArtifactsDeploymentStep(
          projectInfo,
          environmentInfo,
          DeploymentInfo,
          downloadArtifactsDeploymentStep.ArtifactsFilePath,
          GetTempDirPath(),
          _fileAdapter,
          _zipFileAdapter);

      AddSubTask(extractArtifactsDeploymentStep);

      if (projectInfo.ArtifactsAreEnvironmentSpecific)
      {
        var binariesConfiguratorStep =
          new ConfigureBinariesStep(
            environmentInfo.ConfigurationTemplateName,
            GetTempDirPath());

        AddSubTask(binariesConfiguratorStep);
      }

      var extractVersionDeploymentStep =
        new ExtractVersionDeploymentStep(
          new Lazy<string>(() => extractArtifactsDeploymentStep.BinariesDirPath),
          projectInfo.TerminalAppExeName
          );

      AddSubTask(extractVersionDeploymentStep);

      var prepareVersionedFolderDeploymentStep =
        new PrepareVersionedFolderDeploymentStep(
          DeploymentInfo.ProjectName,
          environmentInfo.GetTerminalServerNetworkPath(environmentInfo.TerminalAppsBaseDirPath),
          projectInfo.TerminalAppDirName,
          new Lazy<string>(() => extractVersionDeploymentStep.Version));

      AddSubTask(prepareVersionedFolderDeploymentStep);

      AddSubTask(
        new CleanDirectoryDeploymentStep(
          _directoryAdapter,
          _fileAdapter,
          new Lazy<string>(() => prepareVersionedFolderDeploymentStep.VersionDeploymentDirPath),
          excludedDirs: new string[] { }));

      AddSubTask(
        new CopyFilesDeploymentStep(
          _directoryAdapter,
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
            DeploymentInfo.ProjectName,
            DeploymentInfo.ProjectConfigurationName,
            DeploymentInfo.ProjectConfigurationBuildId,
            DeploymentInfo.TargetEnvironmentName);
      }
    }

    #endregion Overrides of DeploymentTaskBase
  }
}
