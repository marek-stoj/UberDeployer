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

    private NtServiceProjectInfo _ntServiceProjectInfo;

    #region Constructor(s)

    public DeployNtServiceDeploymentTask(
      IEnvironmentInfoRepository environmentInfoRepository,
      IArtifactsRepository artifactsRepository,
      INtServiceManager ntServiceManager,
      IPasswordCollector passwordCollector,
      IFailoverClusterManager failoverClusterManager,
      NtServiceProjectInfo projectInfo)
      : base(environmentInfoRepository)
    {
      if (artifactsRepository == null)
      {
        throw new ArgumentNullException("artifactsRepository");
      }

      if (ntServiceManager == null)
      {
        throw new ArgumentNullException("ntServiceManager");
      }

      if (projectInfo == null)
      {
        throw new ArgumentNullException("projectInfo");
      }

      if (passwordCollector == null)
      {
        throw new ArgumentNullException("passwordCollector");
      }

      if (failoverClusterManager == null)
      {
        throw new ArgumentNullException("failoverClusterManager");
      }

      _artifactsRepository = artifactsRepository;
      _ntServiceManager = ntServiceManager;
      _passwordCollector = passwordCollector;
      _failoverClusterManager = failoverClusterManager;
    }

    #endregion Constructor(s)

    #region Overrides of DeploymentTaskBase

    protected override void DoPrepare()
    {
      EnvironmentInfo environmentInfo = GetEnvironmentInfo();
      _ntServiceProjectInfo = DeploymentInfo.ProjectInfo as NtServiceProjectInfo;

      Guard.NotNull(_ntServiceProjectInfo, "_ntServiceProjectInfo");

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

      if (DeploymentInfo.ProjectInfo.ArtifactsAreEnvironmentSpecific)
      {
        var binariesConfiguratorStep = new ConfigureBinariesStep(
          environmentInfo.ConfigurationTemplateName, GetTempDirPath());

        AddSubTask(binariesConfiguratorStep);
      }

      bool deployToClusteredEnvironment = environmentInfo.EnableFailoverClusteringForNtServices;

      if (deployToClusteredEnvironment)
      {
        if (string.IsNullOrEmpty(environmentInfo.GetFailoverClusterGroupNameForProject(DeploymentInfo.ProjectName)))
        {
          PostDiagnosticMessage(string.Format("Failover clustering for NT services is enabled for environment '{0}' but there is no cluster group mapping for project '{1}'.", environmentInfo.Name, DeploymentInfo.ProjectName), DiagnosticMessageType.Warn);

          deployToClusteredEnvironment = false;
        }
      }

      if (deployToClusteredEnvironment)
      {
        DoPrepareDeploymentToClusteredEnvironment(
          environmentInfo,
          extractArtifactsDeploymentStep.BinariesDirPath);
      }
      else
      {
        DoPrepareDeploymentToStandardEnvironment(
          environmentInfo,
          extractArtifactsDeploymentStep.BinariesDirPath);
      }
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Deploy NT service '{0} ({1}:{2})' to '{3}'.",
            DeploymentInfo.ProjectInfo.Name,
            DeploymentInfo.ProjectConfigurationName,
            DeploymentInfo.ProjectConfigurationBuildId,
            DeploymentInfo.TargetEnvironmentName);
      }
    }

    #endregion Overrides of DeploymentTaskBase    

    #region Private methods

    private void DoPrepareDeploymentToStandardEnvironment(EnvironmentInfo environmentInfo, string artifactsBinariesDirPath)
    {
      Func<CollectedCredentials> collectCredentialsFunc =
        () =>
        {
          EnvironmentUser environmentUser;                    

          string environmentUserPassword =
            PasswordCollectorHelper.CollectPasssword(
              _passwordCollector,
              environmentInfo,
              environmentInfo.AppServerMachineName,
              _ntServiceProjectInfo.NtServiceUserId,
              out environmentUser);

          return
            new CollectedCredentials(
              environmentUser.UserName,
              environmentUserPassword);
        };

      DoPrepareCommonDeploymentSteps(
        _ntServiceProjectInfo.NtServiceName,
        environmentInfo.AppServerMachineName,
        environmentInfo.NtServicesBaseDirPath,
        environmentInfo.GetAppServerNetworkPath,
        artifactsBinariesDirPath,
        collectCredentialsFunc,
        true);
    }

    private void DoPrepareDeploymentToClusteredEnvironment(EnvironmentInfo environmentInfo, string artifactsBinariesDirPath)
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

      List<string> possibleNodeNames =
        _failoverClusterManager.GetPossibleNodeNames(failoverClusterMachineName, clusterGroupName)
          .ToList();

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

          EnvironmentUser environmentUser;

          string environmentUserPassword =
            PasswordCollectorHelper.CollectPasssword(
              _passwordCollector,
              environmentInfo,
              machineName,
              _ntServiceProjectInfo.NtServiceUserId,
              out environmentUser);

          cachedCollectedCredentials =
            new CollectedCredentials(
              environmentUser.UserName,
              environmentUserPassword);

          return cachedCollectedCredentials;
        };

      foreach (string possibleNodeName in possibleNodeNames)
      {
        string machineName = possibleNodeName;

        if (machineName == currentNodeName)
        {
          continue;
        }

        DoPrepareCommonDeploymentSteps(
          _ntServiceProjectInfo.NtServiceName,
          machineName,
          environmentInfo.NtServicesBaseDirPath,
          absoluteLocalPath => EnvironmentInfo.GetNetworkPath(machineName, absoluteLocalPath),
          artifactsBinariesDirPath,
          collectCredentialsFunc(machineName),
          false);
      }

      // move cluster group to another node
      string targetNodeName =
        possibleNodeNames.FirstOrDefault(nodeName => nodeName != currentNodeName);

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
        _ntServiceProjectInfo.NtServiceName,
        previousMachineName,
        environmentInfo.NtServicesBaseDirPath,
        absoluteLocalPath => EnvironmentInfo.GetNetworkPath(previousMachineName, absoluteLocalPath),
        artifactsBinariesDirPath,
        collectCredentialsFunc(previousMachineName),
        false);
    }

    private void DoPrepareCommonDeploymentSteps(string ntServiceName, string appServerMachineName, string ntServicesBaseDirPath, Func<string, string> getAppServerNetworkPathFunc, string artifactsBinariesDirPath, Func<CollectedCredentials> collectCredentialsFunc, bool startServiceAfterDeployment)
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
            _ntServiceProjectInfo.NtServiceName));
      }

      // create a step for copying the binaries to the target machine
      string targetDirPath = Path.Combine(ntServicesBaseDirPath, _ntServiceProjectInfo.NtServiceDirName);

      // create a backup step if needed
      string targetDirNetworkPath = getAppServerNetworkPathFunc(targetDirPath);

      if (Directory.Exists(targetDirNetworkPath))
      {
        AddSubTask(new BackupFilesDeploymentStep(targetDirNetworkPath));
      }

      AddSubTask(
        new CopyFilesDeploymentStep(
          artifactsBinariesDirPath,
          getAppServerNetworkPathFunc(targetDirPath)));

      if (!serviceExists)
      {
        // collect credentials
        CollectedCredentials collectedCredentials = collectCredentialsFunc();

        // create a step for installing the service,
        string serviceExecutablePath = Path.Combine(targetDirPath, _ntServiceProjectInfo.NtServiceExeName);

        var ntServiceDescriptor =
          new NtServiceDescriptor(
            _ntServiceProjectInfo.NtServiceName,
            serviceExecutablePath,
            ServiceAccount.NetworkService,
            ServiceStartMode.Automatic,
            _ntServiceProjectInfo.NtServiceDisplayName,
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
            _ntServiceProjectInfo.NtServiceName));
      }
    }

    #endregion Private methods
  }
}