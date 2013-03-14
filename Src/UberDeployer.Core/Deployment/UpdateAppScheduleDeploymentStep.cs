using System;
using System.IO;
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

    #region Constructor(s)

    // TODO IMM HI: can we update scheduler app without user name and password?
    public UpdateAppScheduleDeploymentStep(
      ITaskScheduler taskScheduler,
      string machineName,      
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
      _executablePath = executablePath;
      _userName = userName;
      _password = password;
    }

    #endregion

    #region Overrides of DeploymentStep

    protected override void DoExecute()
    {
      var schedulerAppProjectInfo = (SchedulerAppProjectInfo) DeploymentInfo.ProjectInfo;
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
        var schedulerAppProjectInfo = (SchedulerAppProjectInfo)DeploymentInfo.ProjectInfo;

        return
          string.Format(
          "Update schedule of app named '{0}' on machine '{1}' to run daily at '{2}:{3}' with execution time limit of '{4}' minutes.",
          schedulerAppProjectInfo.Name,
          _machineName,
          schedulerAppProjectInfo.ScheduledHour.ToString().PadLeft(2, '0'),
          schedulerAppProjectInfo.ScheduledMinute.ToString().PadLeft(2, '0'),
          schedulerAppProjectInfo.ExecutionTimeLimitInMinutes);
      }
    }

    #endregion
  }
}
