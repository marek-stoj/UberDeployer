using UberDeployer.CommonConfiguration;
using UberDeployer.ConsoleCommander;
using UberDeployer.Core.Domain;
using UberDeployer.Core.TeamCity;
using UberDeployer.Core.TeamCity.Models;

namespace UberDeployer.ConsoleApp.Commands
{
  public class ListProjectConfigurationsCommand : ConsoleCommand
  {
    public ListProjectConfigurationsCommand(CommandDispatcher commandDispatcher)
      : base(commandDispatcher)
    {
    }

    public override int Run(string[] args)
    {
      if (args.Length != 1)
      {
        DisplayCommandUsage();
        
        return 1;
      }

      string projectName = args[0];

      IProjectInfoRepository projectInfoRepository =
        ObjectFactory.Instance.CreateProjectInfoRepository();

      ProjectInfo projectInfo = projectInfoRepository.FindByName(projectName);

      if (projectInfo == null)
      {
        OutputWriter.WriteLine("Project named '{0}' doesn't exist.", projectName);
        
        return 1;
      }

      ITeamCityClient teamCityClient =
        ObjectFactory.Instance.CreateTeamCityClient();

      Project project = teamCityClient.GetProjectByName(projectInfo.ArtifactsRepositoryName);
      ProjectDetails projectDetails = teamCityClient.GetProjectDetails(project);

      foreach (ProjectConfiguration projectConfiguration in projectDetails.ConfigurationsList.Configurations)
      {
        OutputWriter.WriteLine(projectConfiguration.Name);
      }

      return 0;
    }

    public override void DisplayCommandUsage()
    {
      OutputWriter.WriteLine("Usage: {0} projectName", CommandName);
    }

    public override string CommandName
    {
      get { return "lst-confs"; }
    }
  }
}
