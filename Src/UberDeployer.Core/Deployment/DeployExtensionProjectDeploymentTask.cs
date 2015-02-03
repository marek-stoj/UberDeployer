using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UberDeployer.Common.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.FailoverCluster;
using UberDeployer.Core.Management.NtServices;

namespace UberDeployer.Core.Deployment
{
  public class DeployExtensionProjectDeploymentTask : DeploymentTask
  {
    private readonly IArtifactsRepository _artifactsRepository;
    private readonly IDirectoryAdapter _directoryAdapter;
    private readonly IFileAdapter _fileAdapter;
    private readonly IZipFileAdapter _zipFileAdapter;
    private readonly IFailoverClusterManager _failoverClusterManager;
    private readonly INtServiceManager _ntServiceManager;
    
    private ExtensionProjectInfo _projectInfo;
    private EnvironmentInfo _environmentInfo;
    private NtServiceProjectInfo _extendedProjectInfo;

    public DeployExtensionProjectDeploymentTask(
      IProjectInfoRepository projectInfoRepository, 
      IEnvironmentInfoRepository environmentInfoRepository, 
      IArtifactsRepository artifactsRepository, 
      IDirectoryAdapter directoryAdapter, 
      IFileAdapter fileAdapter, 
      IZipFileAdapter zipFileAdapter, 
      IFailoverClusterManager failoverClusterManager, 
      INtServiceManager ntServiceManager) : 
      base(projectInfoRepository, environmentInfoRepository)
    {
      Guard.NotNull(projectInfoRepository, "projectInfoRepository");
      Guard.NotNull(environmentInfoRepository, "environmentInfoRepository");
      Guard.NotNull(artifactsRepository, "artifactsRepository");
      Guard.NotNull(directoryAdapter, "directoryAdapter");
      Guard.NotNull(fileAdapter, "fileAdapter");
      Guard.NotNull(zipFileAdapter, "zipFileAdapter");
      Guard.NotNull(failoverClusterManager, "failoverClusterManager");
      Guard.NotNull(ntServiceManager, "ntServiceManager");

      _artifactsRepository = artifactsRepository;
      _directoryAdapter = directoryAdapter;
      _fileAdapter = fileAdapter;
      _zipFileAdapter = zipFileAdapter;
      _failoverClusterManager = failoverClusterManager;
      _ntServiceManager = ntServiceManager;
    }

    protected override void DoPrepare()
    {
      _environmentInfo = GetEnvironmentInfo();
      _projectInfo = GetProjectInfo<ExtensionProjectInfo>();
      _extendedProjectInfo = GetProjectInfo<NtServiceProjectInfo>(_projectInfo.ExtendedProjectName);

      // create a step for downloading the artifacts
      var downloadArtifactsDeploymentStep =
        new DownloadArtifactsDeploymentStep(
          _projectInfo,
          DeploymentInfo,
          GetTempDirPath(),
          _artifactsRepository);

      AddSubTask(downloadArtifactsDeploymentStep);

      // create a step for extracting the artifacts
      var extractArtifactsDeploymentStep =
        new ExtractArtifactsDeploymentStep(
          _projectInfo,
          _environmentInfo,
          DeploymentInfo,
          downloadArtifactsDeploymentStep.ArtifactsFilePath,
          GetTempDirPath(),
          _fileAdapter,
          _zipFileAdapter);

      AddSubTask(extractArtifactsDeploymentStep);

      bool deployToClusteredEnvironment =
        _environmentInfo.EnableFailoverClusteringForNtServices;

      if (deployToClusteredEnvironment)
      {
        PostDiagnosticMessage("Will deploy to a clustered environment.", DiagnosticMessageType.Trace);

        if (string.IsNullOrEmpty(_environmentInfo.GetFailoverClusterGroupNameForProject(_extendedProjectInfo.Name)))
        {
          throw new InvalidOperationException(string.Format("Failover clustering for NT services is enabled for environment '{0}' but there is no cluster group mapping for project '{1}'.", _environmentInfo.Name, _extendedProjectInfo.Name));
        }

        DoPrepareDeploymentToClusteredEnvironment(
          new Lazy<string>(() => extractArtifactsDeploymentStep.BinariesDirPath),
          _extendedProjectInfo);
      }
      else
      {
        PostDiagnosticMessage("Will deploy to a non-clustered environment.", DiagnosticMessageType.Trace);

        AddSubTask(
          new StopNtServiceDeploymentStep(
            _ntServiceManager,
            _environmentInfo.AppServerMachineName,
            _extendedProjectInfo.NtServiceName));

        DoPrepareCommonDeploymentSteps(_environmentInfo.GetAppServerNetworkPath, new Lazy<string>(() => extractArtifactsDeploymentStep.BinariesDirPath) );

        AddSubTask(
          new StartNtServiceDeploymentStep(
            _ntServiceManager,
            _environmentInfo.AppServerMachineName,
            _extendedProjectInfo.NtServiceName));
      }
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Deploy extension '{0} ({1}:{2})' to '{3}'.",
            DeploymentInfo.ProjectName,
            DeploymentInfo.ProjectConfigurationName,
            DeploymentInfo.ProjectConfigurationBuildId,
            DeploymentInfo.TargetEnvironmentName);
      }
    }

    private void DoPrepareDeploymentToClusteredEnvironment(Lazy<string> artifactsBinariesDirPathProvider, NtServiceProjectInfo extendedServiceProjectInfo)
    {
      string clusterGroupName = _environmentInfo.GetFailoverClusterGroupNameForProject(extendedServiceProjectInfo.Name);

      if (string.IsNullOrEmpty(clusterGroupName))
      {
        throw new InvalidOperationException(string.Format("There is no cluster group defined for project '{0}' in environment '{1}'.", extendedServiceProjectInfo.Name, _environmentInfo.Name));
      }

      string failoverClusterMachineName = _environmentInfo.FailoverClusterMachineName;

      if (string.IsNullOrEmpty(failoverClusterMachineName))
      {
        throw new InvalidOperationException(string.Format("Environment '{0}' has no failover cluster machine name defined.", _environmentInfo.Name));
      }

      string currentNodeName =
        _failoverClusterManager.GetCurrentNodeName(failoverClusterMachineName, clusterGroupName);

      if (string.IsNullOrEmpty(currentNodeName))
      {
        throw new InvalidOperationException(string.Format("Cluster group '{0}' has no current node in a cluster '{1}' in environment '{2}'.", clusterGroupName, _environmentInfo.FailoverClusterMachineName, _environmentInfo.Name));
      }

      PostDiagnosticMessage(string.Format("Current node: '{0}'.", currentNodeName), DiagnosticMessageType.Trace);

      List<string> possibleNodeNames =
        _failoverClusterManager.GetPossibleNodeNames(failoverClusterMachineName, clusterGroupName)
          .ToList();

      PostDiagnosticMessage(string.Format("Possible nodes: {0}.", string.Join(", ", possibleNodeNames.Select(n => string.Format("'{0}'", n)))), DiagnosticMessageType.Trace);

      if (possibleNodeNames.Count < 2)
      {
        throw new InvalidOperationException(string.Format("There is only one possible node for cluster group '{0}' in a cluster '{1}' in environment '{2}'.", clusterGroupName, _environmentInfo.FailoverClusterMachineName, _environmentInfo.Name));
      }

      foreach (string possibleNodeName in possibleNodeNames)
      {
        string machineName = possibleNodeName;

        if (string.Equals(machineName, currentNodeName, StringComparison.OrdinalIgnoreCase))
        {
          continue;
        }

        DoPrepareCommonDeploymentSteps(
          absoluteLocalPath => EnvironmentInfo.GetNetworkPath(machineName, absoluteLocalPath),
          artifactsBinariesDirPathProvider);
      }

      // move cluster group to another node
      string targetNodeName =
        possibleNodeNames.FirstOrDefault(nodeName => nodeName != currentNodeName);

      PostDiagnosticMessage(string.Format("Target node: '{0}'.", targetNodeName), DiagnosticMessageType.Trace);

      if (string.IsNullOrEmpty(targetNodeName))
      {
        throw new InvalidOperationException(string.Format("There is no node in cluster '{0}' that we can move cluster group '{1}' to.", failoverClusterMachineName, clusterGroupName));
      }

      AddSubTask(
          new MoveClusterGroupToAnotherNodeDeploymentStep(
            _failoverClusterManager,
            failoverClusterMachineName,
            clusterGroupName,
            targetNodeName));

      // update nt service on the machine that was the previous owner node
      string previousMachineName = currentNodeName;

      DoPrepareCommonDeploymentSteps(
        absoluteLocalPath => EnvironmentInfo.GetNetworkPath(previousMachineName, absoluteLocalPath),
        artifactsBinariesDirPathProvider);

    }

    private void DoPrepareCommonDeploymentSteps(
      Func<string, string> getAppServerNetworkPathFunc,
      Lazy<string> artifactsBinariesDirPathProvider)
    {
      string targetDirPath = Path.Combine(_environmentInfo.NtServicesBaseDirPath, _extendedProjectInfo.NtServiceDirName, _extendedProjectInfo.ExtensionsDirName);

      AddSubTask(
        new CopyFilesDeploymentStep(
          _directoryAdapter,
          new Lazy<string>(() => artifactsBinariesDirPathProvider.Value),
          new Lazy<string>(() => getAppServerNetworkPathFunc(targetDirPath))));
    }


  }
}