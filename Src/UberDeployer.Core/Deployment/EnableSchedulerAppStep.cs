using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.ScheduledTasks;

namespace UberDeployer.Core.Deployment
{
  public class EnableSchedulerAppStep : DeploymentStep
  {
    private readonly ITaskScheduler _taskScheduler;
    private readonly string _machineName;
    private readonly bool _enable;

    private string _taskName
    {
      get
      {
        var projectInfo = (SchedulerAppProjectInfo)DeploymentInfo.ProjectInfo;
        return projectInfo.SchedulerAppName;
      }
    }

    public EnableSchedulerAppStep(ITaskScheduler taskScheduler, string machineName, bool enable)
    {
      Guard.NotNull(taskScheduler, "taskScheduler");
      Guard.NotNullNorEmpty(machineName, "machineName");

      _taskScheduler = taskScheduler;
      _machineName = machineName;
      _enable = enable;
    }

    public override string Description
    {
      get
      {
        string action = _enable ? "Enable" : "Disable";

        return string.Format(
          "{0} task: {1} on machine: {2}.", 
          action,
          _taskName,
          _machineName);
      }
    }

    public bool Enable
    {
      get { return _enable; }
    }

    protected override void DoExecute()
    {
      _taskScheduler.EnableTask(_machineName, _taskName, _enable);
    }
  }
}
