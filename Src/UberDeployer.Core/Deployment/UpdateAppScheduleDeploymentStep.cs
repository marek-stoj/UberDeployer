using System;
using System.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.ScheduledTasks;

namespace UberDeployer.Core.Deployment
{
  public class UpdateAppScheduleDeploymentStep : DeploymentStep
  {
    private readonly ITaskScheduler _taskScheduler;
    private readonly string _machineName;
    private readonly string _executablePath;
    private readonly string _userName;
    private readonly string _password;

    private SchedulerAppProjectInfo _schedulerAppProjectInfo
    {
      get { return (SchedulerAppProjectInfo) DeploymentInfo.ProjectInfo; }
    }

    #region Constructor(s)

    // TODO IMM HI: can we update scheduler app without user name and password?
    public UpdateAppScheduleDeploymentStep(
      ITaskScheduler taskScheduler,
      string machineName,      
      string executablePath,
      string userName,
      string password)
    {
      Guard.NotNull(taskScheduler, "taskScheduler");
      Guard.NotNullNorEmpty(machineName, "machineName");
      Guard.NotNullNorEmpty(executablePath, "executablePath");
      Guard.NotNullNorEmpty(userName, "userName");
      Guard.NotNullNorEmpty(password, "password");
      
      if (!Path.IsPathRooted(executablePath))
      {
        throw new ArgumentException(string.Format("Executable path ('{0}') is not an absolute path.", executablePath), "executablePath");
      }

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

      _taskScheduler.UpdateTaskSchedule(
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
          "Update schedule of app named '{0}' on machine '{1}' to run daily at '{2}:{3}' with execution time limit of '{4}' minutes.",
          _schedulerAppProjectInfo.Name,
          _machineName,
          _schedulerAppProjectInfo.ScheduledHour.ToString().PadLeft(2, '0'),
          _schedulerAppProjectInfo.ScheduledMinute.ToString().PadLeft(2, '0'),
          _schedulerAppProjectInfo.ExecutionTimeLimitInMinutes);
      }
    }

    #endregion
  }
}
