using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UberDeployer.Core.Deployment;

namespace UberDeployer.Core.Domain
{
  public class NtServiceProjectInfo : ProjectInfo
  {
    #region Constructor(s)

    public NtServiceProjectInfo(string name, string artifactsRepositoryName, string artifactsRepositoryDirName, string ntServiceName, string ntServiceDirName, string ntServiceDisplayName, string ntServiceExeName, string ntServiceUserId)
      : base(name, artifactsRepositoryName, artifactsRepositoryDirName)
    {
      if (string.IsNullOrEmpty(ntServiceName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "ntServiceName");
      }

      if (string.IsNullOrEmpty(ntServiceDirName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "ntServiceDirName");
      }

      if (string.IsNullOrEmpty(ntServiceExeName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "ntServiceExeName");
      }

      if (string.IsNullOrEmpty(ntServiceUserId))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "ntServiceUserId");
      }

      NtServiceName = ntServiceName;
      NtServiceDisplayName = ntServiceDisplayName;
      NtServiceDirName = ntServiceDirName;
      NtServiceExeName = ntServiceExeName;
      NtServiceUserId = ntServiceUserId;
    }

    #endregion

    #region Overrides of ProjectInfo

    public override DeploymentTask CreateDeploymentTask(IObjectFactory objectFactory, string projectConfigurationName, string projectConfigurationBuildId, string targetEnvironmentName)
    {
      if (objectFactory == null)
      {
        throw new ArgumentNullException("objectFactory");
      }

      return
        new DeployNtServiceDeploymentTask(
          objectFactory.CreateEnvironmentInfoRepository(),
          objectFactory.CreateArtifactsRepository(),
          objectFactory.CreateNtServiceManager(),
          objectFactory.CreatePasswordCollector(),
          this,
          projectConfigurationName,
          projectConfigurationBuildId,
          targetEnvironmentName);
    }

    public override IEnumerable<string> GetTargetFolders(EnvironmentInfo environmentInfo)
    {
      if (environmentInfo == null)
      {
        throw new ArgumentNullException("environmentInfo");
      }

      return
        new List<string>
          {
            environmentInfo.GetAppServerNetworkPath(
              Path.Combine(environmentInfo.NtServicesBaseDirPath, NtServiceDirName))
          };
    }

    #endregion

    #region Properties

    [Category("Specific")]
    public string NtServiceName { get; private set; }

    [Category("Specific")]
    public string NtServiceDirName { get; private set; }

    [Category("Specific")]
    public string NtServiceDisplayName { get; private set; }

    [Category("Specific")]
    public string NtServiceExeName { get; private set; }

    /// <summary>
    /// A reference to a user that will be used to run the scheduled task. Users are defined in target environments.
    /// </summary>
    [Category("Specific")]
    public string NtServiceUserId { get; private set; }

    #endregion
  }
}
