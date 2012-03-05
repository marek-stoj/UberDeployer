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

    public virtual DateTime DateRequested { get; set; }

    public virtual string RequesterIdentity { get; set; }

    public virtual string ProjectName { get; set; }

    public virtual string TargetEnvironmentName { get; set; }

    public virtual bool FinishedSuccessfully { get; set; }
  }
}
