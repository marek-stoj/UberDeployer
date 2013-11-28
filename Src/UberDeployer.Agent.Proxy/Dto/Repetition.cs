using System;

namespace UberDeployer.Agent.Proxy.Dto
{
  public class Repetition
  {
    public bool Enabled { get; set; }

    public TimeSpan Interval { get; set; }

    /// <summary>
    /// TimeSpan.Zero - indefinite duration.
    /// </summary>
    public TimeSpan Duration { get; set; }

    public bool StopAtDurationEnd { get; set; }
  }
}
