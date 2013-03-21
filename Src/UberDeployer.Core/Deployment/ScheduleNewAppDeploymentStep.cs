using System;
using System.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.ScheduledTasks;

namespace UberDeployer.Core.Deployment
{
  public class ScheduleNewAppDeploymentStep : DeploymentStep
  {
    private readonly ITaskScheduler _taskScheduler;
    private readonly string _machineName;
    private readonly string _executablePath;
    private readonly string _userName;
    private readonly string _password;

    #region Constructor(s)

    public ScheduleNewAppDeploymentStep(
      ProjectInfo projectInfo,
      ITaskScheduler taskScheduler,
      string machineName,
      string executablePath,
      string userName,
      string password)
      : base(projectInfo)
    {
      Guard.NotNull(taskScheduler, "taskScheduler");
      Guard.NotNullNorEmpty(machineName, "machineName");
      Guard.NotNullNorEmpty(executablePath, "executablePath");

      if (!Path.IsPathRooted(executablePath))
      {
        throw new ArgumentException(string.Format("Executable path ('{0}') is not an absolute path.", executablePath), "executablePath");
      }

      Guard.NotNullNorEmpty(userName, "userName");
      Guard.NotNullNorEmpty(password, "password");

      _taskScheduler = taskScheduler;
      _machineName = machineName;
      _executablePath = executablePath;
      _userName = userName;
      _password = password;
    }

    #endregion

    #region Overrides of DeploymentStep

    protected override void DoExecute()
    {
      var schedulerAppProjectInfo = (SchedulerAppProjectInfo)ProjectInfo;
      string taskName = schedulerAppProjectInfo.SchedulerAppName;
      int scheduledHour = schedulerAppProjectInfo.ScheduledHour;
      int scheduledMinute = schedulerAppProjectInfo.ScheduledMinute;
      int executionTimeLimitInMinutes = schedulerAppProjectInfo.ExecutionTimeLimitInMinutes;

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
        var schedulerAppProjectInfo = (SchedulerAppProjectInfo)ProjectInfo;

        return
          string.Format(
            "Schedule new app named '{0}' on machine '{1}' to run daily at '{2}:{3}' with execution time limit of '{4}' minutes under user named '{5}'.",
            schedulerAppProjectInfo.Name,
            _machineName,
            schedulerAppProjectInfo.ScheduledHour.ToString().PadLeft(2, '0'),
            schedulerAppProjectInfo.ScheduledMinute.ToString().PadLeft(2, '0'),
            schedulerAppProjectInfo.ExecutionTimeLimitInMinutes,
            _userName);
      }
    }

    #endregion
  }
}
