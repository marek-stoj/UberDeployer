namespace UberDeployer.Agent.Proxy.Dto
{
  public class SchedulerAppTask
  {
    public string Name { get; set; }

    public string ExecutableName { get; set; }

    public string UserId { get; set; }

    public int ScheduledHour { get; set; }

    public int ScheduledMinute { get; set; }

    /// <summary>
    /// 0 - no limit.
    /// </summary>
    public int ExecutionTimeLimitInMinutes { get; set; }

    public Repetition Repetition { get; set; }
  }
}
