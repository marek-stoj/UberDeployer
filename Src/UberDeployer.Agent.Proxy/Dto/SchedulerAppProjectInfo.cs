namespace UberDeployer.Agent.Proxy.Dto
{
  public class SchedulerAppProjectInfo : ProjectInfo
  {
    public string SchedulerAppName { get; set; }

    public string SchedulerAppDirName { get; set; }

    public string SchedulerAppExeName { get; set; }

    public string SchedulerAppUserId { get; set; }

    public int ScheduledHour { get; set; }
    
    public int ScheduledMinute { get; set; }

    public int ExecutionTimeLimitInMinutes { get; set; }
  }
}
