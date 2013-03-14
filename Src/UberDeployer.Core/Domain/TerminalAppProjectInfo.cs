using System;
using System.Collections.Generic;
using System.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain.Input;

namespace UberDeployer.Core.Domain
{
  public class TerminalAppProjectInfo : ProjectInfo
  {
    #region Constructor(s)

    public TerminalAppProjectInfo(string name, string artifactsRepositoryName, string artifactsRepositoryDirName, bool artifactsAreNotEnvironmentSpecific, string terminalAppName, string terminalAppDirName, string terminalAppExeName)
      : base(name, artifactsRepositoryName, artifactsRepositoryDirName, artifactsAreNotEnvironmentSpecific)
    {
      Guard.NotNullNorEmpty(terminalAppName, "terminalAppName");
      Guard.NotNullNorEmpty(terminalAppDirName, "terminalAppDirName");
      Guard.NotNullNorEmpty(terminalAppExeName, "terminalAppExeName");

      TerminalAppName = terminalAppName;
      TerminalAppDirName = terminalAppDirName;
      TerminalAppExeName = terminalAppExeName;
    }

    #endregion

    #region Overrides of ProjectInfo

    public override ProjectType Type
    {
      get { return ProjectType.TerminalApp; }
    }

    public override InputParams CreateEmptyInputParams()
    {
      return new TerminalAppInputParams();
    }

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
