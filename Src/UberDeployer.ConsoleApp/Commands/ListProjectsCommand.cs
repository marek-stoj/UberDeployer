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

      string projectType = null;

      if (args.Length == 1)
      {
        projectType = args[0];
      }

      IProjectInfoRepository projectInfoRepository =
        ObjectFactory.Instance.CreateProjectInfoRepository();

      IEnumerable<IGrouping<string, ProjectInfo>> projectsByType =
        projectInfoRepository.GetAll()
          .GroupBy(pi => pi.Type)
          .OrderBy(infos => infos.Key);

      int count = projectsByType.Count();
      int index = 0;

      foreach (IGrouping<string, ProjectInfo> projectGrouping in projectsByType)
      {
        string currentProjecType = projectGrouping.Key;

        if (string.IsNullOrEmpty(projectType) || currentProjecType.ToUpper() == projectType.ToUpper())
        {
          OutputWriter.WriteLine("{0}:", currentProjecType);

          foreach (ProjectInfo projectInfo in projectGrouping)
          {
            OutputWriter.WriteLine("  {0}", projectInfo.Name);
          }
        }

        index++;

        if (string.IsNullOrEmpty(projectType) && index < count)
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
