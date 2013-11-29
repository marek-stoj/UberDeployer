using System;
using System.IO;
using UberDeployer.Common.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  public class CreateManualDeploymentPackageDeploymentTask : DeploymentTask
  {
    private readonly IArtifactsRepository _artifactsRepository;
    private readonly IDirectoryAdapter _directoryAdapter;
    private readonly IFileAdapter _fileAdapter;
    private readonly IZipFileAdapter _zipFileAdapter;

    public string PackageDirPath { get; private set; }

    public CreateManualDeploymentPackageDeploymentTask(
      IProjectInfoRepository projectInfoRepository,
      IEnvironmentInfoRepository environmentInfoRepository,
      IArtifactsRepository artifactsRepository,
      IDirectoryAdapter directoryAdapter,
      IFileAdapter fileAdapter,
      IZipFileAdapter zipFileAdapter,
      string packageDirPath)
      : base(projectInfoRepository, environmentInfoRepository)
    {
      Guard.NotNull(artifactsRepository, "artifactsRepository");
      Guard.NotNullNorEmpty(packageDirPath, "packageDirPath");
      Guard.NotNull(directoryAdapter, "directoryAdapter");
      Guard.NotNull(fileAdapter, "fileAdapter");
      Guard.NotNull(zipFileAdapter, "zipFileAdapter");

      _artifactsRepository = artifactsRepository;
      _directoryAdapter = directoryAdapter;
      _fileAdapter = fileAdapter;
      _zipFileAdapter = zipFileAdapter;
      PackageDirPath = packageDirPath;
    }

    protected override void DoPrepare()
    {
      EnvironmentInfo environmentInfo = GetEnvironmentInfo();

      var projectInfo = GetProjectInfo<ProjectInfo>();

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

      AddSubTask(
        new CopyFilesDeploymentStep(
          _directoryAdapter,
          _fileAdapter,
          new Lazy<string>(() => extractArtifactsDeploymentStep.BinariesDirPath),
          new Lazy<string>(() => PreparePackageDirPath(PackageDirPath))));
    }

    public override string Description
    {
      get
      {
        return string.Format(
            "Copy artifacts '{0} ({1}:{2})' to '{3}'.",
            DeploymentInfo.ProjectName,
            DeploymentInfo.ProjectConfigurationName,
            DeploymentInfo.ProjectConfigurationBuildId,
            DeploymentInfo.TargetEnvironmentName);
      }
    }

    private static string PreparePackageDirPath(string packageDirPath)
    {
      if (Directory.Exists(packageDirPath))
      {
        throw new DeploymentTaskException(string.Format("Target directory already exists: '{0}'", packageDirPath));
      }

      Directory.CreateDirectory(packageDirPath);

      return packageDirPath;
    }    
  }
}
