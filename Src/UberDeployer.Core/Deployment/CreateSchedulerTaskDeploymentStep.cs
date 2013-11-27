using System;
using System.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Management.ScheduledTasks;

namespace UberDeployer.Core.Deployment
{
  public class CreateSchedulerTaskDeploymentStep : DeploymentStep
  {
    private readonly ITaskScheduler _taskScheduler;
    private readonly string _machineName;
    private readonly string _schedulerTaskName;
    private readonly string _executablePath;
    private readonly string _userName;
    private readonly string _password;
    private readonly int _scheduledHour;
    private readonly int _scheduledMinute;
    private readonly int _executionTimeLimitInMinutes;
    private readonly RepetitionSpecification _repetitionSpecification;

    #region Constructor(s)

    public CreateSchedulerTaskDeploymentStep(
      string machineName,
      string schedulerTaskName,
      string executablePath,
      string userName,
      string password,
      int scheduledHour,
      int scheduledMinute,
      int executionTimeLimitInMinutes,
      RepetitionSpecification repetitionSpecification,
      ITaskScheduler taskScheduler)
    {
      Guard.NotNull(taskScheduler, "taskScheduler");
      Guard.NotNullNorEmpty(machineName, "machineName");
      Guard.NotNullNorEmpty(schedulerTaskName, "schedulerTaskName");
      Guard.NotNullNorEmpty(executablePath, "executablePath");
      Guard.NotNullNorEmpty(userName, "userName");
      Guard.NotNullNorEmpty(password, "password");
      Guard.NotNull(repetitionSpecification, "repetitionSpecification");

      if (!Path.IsPathRooted(executablePath))
      {
        throw new ArgumentException(string.Format("Executable path ('{0}') is not an absolute path.", executablePath), "executablePath");
      }

      _taskScheduler = taskScheduler;
      _machineName = machineName;
      _schedulerTaskName = schedulerTaskName;
      _executablePath = executablePath;
      _userName = userName;
      _password = password;
      _scheduledHour = scheduledHour;
      _scheduledMinute = scheduledMinute;
      _executionTimeLimitInMinutes = executionTimeLimitInMinutes;
      _repetitionSpecification = repetitionSpecification;
    }

    #endregion

    #region Overrides of DeploymentStep

    protected override void DoExecute()
    {
      var scheduledTaskSpecification =
        new ScheduledTaskSpecification(
          _schedulerTaskName,
          _executablePath,
          _scheduledHour,
          _scheduledMinute,
          _executionTimeLimitInMinutes,
          _repetitionSpecification);

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
            "Create new scheduler task named '{0}' on machine '{1}' to run daily at '{2}:{3}' with execution time limit of '{4}' minutes under user named '{5}'.",
            _schedulerTaskName,
            _machineName,
            _scheduledHour.ToString().PadLeft(2, '0'),
            _scheduledMinute.ToString().PadLeft(2, '0'),
            _executionTimeLimitInMinutes,
            _userName);
      }
    }

    #endregion
  }
}
