using System;

namespace UberDeployer.Core.Management.ScheduledTasks
{
  public class ScheduledTaskRepetition
  {
    public ScheduledTaskRepetition(TimeSpan interval, TimeSpan duration, bool stopAtDurationEnd)
    {
      Interval = interval;
      Duration = duration;
      StopAtDurationEnd = stopAtDurationEnd;
    }

    public TimeSpan Interval { get; set; }

    public TimeSpan Duration { get; set; }

    public bool StopAtDurationEnd { get; set; }
  }
}
