namespace UberDeployer.Core.Management.ScheduledTasks
{
  // TODO IMM HI: introduce two classes instead of one ScheduledTaskSpecification (one for update; one for new tasks)
  public interface ITaskScheduler
  {
    void ScheduleNewTask(string machineName, ScheduledTaskSpecification scheduledTaskSpecification, string userName, string password);

    void UpdateTaskSchedule(string machineName, ScheduledTaskSpecification scheduledTaskSpecification, string userName, string password);

    ScheduledTaskDetails GetScheduledTaskDetails(string machineName, string taskName);
    
    void EnableTask(string machineName, string taskName, bool enable);
  }
}
