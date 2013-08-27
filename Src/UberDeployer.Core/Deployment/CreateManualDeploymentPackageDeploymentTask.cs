using System;
using System.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  public class CreateManualDeploymentPackageDeploymentTask : DeploymentTask
  {
    private readonly IArtifactsRepository _artifactsRepository;
    
    private readonly IDirPathParamsResolver _dirPathParamsResolver;

    public string PackageDirPath { get; private set; }

    public CreateManualDeploymentPackageDeploymentTask(
      IProjectInfoRepository projectInfoRepository,
      IEnvironmentInfoRepository environmentInfoRepository,
      IArtifactsRepository artifactsRepository,
      IDirPathParamsResolver dirPathParamsResolver,
      string packageDirPath)
      : base(projectInfoRepository, environmentInfoRepository)
    {
      Guard.NotNull(artifactsRepository, "artifactsRepository");
      Guard.NotNull(dirPathParamsResolver, "dirPathParamsResolver");
      Guard.NotNullNorEmpty(packageDirPath, "packageDirPath");

      _artifactsRepository = artifactsRepository;
      _dirPathParamsResolver = dirPathParamsResolver;
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
          GetTempDirPath());

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

    private string PreparePackageDirPath(string packageDirPath)
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
