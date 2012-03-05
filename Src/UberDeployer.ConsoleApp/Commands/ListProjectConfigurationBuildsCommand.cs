using System.Linq;
using UberDeployer.CommonConfiguration;
using UberDeployer.ConsoleCommander;
using UberDeployer.Core;
using UberDeployer.Core.Domain;
using UberDeployer.Core.TeamCity;
using UberDeployer.Core.TeamCity.Models;

namespace UberDeployer.ConsoleApp.Commands
{
  public class ListProjectConfigurationBuildsCommand : ConsoleCommand
  {
    private const int _MaxProjectConfigurationBuildsCount = 10;

    public ListProjectConfigurationBuildsCommand(CommandDispatcher commandDispatcher)
      : base(commandDispatcher)
    {
    }

    public override void Run(string[] args)
    {
      if (args.Length != 2)
      {
        DisplayCommandUsage();
        return;
      }

      string projectName = args[0];
      string projectConfigurationName = args[1];

      IProjectInfoRepository projectInfoRepository =
        ObjectFactory.Instance.CreateProjectInfoRepository();

      ProjectInfo projectInfo = projectInfoRepository.GetByName(projectName);

      if (projectInfo == null)
      {
        OutputWriter.WriteLine("Project named '{0}' doesn't exist.", projectName);
        return;
      }

      ITeamCityClient teamCityClient =
        ObjectFactory.Instance.CreateTeamCityClient();

      Project project = teamCityClient.GetProjectByName(projectInfo.ArtifactsRepositoryName);
      ProjectDetails projectDetails = teamCityClient.GetProjectDetails(project);

      ProjectConfiguration projectConfiguration =
        projectDetails.ConfigurationsList.Configurations
          .Where(pc => pc.Name == projectConfigurationName)
          .SingleOrDefault();

      if (projectConfiguration == null)
      {
        OutputWriter.WriteLine("Project configuration named '{0}' doesn't exist for project '{1}'.", projectConfigurationName, projectName);
        return;
      }

      ProjectConfigurationDetails projectConfigurationDetails =
        teamCityClient.GetProjectConfigurationDetails(projectConfiguration);

      ProjectConfigurationBuildsList projectConfigurationBuildsList =
        teamCityClient.GetProjectConfigurationBuilds(projectConfigurationDetails, 0, _MaxProjectConfigurationBuildsCount);

      foreach (ProjectConfigurationBuild projectConfigurationBuild in projectConfigurationBuildsList.Builds)
      {
        OutputWriter.WriteLine("{0}\t{1}", projectConfigurationBuild.Id, projectConfigurationBuild.Status);
      }

      return;
    }

    public override string CommandName
    {
      get { return "lst-builds"; }
    }

    public override void DisplayCommandUsage()
    {
      OutputWriter.WriteLine("Usage: {0} projectName projectConfigurationName", CommandName);
    }
  }
}
