using System.Collections.Generic;

namespace UberDeployer.Agent.Proxy.Dto
{
  public class SchedulerAppProjectInfo : ProjectInfo
  {
    public string SchedulerAppDirName { get; set; }

    public string SchedulerAppExeName { get; set; }

    public List<SchedulerAppTask> SchedulerAppTasks { get; set; }
  }
}
