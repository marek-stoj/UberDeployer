using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Management.ScheduledTasks;

namespace UberDeployer.Core.Deployment
{
  public class ToggleSchedulerAppEnabledStep : DeploymentStep
  {
    private readonly ITaskScheduler _taskScheduler;
    private readonly string _machineName;
    private readonly string _schedulerTaskName;
    private readonly bool _enabled;

    public ToggleSchedulerAppEnabledStep(ITaskScheduler taskScheduler, string machineName, string schedulerTaskName, bool enabled)
    {
      Guard.NotNull(taskScheduler, "taskScheduler");
      Guard.NotNullNorEmpty(machineName, "machineName");

      _taskScheduler = taskScheduler;
      _machineName = machineName;
      _schedulerTaskName = schedulerTaskName;
      _enabled = enabled;
    }

    protected override void DoExecute()
    {
      _taskScheduler.ToggleTaskEnabled(
        _machineName,
        _schedulerTaskName,
        _enabled);
    }

    public override string Description
    {
      get
      {
        string action = _enabled ? "Enable" : "Disable";

        return string.Format(
          "{0} task: {1} on machine: {2}.",
          action,
          _schedulerTaskName,
          _machineName);
      }
    }

    public bool Enabled
    {
      get { return _enabled; }
    }
  }
}
