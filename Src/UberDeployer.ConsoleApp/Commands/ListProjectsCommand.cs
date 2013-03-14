using System;
using System.Collections.Generic;
using System.Linq;
using UberDeployer.CommonConfiguration;
using UberDeployer.ConsoleCommander;
using UberDeployer.Core.Domain;

namespace UberDeployer.ConsoleApp.Commands
{
  public class ListProjectsCommand : ConsoleCommand
  {
    public ListProjectsCommand(CommandDispatcher commandDispatcher)
      : base(commandDispatcher)
    {
    }

    public override int Run(string[] args)
    {
      if (args.Length > 1)
      {
        DisplayCommandUsage();
        
        return 1;
      }

      ProjectType? projectType = null;

      if (args.Length == 1)
      {
        projectType = (ProjectType)Enum.Parse(typeof(ProjectType), args[0], true);
      }

      IProjectInfoRepository projectInfoRepository =
        ObjectFactory.Instance.CreateProjectInfoRepository();

      IEnumerable<IGrouping<ProjectType, ProjectInfo>> projectsByType =
        projectInfoRepository.GetAll()
          .GroupBy(pi => pi.Type)
          .OrderBy(infos => infos.Key);

      int count = projectsByType.Count();
      int index = 0;

      foreach (IGrouping<ProjectType, ProjectInfo> projectGrouping in projectsByType)
      {
        ProjectType currentProjecType = projectGrouping.Key;

        if (!projectType.HasValue || currentProjecType == projectType.Value)
        {
          OutputWriter.WriteLine("{0}:", currentProjecType);

          foreach (ProjectInfo projectInfo in projectGrouping)
          {
            OutputWriter.WriteLine("  {0}", projectInfo.Name);
          }
        }

        index++;

        if (!projectType.HasValue && index < count)
        {
          OutputWriter.WriteLine();
        }
      }

      return 0;
    }

    public override void DisplayCommandUsage()
    {
      OutputWriter.WriteLine("Usage: {0} [projectType]", CommandName);
      OutputWriter.WriteLine("  projectType\tis one of: NTService, WebApp, SchedulerApp or TerminalApp");
    }

    public override string CommandName
    {
      get { return "lst-projs"; }
    }
  }
}
