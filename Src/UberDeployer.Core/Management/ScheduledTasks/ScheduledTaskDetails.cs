using System;
using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Core.Management.ScheduledTasks
{
  public class ScheduledTaskDetails
  {
    public ScheduledTaskDetails(string name, bool isEnabled, bool isRunning, DateTime lastRunTime, DateTime nextRunTime, string exeAbsolutePath, int scheduledHour, int scheduledMinute, int executionTimeLimitInMinutes, ScheduledTaskRepetition repetition)
    {
      Guard.NotNullNorEmpty(name, "name");
      Guard.NotNull(repetition, "repetition");

      Name = name;
      IsEnabled = isEnabled;
      IsRunning = isRunning;
      LastRunTime = lastRunTime;
      NextRunTime = nextRunTime;
      ExeAbsolutePath = exeAbsolutePath;
      ScheduledHour = scheduledHour;
      ScheduledMinute = scheduledMinute;
      ExecutionTimeLimitInMinutes = executionTimeLimitInMinutes;
      Repetition = repetition;
    }

    public string Name { get; private set; }

    public bool IsEnabled { get; private set; }

    public bool IsRunning { get; private set; }

    public DateTime LastRunTime { get; private set; }

    public DateTime NextRunTime { get; private set; }

    public string ExeAbsolutePath { get; private set; }

    public int ScheduledHour { get; private set; }

    public int ScheduledMinute { get; private set; }

    /// <summary>
    /// 0 - no limit.
    /// </summary>
    public int ExecutionTimeLimitInMinutes { get; private set; }

    public ScheduledTaskRepetition Repetition { get; private set; }
  }
}
