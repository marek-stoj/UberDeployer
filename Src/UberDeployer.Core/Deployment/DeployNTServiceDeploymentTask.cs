using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.FailoverCluster;
using UberDeployer.Core.Management.NtServices;

namespace UberDeployer.Core.Deployment
{
  public class DeployNtServiceDeploymentTask : DeploymentTask
  {
    private readonly IArtifactsRepository _artifactsRepository;

    private readonly INtServiceManager _ntServiceManager;
    private readonly IPasswordCollector _passwordCollector;
    private readonly IFailoverClusterManager _failoverClusterManager;

    private NtServiceProjectInfo _projectInfo;

    #region Constructor(s)

    public DeployNtServiceDeploymentTask(
      IProjectInfoRepository projectInfoRepository,
      IEnvironmentInfoRepository environmentInfoRepository,
      IArtifactsRepository artifactsRepository,
      INtServiceManager ntServiceManager,
      IPasswordCollector passwordCollector,
      IFailoverClusterManager failoverClusterManager)
      : base(projectInfoRepository, environmentInfoRepository)
    {
      Guard.NotNull(artifactsRepository, "artifactsRepository");
      Guard.NotNull(ntServiceManager, "ntServiceManager");
      Guard.NotNull(passwordCollector, "passwordCollector");
      Guard.NotNull(failoverClusterManager, "failoverClusterManager");

      _artifactsRepository = artifactsRepository;
      _ntServiceManager = ntServiceManager;
      _passwordCollector = passwordCollector;
      _failoverClusterManager = failoverClusterManager;
    }

    #endregion

    #region Overrides of DeploymentTaskBase

    protected override void DoPrepare()
    {
      EnvironmentInfo environmentInfo = GetEnvironmentInfo();

      _projectInfo = GetProjectInfo<NtServiceProjectInfo>();

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
          environmentInfo,
          DeploymentInfo,
          downloadArtifactsDeploymentStep.ArtifactsFilePath,
          GetTempDirPath());

      AddSubTask(extractArtifactsDeploymentStep);

      if (_projectInfo.ArtifactsAreEnvironmentSpecific)
      {
        var binariesConfiguratorStep =
          new ConfigureBinariesStep(
            environmentInfo.ConfigurationTemplateName,
            GetTempDirPath());

        AddSubTask(binariesConfiguratorStep);
      }

      bool deployToClusteredEnvironment =
        environmentInfo.EnableFailoverClusteringForNtServices;

      if (deployToClusteredEnvironment)
      {
        PostDiagnosticMessage("Will deploy to a clustered environment.", DiagnosticMessageType.Trace);

        if (string.IsNullOrEmpty(environmentInfo.GetFailoverClusterGroupNameForProject(DeploymentInfo.ProjectName)))
        {
          throw new InvalidOperationException(string.Format("Failover clustering for NT services is enabled for environment '{0}' but there is no cluster group mapping for project '{1}'.", environmentInfo.Name, DeploymentInfo.ProjectName));
        }

        DoPrepareDeploymentToClusteredEnvironment(
          environmentInfo,
          new Lazy<string>(() => extractArtifactsDeploymentStep.BinariesDirPath));
      }
      else
      {
        PostDiagnosticMessage("Will deploy to a non-clustered environment.", DiagnosticMessageType.Trace);

        DoPrepareDeploymentToStandardEnvironment(
          environmentInfo,
          new Lazy<string>(() => extractArtifactsDeploymentStep.BinariesDirPath));
      }
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Deploy NT service '{0} ({1}:{2})' to '{3}'.",
            DeploymentInfo.ProjectName,
            DeploymentInfo.ProjectConfigurationName,
            DeploymentInfo.ProjectConfigurationBuildId,
            DeploymentInfo.TargetEnvironmentName);
      }
    }

    #endregion

    #region Private methods

    private void DoPrepareDeploymentToStandardEnvironment(EnvironmentInfo environmentInfo, Lazy<string> artifactsBinariesDirPathProvider)
    {
      Func<CollectedCredentials> collectCredentialsFunc =
        () =>
        {
          EnvironmentUser environmentUser =
            environmentInfo.GetEnvironmentUserById(_projectInfo.NtServiceUserId);

          string environmentUserPassword =
            PasswordCollectorHelper.CollectPasssword(
              _passwordCollector,
              DeploymentInfo.DeploymentId,
              environmentInfo,
              environmentInfo.AppServerMachineName,
              environmentUser, 
              OnDiagnosticMessagePosted);

          return
            new CollectedCredentials(
              environmentUser.UserName,
              environmentUserPassword);
        };

      DoPrepareCommonDeploymentSteps(
        _projectInfo.NtServiceName,
        environmentInfo.AppServerMachineName,
        environmentInfo.NtServicesBaseDirPath,
        environmentInfo.GetAppServerNetworkPath,
        artifactsBinariesDirPathProvider,
        collectCredentialsFunc,
        true);
    }

    private void DoPrepareDeploymentToClusteredEnvironment(EnvironmentInfo environmentInfo, Lazy<string> artifactsBinariesDirPathProvider)
    {
      string clusterGroupName = environmentInfo.GetFailoverClusterGroupNameForProject(DeploymentInfo.ProjectName);

      if (string.IsNullOrEmpty(clusterGroupName))
      {
        throw new InvalidOperationException(string.Format("There is no cluster group defined for project '{0}' in environment '{1}'.", DeploymentInfo.ProjectName, environmentInfo.Name));
      }

      string failoverClusterMachineName = environmentInfo.FailoverClusterMachineName;

      if (string.IsNullOrEmpty(failoverClusterMachineName))
      {
        throw new InvalidOperationException(string.Format("Environment '{0}' has no failover cluster machine name defined.", environmentInfo.Name));
      }

      string currentNodeName =
        _failoverClusterManager.GetCurrentNodeName(failoverClusterMachineName, clusterGroupName);

      if (string.IsNullOrEmpty(currentNodeName))
      {
        throw new InvalidOperationException(string.Format("Cluster group '{0}' has no current node in a cluster '{1}' in environment '{2}'.", clusterGroupName, environmentInfo.FailoverClusterMachineName, environmentInfo.Name));
      }

      PostDiagnosticMessage(string.Format("Current node: '{0}'.", currentNodeName), DiagnosticMessageType.Trace);

      List<string> possibleNodeNames =
        _failoverClusterManager.GetPossibleNodeNames(failoverClusterMachineName, clusterGroupName)
          .ToList();

      PostDiagnosticMessage(string.Format("Possible nodes: {0}.", string.Join(", ", possibleNodeNames.Select(n => string.Format("'{0}'", n)))), DiagnosticMessageType.Trace);

      if (possibleNodeNames.Count < 2)
      {
        throw new InvalidOperationException(string.Format("There is only one possible node for cluster group '{0}' in a cluster '{1}' in environment '{2}'.", clusterGroupName, environmentInfo.FailoverClusterMachineName, environmentInfo.Name));
      }

      // update nt service on all machines other than current owner node
      CollectedCredentials cachedCollectedCredentials = null;

      Func<string, Func<CollectedCredentials>> collectCredentialsFunc =
        machineName =>
        () =>
        {
          // ReSharper disable AccessToModifiedClosure
          if (cachedCollectedCredentials != null)
          {
            return cachedCollectedCredentials;
          }
          // ReSharper restore AccessToModifiedClosure

          EnvironmentUser environmentUser =
            environmentInfo.GetEnvironmentUserById(_projectInfo.NtServiceUserId);

          string environmentUserPassword =
            PasswordCollectorHelper.CollectPasssword(
              _passwordCollector,
              DeploymentInfo.DeploymentId,
              environmentInfo,
              machineName,
              environmentUser,
              OnDiagnosticMessagePosted);

          cachedCollectedCredentials =
            new CollectedCredentials(
              environmentUser.UserName,
              environmentUserPassword);

          return cachedCollectedCredentials;
        };

      foreach (string possibleNodeName in possibleNodeNames)
      {
        string machineName = possibleNodeName;

        if (string.Equals(machineName, currentNodeName, StringComparison.OrdinalIgnoreCase))
        {
          continue;
        }

        DoPrepareCommonDeploymentSteps(
          _projectInfo.NtServiceName,
          machineName,
          environmentInfo.NtServicesBaseDirPath,
          absoluteLocalPath => EnvironmentInfo.GetNetworkPath(machineName, absoluteLocalPath),
          artifactsBinariesDirPathProvider,
          collectCredentialsFunc(machineName),
          false);
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
        _projectInfo.NtServiceName,
        previousMachineName,
        environmentInfo.NtServicesBaseDirPath,
        absoluteLocalPath => EnvironmentInfo.GetNetworkPath(previousMachineName, absoluteLocalPath),
        artifactsBinariesDirPathProvider,
        collectCredentialsFunc(previousMachineName),
        false);
    }

    private void DoPrepareCommonDeploymentSteps(string ntServiceName, string appServerMachineName, string ntServicesBaseDirPath, Func<string, string> getAppServerNetworkPathFunc, Lazy<string> artifactsBinariesDirPathProvider, Func<CollectedCredentials> collectCredentialsFunc, bool startServiceAfterDeployment)
    {
      // check if the service is present on the target machine
      bool serviceExists =
        _ntServiceManager
          .DoesServiceExist(appServerMachineName, ntServiceName);

      if (serviceExists)
      {
        // create a step for stopping the service
        AddSubTask(
          new StopNtServiceDeploymentStep(
            _ntServiceManager,
            appServerMachineName,
            _projectInfo.NtServiceName));
      }

      // create a step for copying the binaries to the target machine
      string targetDirPath = Path.Combine(ntServicesBaseDirPath, _projectInfo.NtServiceDirName);

/* // TODO IMM HI: xxx we don't need this for now - should we parameterize this somehow?
      // create a backup step if needed
      string targetDirNetworkPath = getAppServerNetworkPathFunc(targetDirPath);

      if (Directory.Exists(targetDirNetworkPath))
      {
        AddSubTask(
          new BackupFilesDeploymentStep(
            targetDirNetworkPath));
      }
*/

      AddSubTask(
        new CopyFilesDeploymentStep(
          artifactsBinariesDirPathProvider,
          new Lazy<string>(() => getAppServerNetworkPathFunc(targetDirPath))));

      if (!serviceExists)
      {
        // collect credentials
        CollectedCredentials collectedCredentials = collectCredentialsFunc();

        // create a step for installing the service,
        string serviceExecutablePath = Path.Combine(targetDirPath, _projectInfo.NtServiceExeName);

        var ntServiceDescriptor =
          new NtServiceDescriptor(
            _projectInfo.NtServiceName,
            serviceExecutablePath,
            ServiceAccount.NetworkService,
            ServiceStartMode.Automatic,
            _projectInfo.NtServiceDisplayName,
            collectedCredentials.UserName,
            collectedCredentials.Password);

        AddSubTask(
          new InstallNtServiceDeploymentStep(
            _ntServiceManager,
            appServerMachineName,
            ntServiceDescriptor));
      }

      if (startServiceAfterDeployment)
      {
        // create a step for starting the service
        AddSubTask(
          new StartNtServiceDeploymentStep(
            _ntServiceManager,
            appServerMachineName,
            _projectInfo.NtServiceName));
      }
    }

    #endregion
  }
}
