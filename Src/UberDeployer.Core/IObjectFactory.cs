using UberDeployer.Core.Configuration;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Deployment.Pipeline;
using UberDeployer.Core.Deployment.Pipeline.Modules;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Iis;
using UberDeployer.Core.Management.MsDeploy;
using UberDeployer.Core.Management.NtServices;
using UberDeployer.Core.Management.ScheduledTasks;
using UberDeployer.Core.TeamCity;

namespace UberDeployer.Core
{
  // TODO IMM HI: rethink
  public interface IObjectFactory
  {
    IApplicationConfiguration CreateApplicationConfiguration();

    IProjectInfoRepository CreateProjectInfoRepository();

    IEnvironmentInfoRepository CreateEnvironmentInfoRepository();

    IArtifactsRepository CreateArtifactsRepository();

    IDeploymentRequestRepository CreateDeploymentRequestRepository();

    ITeamCityClient CreateTeamCityClient();

    INtServiceManager CreateNtServiceManager();

    IIisManager CreateIIisManager();
    
    ITaskScheduler CreateTaskScheduler();

    IMsDeploy CreateIMsDeploy();
    
    IDeploymentPipeline CreateDeploymentPipeline();

    IPasswordCollector CreatePasswordCollector();
  }
}
