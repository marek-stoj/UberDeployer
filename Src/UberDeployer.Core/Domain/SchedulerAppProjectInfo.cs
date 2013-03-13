using System;
using System.Collections.Generic;
using UberDeployer.Core.Deployment;
using System.IO;

namespace UberDeployer.Core.Domain
{
  public class SchedulerAppProjectInfo : ProjectInfo
  {
    #region Constructor(s)

    public SchedulerAppProjectInfo(string name, string artifactsRepositoryName, string artifactsRepositoryDirName, bool artifactsAreNotEnvironmentSpecific, string schedulerAppName, string schedulerAppDirName, string schedulerAppExeName, string schedulerAppUserId, int scheduledHour, int scheduledMinute, int executionTimeLimitInMinutes)
      : base(name, artifactsRepositoryName, artifactsRepositoryDirName, artifactsAreNotEnvironmentSpecific)
    {
      if (string.IsNullOrEmpty(schedulerAppName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "schedulerAppName");
      }

      if (string.IsNullOrEmpty(schedulerAppDirName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "schedulerAppDirName");
      }

      if (string.IsNullOrEmpty(schedulerAppExeName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "schedulerAppExeName");
      }

      if (schedulerAppUserId == null)
      {
        throw new ArgumentNullException("schedulerAppUserId");
      }
      
      if (scheduledHour < 0 || scheduledHour > 23)
      {
        throw new ArgumentException("Hour must be between 0 and 23 (inclusive).", "scheduledHour");
      }

      if (scheduledMinute < 0 || scheduledMinute > 59)
      {
        throw new ArgumentException("Minute must be between 0 and 59 (inclusive).", "scheduledMinute");
      }

      if (executionTimeLimitInMinutes < 0)
      {
        throw new ArgumentException("Execution time limit must be a non-negative integer.", "executionTimeLimitInMinutes");
      }
      
      SchedulerAppName = schedulerAppName;
      SchedulerAppDirName = schedulerAppDirName;
      SchedulerAppExeName = schedulerAppExeName;
      ScheduledHour = scheduledHour;
      ScheduledMinute = scheduledMinute;
      ExecutionTimeLimitInMinutes = executionTimeLimitInMinutes;
      SchedulerAppUserId = schedulerAppUserId;
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
        new DeploySchedulerAppDeploymentTask(
          objectFactory.CreateEnvironmentInfoRepository(),
          objectFactory.CreateArtifactsRepository(),
          objectFactory.CreateTaskScheduler(),
          objectFactory.CreatePasswordCollector());
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
              Path.Combine(environmentInfo.SchedulerAppsBaseDirPath, SchedulerAppDirName))
          };
    }

    #endregion

    #region Properties

    public string SchedulerAppName { get; private set; }

    public string SchedulerAppDirName { get; private set; }

    public string SchedulerAppExeName { get; private set; }

    /// <summary>
    /// A reference to a user that will be used to run the scheduled task. Users are defined in target environments.
    /// </summary>
    public string SchedulerAppUserId { get; private set; }

    public int ScheduledHour { get; private set; }

    public int ScheduledMinute { get; private set; }

    /// <summary>
    /// 0 - no limit.
    /// </summary>
    public int ExecutionTimeLimitInMinutes { get; private set; }

    #endregion
  }
}
