using System;
using System.Linq;
using UberDeployer.CommonConfiguration;
using UberDeployer.ConsoleCommander;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Deployment.Pipeline;
using UberDeployer.Core.Domain;
using UberDeployer.Core.TeamCity;
using UberDeployer.Core.TeamCity.Models;

namespace UberDeployer.ConsoleApp.Commands
{
  public class DeployLatestCommand : DeployCommand
  {
    public DeployLatestCommand(CommandDispatcher commandDispatcher)
      : base(commandDispatcher)
    {
    }

    public override string CommandName
    {
      get { return "deploy-latest"; }
    }

    public override void Run(string[] args)
    {
      if (args.Length != 3)
      {
        DisplayCommandUsage();
        return;
      }

      IProjectInfoRepository projectInfoRepository =
        ObjectFactory.Instance.CreateProjectInfoRepository();

      string projectName = args[0];
      string projectConfigurationName = args[1];
      string targetEnvironmentName = args[2];

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
        projectDetails.ConfigurationsList
          .Configurations
          .SingleOrDefault(pc => pc.Name == projectConfigurationName);

      if (projectConfiguration == null)
      {
        OutputWriter.WriteLine(
          "Project configuration named '{0}' doesn't exist for project '{1}'.",
          projectConfigurationName,
          projectName);

        return;
      }

      ProjectConfigurationDetails projectConfigurationDetails =
        teamCityClient.GetProjectConfigurationDetails(projectConfiguration);

      ProjectConfigurationBuild projectConfigurationBuild =
        teamCityClient.GetProjectConfigurationBuilds(projectConfigurationDetails, 0, 1)
          .Builds
          .FirstOrDefault();

      if (projectConfigurationBuild == null)
      {
        throw new InvalidOperationException(
          string.Format(
            "Project configuration '{0}' of project '{1}' doesn't have any builds yet.",
            projectConfigurationName,
            projectName));
      }

      if (projectConfigurationBuild.Status != BuildStatus.Success)
      {
        throw new InvalidOperationException(
          string.Format(
            "Couldn't deploy latest build of project configuration '{0}' of project '{1}' because it was not successfull.",
            projectConfigurationName,
            projectName));
      }

      string projectConfigurationBuildId = projectConfigurationBuild.Id;

      try
      {
        DeploymentTask deploymentTask =
          projectInfo.CreateDeploymentTask(
            ObjectFactory.Instance,
            projectConfigurationName,
            projectConfigurationBuildId,
            targetEnvironmentName);

        deploymentTask.DiagnosticMessagePosted +=
          (eventSender, tmpArgs) => LogMessage(tmpArgs.Message);

        IDeploymentPipeline deploymentPipeline =
          ObjectFactory.Instance.CreateDeploymentPipeline();

        deploymentPipeline.StartDeployment(deploymentTask);
      }
      catch (Exception exc)
      {
        LogMessage("Error: " + exc);
      }
    }

    public override void DisplayCommandUsage()
    {
      OutputWriter.WriteLine("Usage: {0} project projectConfiguration targetEnvironment", CommandName);
    }

    private void LogMessage(string message)
    {
      OutputWriter.WriteLine(message);
    }
  }
}
