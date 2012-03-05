using System.IO;
using Microsoft.Win32.TaskScheduler;
using System;
using Action = Microsoft.Win32.TaskScheduler.Action;

namespace UberDeployer.Core.Management.ScheduledTasks
{
  public class TaskScheduler : ITaskScheduler
  {
    private const string _TaskRegistrationInfoSource = "UberDeployer";

    #region ITaskScheduler members

    public void ScheduleNewTask(string machineName, ScheduledTaskSpecification scheduledTaskSpecification, string userName, string password)
    {
      if (string.IsNullOrEmpty(machineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "machineName");
      }

      if (scheduledTaskSpecification == null)
      {
        throw new ArgumentNullException("scheduledTaskSpecification");
      }

      if (string.IsNullOrEmpty(userName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "userName");
      }

      if (string.IsNullOrEmpty(password))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "password");
      }

      using (var taskService = CreateTaskService(machineName))
      {
        Task task = taskService.FindTask(scheduledTaskSpecification.Name, false);

        if (task != null)
        {
          task.Dispose();

          throw new InvalidOperationException(string.Format("Couldn't schedule new task because a task with the same name ('{0}') has already been scheduled.", scheduledTaskSpecification.Name));
        }

        Action taskAction =
          new ExecAction(scheduledTaskSpecification.ExeAbsolutePath)
            {
              WorkingDirectory = Path.GetDirectoryName(scheduledTaskSpecification.ExeAbsolutePath),
            };

        DailyTrigger taskTrigger = CreateDailyTrigger(scheduledTaskSpecification);
        TaskDefinition taskDefinition = null;
        Task registeredTask = null;

        try
        {
          taskDefinition = taskService.NewTask();

          taskDefinition.Settings.AllowDemandStart = true;
          taskDefinition.Settings.AllowHardTerminate = true;
          taskDefinition.Settings.DisallowStartIfOnBatteries = false;
          taskDefinition.Settings.DisallowStartOnRemoteAppSession = false;
          taskDefinition.Settings.RunOnlyIfIdle = false;
          taskDefinition.Settings.RunOnlyIfNetworkAvailable = false;
          taskDefinition.Settings.StartWhenAvailable = true;
          taskDefinition.Settings.StopIfGoingOnBatteries = false;

          taskDefinition.RegistrationInfo.Source = _TaskRegistrationInfoSource;

          taskDefinition.Actions.Add(taskAction);
          taskDefinition.Triggers.Add(taskTrigger);

          registeredTask =
            taskService.RootFolder.RegisterTaskDefinition(
              scheduledTaskSpecification.Name,
              taskDefinition,
              TaskCreation.Create,
              userName,
              password,
              TaskLogonType.Password);
        }
        finally
        {
          if (taskDefinition != null)
          {
            taskDefinition.Dispose();
          }

          if (registeredTask != null)
          {
            registeredTask.Dispose();
          }
        }
      }
    }

    // TODO IMM HI: can we do this without user name and password?
    public void UpdateTaskSchedule(string machineName, ScheduledTaskSpecification scheduledTaskSpecification, string userName, string password)
    {
      if (string.IsNullOrEmpty(machineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "machineName");
      }

      if (scheduledTaskSpecification == null)
      {
        throw new ArgumentNullException("scheduledTaskSpecification");
      }

      string taskName = scheduledTaskSpecification.Name;

      using (var taskService = CreateTaskService(machineName))
      {
        Task task = null;
        Task registeredTask = null;
        TaskDefinition taskDefinition = null;

        try
        {
          task = taskService.FindTask(taskName);

          if (task == null)
          {
            throw new InvalidOperationException(string.Format("Task named '{0}' doesn't exist on the target machine ('{1}').", taskName, machineName));
          }

          taskDefinition = task.Definition;

          taskDefinition.Triggers.Clear();

          Trigger dailyTrigger = CreateDailyTrigger(scheduledTaskSpecification);

          taskDefinition.Triggers.Add(dailyTrigger);

          registeredTask =
            taskService.RootFolder.RegisterTaskDefinition(
              scheduledTaskSpecification.Name,
              taskDefinition,
              TaskCreation.Update,
              userName,
              password,
              TaskLogonType.Password);
        }
        finally
        {
          if (taskDefinition != null)
          {
            taskDefinition.Dispose();
          }

          if (task != null)
          {
            task.Dispose();
          }

          if (registeredTask != null)
          {
            registeredTask.Dispose();
          }
        }
      }
    }

    public bool IsTaskScheduled(string machineName, string taskName)
    {
      if (string.IsNullOrEmpty(machineName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "machineName");
      }

      if (string.IsNullOrEmpty(taskName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "taskName");
      }

      using (var taskService = CreateTaskService(machineName))
      {
        Task task = taskService.FindTask(taskName, false);

        return task != null;
      }
    }

    #endregion

    #region Private helper methods

    private static TaskService CreateTaskService(string machineName)
    {
      return new TaskService(machineName);
    }

    private static DailyTrigger CreateDailyTrigger(ScheduledTaskSpecification scheduledTaskSpecification)
    {
      DateTime now = DateTime.Now;

      var dailyTrigger =
        new DailyTrigger
          {
            DaysInterval = 1,
            StartBoundary =
              new DateTime(
              now.Year,
              now.Month,
              now.Day,
              scheduledTaskSpecification.ScheduledHour + 1, // TODO IMM HI: remove + 1
              scheduledTaskSpecification.ScheduledMinute,
              0),
          };

      if (scheduledTaskSpecification.ExecutionTimeLimitInMinutes > 0)
      {
        dailyTrigger.ExecutionTimeLimit =
          TimeSpan.FromMinutes(scheduledTaskSpecification.ExecutionTimeLimitInMinutes);
      }

      return dailyTrigger;
    }

    #endregion
  }
}
