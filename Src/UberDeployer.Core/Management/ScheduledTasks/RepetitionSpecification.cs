using System;

namespace UberDeployer.Core.Management.ScheduledTasks
{
  public class RepetitionSpecification
  {
    private RepetitionSpecification(bool enabled, TimeSpan interval, TimeSpan duration, bool stopAtDurationEnd)
    {
      Enabled = enabled;
      Interval = interval;
      Duration = duration;
      StopAtDurationEnd = stopAtDurationEnd;
    }

    public static RepetitionSpecification CreatedDisabled()
    {
      return new RepetitionSpecification(false, TimeSpan.Zero, TimeSpan.Zero, false);
    }

    public static RepetitionSpecification CreateEnabled(TimeSpan interval, TimeSpan duration, bool stopAtDurationEnd)
    {
      return new RepetitionSpecification(true, interval, duration, stopAtDurationEnd);
    }

    public bool Enabled { get; private set; }

    public TimeSpan Interval { get; private set; }

    public TimeSpan Duration { get; private set; }

    public bool StopAtDurationEnd { get; private set; }
  }
}
