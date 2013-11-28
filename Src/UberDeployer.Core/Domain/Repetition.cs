using System;

namespace UberDeployer.Core.Domain
{
  public class Repetition
  {
    private Repetition(bool enabled, TimeSpan interval, TimeSpan duration, bool stopAtDurationEnd)
    {
      Enabled = enabled;
      Interval = interval;
      Duration = duration;
      StopAtDurationEnd = stopAtDurationEnd;
    }

    public static Repetition CreatedDisabled()
    {
      return new Repetition(false, TimeSpan.Zero, TimeSpan.Zero, false);
    }

    public static Repetition CreateEnabled(TimeSpan interval, TimeSpan duration, bool stopAtDurationEnd)
    {
      return new Repetition(true, interval, duration, stopAtDurationEnd);
    }

    public bool Enabled { get; private set; }

    public TimeSpan Interval { get; private set; }

    /// <summary>
    /// TimeSpan.Zero - indefinite duration.
    /// </summary>
    public TimeSpan Duration { get; private set; }

    public bool StopAtDurationEnd { get; private set; }
  }
}
