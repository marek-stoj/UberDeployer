using System;
using System.IO;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.ScheduledTasks;

namespace UberDeployer.Core.Deployment
{
  public class ScheduleNewAppDeploymentStep : DeploymentStep
  {
    private readonly ITaskScheduler _taskScheduler;
    private readonly string _machineName;
    private readonly SchedulerAppProjectInfo _schedulerAppProjectInfo;
    private readonly string _executablePath;
    private readonly string _userName;
    private readonly string _password;

    #region Constructor(s)

    public ScheduleNewAppDeploymentStep(
      ITaskScheduler taskScheduler,
      string machineName,
      SchedulerAppProjectInfo schedulerAppProjectInfo,
      string executablePath,
      string userName,
      string password)
    {
      if (taskScheduler == null)
      {
        throw new ArgumentNullException("taskScheduler");
      }

      if (string.IsNullOrEmpty(machineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "machineName");
      }

      if (schedulerAppProjectInfo == null)
      {
        throw new ArgumentNullException("schedulerAppProjectInfo");
      }

      if (string.IsNullOrEmpty(executablePath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "executablePath");
      }
      
      if (!Path.IsPathRooted(executablePath))
      {
        throw new ArgumentException(string.Format("Executable path ('{0}') is not an absolute path.", executablePath), "executablePath");
      }

      if (string.IsNullOrEmpty(userName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "userName");
      }

      if (string.IsNullOrEmpty(password))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "password");
      }

      _taskScheduler = taskScheduler;
      _machineName = machineName;
      _schedulerAppProjectInfo = schedulerAppProjectInfo;
      _executablePath = executablePath;
      _userName = userName;
      _password = password;
    }

    #endregion

    #region Overrides of DeploymentStep

    protected override void DoExecute()
    {
      string taskName = _schedulerAppProjectInfo.SchedulerAppName;
      int scheduledHour = _schedulerAppProjectInfo.ScheduledHour;
      int scheduledMinute = _schedulerAppProjectInfo.ScheduledMinute;
      int executionTimeLimitInMinutes = _schedulerAppProjectInfo.ExecutionTimeLimitInMinutes;

      var scheduledTaskSpecification =
        new ScheduledTaskSpecification(
          taskName,
          _executablePath,
          scheduledHour,
          scheduledMinute,
          executionTimeLimitInMinutes);

      _taskScheduler
        .ScheduleNewTask(
          _machineName,
          scheduledTaskSpecification,
          _userName,
          _password);
    }

    public override string Description
    {
      get
      {
        return
          string.Format(
          "Schedule new app named '{0}' on machine '{1}' to run daily at '{2}:{3}' with execution time limit of '{4}' minutes under user named '{5}'.",
          _schedulerAppProjectInfo.Name,
          _machineName,
          _schedulerAppProjectInfo.ScheduledHour.ToString().PadLeft(2, '0'),
          _schedulerAppProjectInfo.ScheduledMinute.ToString().PadLeft(2, '0'),
          _schedulerAppProjectInfo.ExecutionTimeLimitInMinutes,
          _userName);
      }
    }

    #endregion
  }
}
