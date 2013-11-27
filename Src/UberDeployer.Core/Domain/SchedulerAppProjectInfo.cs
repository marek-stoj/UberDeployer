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
    private List<SchedulerAppTask> _schedulerAppTasks;

    #region Constructor(s)

    public SchedulerAppProjectInfo(string name, string artifactsRepositoryName, IEnumerable<string> allowedEnvironmentNames, string artifactsRepositoryDirName, bool artifactsAreNotEnvironmentSpecific, string schedulerAppDirName, string schedulerAppExeName, IEnumerable<SchedulerAppTask> schedulerAppTasks)
      : base(name, artifactsRepositoryName, allowedEnvironmentNames, artifactsRepositoryDirName, artifactsAreNotEnvironmentSpecific)
    {
      Guard.NotNullNorEmpty(schedulerAppDirName, "schedulerAppDirName");
      Guard.NotNullNorEmpty(schedulerAppExeName, "schedulerAppExeName");

      if (schedulerAppTasks == null)
      {
        throw new ArgumentNullException("schedulerAppTasks");
      }

      List<SchedulerAppTask> schedulerAppTasksList =
        schedulerAppTasks.ToList();

      if (schedulerAppTasksList.Count == 0)
      {
        throw new ArgumentException("At least one scheduler app task must be specified.", "schedulerAppTasks");
      }
      
      SchedulerAppDirName = schedulerAppDirName;
      SchedulerAppExeName = schedulerAppExeName;
      _schedulerAppTasks = schedulerAppTasksList;
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

    public IEnumerable<SchedulerAppTask> SchedulerAppTasks
    {
      get { return _schedulerAppTasks.AsReadOnly(); }
    }

    #endregion
  }
}
