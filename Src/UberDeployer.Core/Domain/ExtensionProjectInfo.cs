using System;
using System.Collections.Generic;
using System.Linq;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain.Input;

namespace UberDeployer.Core.Domain
{
  public class ExtensionProjectInfo : ProjectInfo
  {
    public string ExtendedProjectName { get; set; }

    public ExtensionProjectInfo(
      string name, 
      string artifactsRepositoryName, 
      IEnumerable<string> allowedEnvironmentNames, 
      string artifactsRepositoryDirName, 
      bool artifactsAreNotEnvironmentSpecific, 
      string extendedProjectName)
      : base(name, artifactsRepositoryName, allowedEnvironmentNames, artifactsRepositoryDirName, artifactsAreNotEnvironmentSpecific)
    {
      Guard.NotNullNorEmpty(extendedProjectName, "extendedProjectName");

      ExtendedProjectName = extendedProjectName;
    }

    #region Overrides of ProjectInfo

    public override ProjectType Type
    {
      get { return ProjectType.Extension; }
    }

    public override InputParams CreateEmptyInputParams()
    {
      return new ExtensionInputParams();
    }

    public override DeploymentTask CreateDeploymentTask(IObjectFactory objectFactory)
    {
      if (objectFactory == null)
      {
        throw new ArgumentNullException("objectFactory");
      }

      return
        new DeployExtensionProjectDeploymentTask(
          objectFactory.CreateProjectInfoRepository(),
          objectFactory.CreateEnvironmentInfoRepository(),
          objectFactory.CreateArtifactsRepository(),
          objectFactory.CreateDirectoryAdapter(),
          objectFactory.CreateFileAdapter(),
          objectFactory.CreateZipFileAdapter(),
          objectFactory.CreateFailoverClusterManager(),
          objectFactory.CreateNtServiceManager());
    }

    public override IEnumerable<string> GetTargetFolders(IObjectFactory objectFactory, EnvironmentInfo environmentInfo)
    {
      Guard.NotNull(objectFactory, "objectFactory");
      Guard.NotNull(environmentInfo, "environmentInfo");

      return
        environmentInfo.TerminalAppsBaseDirPath
          .Select(machineName => environmentInfo.GetSchedulerServerNetworkPath(machineName.ToString(), ""))
          .ToList();
    }

    public override string GetMainAssemblyFileName()
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
