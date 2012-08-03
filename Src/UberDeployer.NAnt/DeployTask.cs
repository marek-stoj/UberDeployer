using System;
using NAnt.Core;
using NAnt.Core.Attributes;
using UberDeployer.CommonConfiguration;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Deployment.Pipeline;

namespace UberDeployer.NAnt
{
  // TODO IMM HI: what about 'startup useLegacyV2RuntimeActivationPolicy="true"' for deploying to a cluster? (we can't use app.config here)
  [TaskName("Deploy")]
  public class DeployTask : Task
  {
    static DeployTask()
    {
      Bootstraper.Bootstrap();
    }

    protected override void ExecuteTask()
    {
      try
      {
        IProjectInfoRepository projectInfoRepository = ObjectFactory.Instance.CreateProjectInfoRepository();

        ProjectInfo projectInfo = projectInfoRepository.GetByName(ProjectName);

        DeploymentTask deploymentTask = projectInfo.CreateDeploymentTask(
          ObjectFactory.Instance,
          ConfigurationName,
          BuildId,
          Environment);

        deploymentTask.DiagnosticMessagePosted +=
          (eventSender, tmpArgs) => Log(Level.Info, tmpArgs.Message);

        IDeploymentPipeline deploymentPipeline =
          ObjectFactory.Instance.CreateDeploymentPipeline();

        deploymentPipeline.StartDeployment(deploymentTask);
      }
      catch (Exception exc)
      {
        Log(Level.Error, "Error: " + exc);
      }
    }

    [TaskAttribute("projectName", Required = true)]
    [StringValidator(AllowEmpty = false)]
    public string ProjectName { get; set; }

    [TaskAttribute("configurationName", Required = true)]
    [StringValidator(AllowEmpty = false)]
    public string ConfigurationName { get; set; }

    [TaskAttribute("buildId", Required = true)]
    [StringValidator(AllowEmpty = false)]
    public string BuildId { get; set; }

    [TaskAttribute("environment", Required = true)]
    [StringValidator(AllowEmpty = false)]
    public string Environment { get; set; }
  }
}
