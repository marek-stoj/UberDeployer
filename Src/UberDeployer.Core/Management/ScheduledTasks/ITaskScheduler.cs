namespace UberDeployer.Core.Management.ScheduledTasks
{
  // TODO IMM HI: introduce two classes instead of one ScheduledTaskSpecification (one for update; one for new tasks)
  public interface ITaskScheduler
  {
    void ScheduleNewTask(string machineName, ScheduledTaskSpecification scheduledTaskSpecification, string userName, string password);

    void UpdateTaskSchedule(string machineName, ScheduledTaskSpecification scheduledTaskSpecification, string userName, string password);

    bool IsTaskScheduled(string machineName, string taskName);
  }
}
