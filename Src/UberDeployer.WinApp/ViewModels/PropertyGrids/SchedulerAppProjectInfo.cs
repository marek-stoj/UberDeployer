using System.ComponentModel;

namespace UberDeployer.WinApp.ViewModels.PropertyGrids
{
  public class SchedulerAppProjectInfoInPropertyGridViewModel : ProjectInfoInPropertyGridViewModel
  {
    [Category("Specific")]
    [ReadOnly(true)]
    public string SchedulerAppName { get; set; }

    [Category("Specific")]
    [ReadOnly(true)]
    public string SchedulerAppDirName { get; set; }

    [Category("Specific")]
    [ReadOnly(true)]
    public string SchedulerAppExeName { get; set; }

    [Category("Specific")]
    [ReadOnly(true)]
    public string SchedulerAppUserId { get; set; }

    [Category("Specific")]
    [ReadOnly(true)]
    public int ScheduledHour { get; set; }

    [Category("Specific")]
    [ReadOnly(true)]
    public int ScheduledMinute { get; set; }

    [Category("Specific")]
    [ReadOnly(true)]
    public int ExecutionTimeLimitInMinutes { get; set; }
  }
}
