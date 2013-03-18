using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.ScheduledTasks;

namespace UberDeployer.Core.Deployment
{
  public class ToggleSchedulerAppEnabledStep : DeploymentStep
  {
    private readonly ITaskScheduler _taskScheduler;
    private readonly string _machineName;
    private readonly bool _enabled;

    private string _taskName
    {
      get
      {
        var projectInfo = (SchedulerAppProjectInfo)DeploymentInfo.ProjectInfo;

        return projectInfo.SchedulerAppName;
      }
    }

    public ToggleSchedulerAppEnabledStep(ITaskScheduler taskScheduler, string machineName, bool enabled)
    {
      Guard.NotNull(taskScheduler, "taskScheduler");
      Guard.NotNullNorEmpty(machineName, "machineName");

      _taskScheduler = taskScheduler;
      _machineName = machineName;
      _enabled = enabled;
    }

    public override string Description
    {
      get
      {
        string action = _enabled ? "Enable" : "Disable";

        return string.Format(
          "{0} task: {1} on machine: {2}.", 
          action,
          _taskName,
          _machineName);
      }
    }

    public bool Enabled
    {
      get { return _enabled; }
    }

    protected override void DoExecute()
    {
      _taskScheduler.ToggleTaskEnabled(_machineName, _taskName, _enabled);
    }
  }
}
