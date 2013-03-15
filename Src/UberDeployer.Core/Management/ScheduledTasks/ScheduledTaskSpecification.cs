using System;
using System.IO;

namespace UberDeployer.Core.Management.ScheduledTasks
{
  public class ScheduledTaskSpecification
  {
    #region Constructor(s)

    public  ScheduledTaskSpecification(string name, string exeAbsolutePath, int scheduledHour, int scheduledMinute, int executionTimeLimitInMinutes)
    {
      if (string.IsNullOrEmpty(name))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "name");
      }

      if (string.IsNullOrEmpty(exeAbsolutePath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "exeAbsolutePath");
      }

      if (!Path.IsPathRooted(exeAbsolutePath))
      {
        throw new ArgumentException(string.Format("Executable path ('{0}') is not an absolute path.", exeAbsolutePath), "exeAbsolutePath");
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

      Name = name;
      ExeAbsolutePath = exeAbsolutePath;
      ScheduledHour = scheduledHour;
      ScheduledMinute = scheduledMinute;
      ExecutionTimeLimitInMinutes = executionTimeLimitInMinutes;
    }

    #endregion

    #region Properties

    public string Name { get; private set; }

    public string ExeAbsolutePath { get; private set; }

    public int ScheduledHour { get; private set; }

    public int ScheduledMinute { get; private set; }

    /// <summary>
    /// 0 - no limit.
    /// </summary>
    public int ExecutionTimeLimitInMinutes { get; set; }

    #endregion
  }
}
