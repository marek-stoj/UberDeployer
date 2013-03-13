using System;
using System.Collections.Generic;
using System.IO;
using UberDeployer.Core.Deployment;

namespace UberDeployer.Core.Domain
{
  public class TerminalAppProjectInfo : ProjectInfo
  {
    #region Constructor(s)

    public TerminalAppProjectInfo(string name, string artifactsRepositoryName, string artifactsRepositoryDirName, bool artifactsAreNotEnvironmentSpecific, string terminalAppName, string terminalAppDirName, string terminalAppExeName)
      : base(name, artifactsRepositoryName, artifactsRepositoryDirName, artifactsAreNotEnvironmentSpecific)
    {
      if (string.IsNullOrEmpty(terminalAppName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "terminalAppName");
      }

      if (string.IsNullOrEmpty(terminalAppDirName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "terminalAppDirName");
      }

      if (string.IsNullOrEmpty(terminalAppExeName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "terminalAppExeName");
      }

      TerminalAppName = terminalAppName;
      TerminalAppDirName = terminalAppDirName;
      TerminalAppExeName = terminalAppExeName;
    }

    #endregion

    #region Overrides of ProjectInfo

    public override DeploymentTask CreateDeploymentTask(IObjectFactory objectFactory)
    {
      if (objectFactory == null)
      {
        throw new ArgumentNullException("objectFactory");
      }

      return
        new DeployTerminalAppDeploymentTask(
          objectFactory.CreateEnvironmentInfoRepository(),
          objectFactory.CreateArtifactsRepository());
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
            environmentInfo.GetTerminalServerNetworkPath(
              Path.Combine(environmentInfo.TerminalAppsBaseDirPath, TerminalAppDirName))
          };
    }

    #endregion

    #region Properties

    public string TerminalAppName { get; private set; }

    public string TerminalAppDirName { get; private set; }

    public string TerminalAppExeName { get; private set; }

    #endregion
  }
}
