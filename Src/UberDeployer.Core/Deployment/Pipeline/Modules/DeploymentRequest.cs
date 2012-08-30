using System;

namespace UberDeployer.Core.Deployment.Pipeline.Modules
{
  public class DeploymentRequest
  {
    public DeploymentRequest()
    {
      Id = -1;
    }

    public virtual int Id { get; private set; }

    public virtual DateTime DateStarted { get; set; }
    
    public virtual DateTime DateFinished { get; set; }

    public virtual string RequesterIdentity { get; set; }

    public virtual string ProjectName { get; set; }

    public virtual string ProjectConfigurationName { get; set; }

    public virtual string ProjectConfigurationBuildId { get; set; }

    public virtual string TargetEnvironmentName { get; set; }

    public virtual bool FinishedSuccessfully { get; set; }
  }
}
