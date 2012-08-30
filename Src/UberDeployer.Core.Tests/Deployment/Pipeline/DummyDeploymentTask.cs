using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Tests.Deployment.Pipeline
{
  public class DummyDeploymentTask : DeploymentTask
  {
    public DummyDeploymentTask(IEnvironmentInfoRepository environmentInfoRepository, string targetEnvironmentName)
      : base(environmentInfoRepository, targetEnvironmentName)
    {
    }

    protected override void DoPrepare()
    {
      // do nothing
    }

    public override string ProjectName
    {
      get { return "DummyProject"; }
    }

    public override string ProjectConfigurationName
    {
      get { return "Trunk"; }
    }

    public override string ProjectConfigurationBuildId
    {
      get { return "1"; }
    }  }
}
