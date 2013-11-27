using System;
using System.Collections.Generic;
using System.Linq;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Deployment;
using System.IO;
using UberDeployer.Core.Domain.Input;

namespace UberDeployer.Core.Domain
{
  public class SchedulerAppProjectInfo : ProjectInfo
  {
    #region Constructor(s)

    public SchedulerAppProjectInfo(string name, string artifactsRepositoryName, IEnumerable<string> allowedEnvironmentNames, string artifactsRepositoryDirName, bool artifactsAreNotEnvironmentSpecific, string schedulerAppDirName, string schedulerAppExeName, IEnumerable<SchedulerAppTask> schedulerTasks)
      : base(name, artifactsRepositoryName, allowedEnvironmentNames, artifactsRepositoryDirName, artifactsAreNotEnvironmentSpecific)
    {
      Guard.NotNullNorEmpty(schedulerAppDirName, "schedulerAppDirName");
      Guard.NotNullNorEmpty(schedulerAppExeName, "schedulerAppExeName");

      if (schedulerTasks == null)
      {
        throw new ArgumentNullException("schedulerTasks");
      }
      
      SchedulerAppDirName = schedulerAppDirName;
      SchedulerAppExeName = schedulerAppExeName;
      SchedulerTasks = schedulerTasks.ToList();
    }

    #endregion

    #region Overrides of ProjectInfo

    public override ProjectType Type
    {
      get { return ProjectType.SchedulerApp; }
    }

    public override InputParams CreateEmptyInputParams()
    {
      return new SchedulerAppInputParams();
    }

    public override DeploymentTask CreateDeploymentTask(IObjectFactory objectFactory)
    {
      if (objectFactory == null)
      {
        throw new ArgumentNullException("objectFactory");
      }

      return
        new DeploySchedulerAppDeploymentTask(
          objectFactory.CreateProjectInfoRepository(),
          objectFactory.CreateEnvironmentInfoRepository(),
          objectFactory.CreateArtifactsRepository(),
          objectFactory.CreateTaskScheduler(),
          objectFactory.CreatePasswordCollector(),
          objectFactory.CreateDirectoryAdapter());
    }

    public override IEnumerable<string> GetTargetFolders(IObjectFactory objectFactory, EnvironmentInfo environmentInfo)
    {
      Guard.NotNull(objectFactory, "objectFactory");
      Guard.NotNull(environmentInfo, "environmentInfo");

      return
        new List<string>
          {
            environmentInfo.GetSchedulerServerNetworkPath(
              Path.Combine(environmentInfo.SchedulerAppsBaseDirPath, SchedulerAppDirName))
          };
    }

    public override string GetMainAssemblyFileName()
    {
      return SchedulerAppExeName;
    }

    #endregion

    #region Properties

    public string SchedulerAppDirName { get; private set; }

    public string SchedulerAppExeName { get; private set; }

    public List<SchedulerAppTask> SchedulerTasks { get; private set; }

    #endregion
  }
}
