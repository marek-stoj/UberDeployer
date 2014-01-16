using System;
using System.IO;
using System.Reflection;
using UberDeployer.Common;
using UberDeployer.Common.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.FailoverCluster;
using UberDeployer.Core.Management.NtServices;

namespace UberDeployer.Core.Deployment
{
  public class DeployUberDeployerAgentDeploymentTask : DeployNtServiceDeploymentTask
  {
    private readonly IDirectoryAdapter _directoryAdapter;

    #region Constructor(s)

    public DeployUberDeployerAgentDeploymentTask(IProjectInfoRepository projectInfoRepository, IEnvironmentInfoRepository environmentInfoRepository, IArtifactsRepository artifactsRepository, INtServiceManager ntServiceManager, IPasswordCollector passwordCollector, IFailoverClusterManager failoverClusterManager, IDirectoryAdapter directoryAdapter, IFileAdapter fileAdapter, IZipFileAdapter zipFileAdapter)
      : base(projectInfoRepository, environmentInfoRepository, artifactsRepository, ntServiceManager, passwordCollector, failoverClusterManager, directoryAdapter, fileAdapter, zipFileAdapter)
    {
      Guard.NotNull(directoryAdapter, "directoryAdapter");

      _directoryAdapter = directoryAdapter;
    }

    #endregion

    #region Overrides of DeploymentTaskBase

    protected override void DoPrepare()
    {
      string currentExeFileName =
        Path.GetFileName(
          ReflectionUtils.GetCodeBaseFilePath(
            Assembly.GetEntryAssembly()));

      bool isRunningInsideAgent =
        string.Equals(currentExeFileName, "UberDeployer.Agent.NtService.exe", StringComparison.OrdinalIgnoreCase);

      if (isRunningInsideAgent)
      {
        var deployUberDeployerAgentStep =
          new DeployUberDeployerAgentUsingConsoleAppDeploymentStep(
            DeploymentInfo,
            _directoryAdapter);

        AddSubTask(deployUberDeployerAgentStep);
      }
      else
      {
        base.DoPrepare();
      }
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Deploy UberDeployer Agent '{0} ({1}:{2})' to '{3}'.",
            DeploymentInfo.ProjectName,
            DeploymentInfo.ProjectConfigurationName,
            DeploymentInfo.ProjectConfigurationBuildId,
            DeploymentInfo.TargetEnvironmentName);
      }
    }

    #endregion
  }
}
