using System;
using System.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.ScheduledTasks;

namespace UberDeployer.Core.Deployment
{
  public class UpdateSchedulerTaskDeploymentStep : DeploymentStep
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
    private readonly Repetition _repetition;

    #region Constructor(s)

    // TODO IMM HI: can we update scheduler app without user name and password?
    public UpdateSchedulerTaskDeploymentStep(
      string machineName,
      string schedulerTaskName,
      string executablePath,
      string userName,
      string password,
      int scheduledHour,
      int scheduledMinute,
      int executionTimeLimitInMinutes,
      Repetition repetition,
      ITaskScheduler taskScheduler)
    {
      Guard.NotNull(taskScheduler, "taskScheduler");
      Guard.NotNullNorEmpty(machineName, "machineName");
      Guard.NotNullNorEmpty(schedulerTaskName, "schedulerTaskName");
      Guard.NotNullNorEmpty(executablePath, "executablePath");
      Guard.NotNullNorEmpty(userName, "userName");
      Guard.NotNullNorEmpty(password, "password");
      Guard.NotNull(repetition, "repetition");
      
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
      _repetition = repetition;
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
          Converter.CreateRepetitionSpecification(_repetition));

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
            "Update schedule of task named '{0}' on machine '{1}' to run daily at '{2}:{3}' with execution time limit of '{4}' minutes under user named '{5}'. Repetition is {6}.",
            _schedulerTaskName,
            _machineName,
            _scheduledHour.ToString().PadLeft(2, '0'),
            _scheduledMinute.ToString().PadLeft(2, '0'),
            _executionTimeLimitInMinutes,
            _userName,
            _repetition.Enabled
              ? string.Format("enabled (interval: '{0}'; duration: '{1}'; stop at duration end: '{2}')", _repetition.Interval, _repetition.Duration, _repetition.StopAtDurationEnd)
              : "disabled");
      }
    }

    #endregion
  }
}
