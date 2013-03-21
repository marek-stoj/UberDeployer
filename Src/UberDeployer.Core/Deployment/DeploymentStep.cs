using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  public abstract class DeploymentStep : DeploymentTaskBase
  {
    #region Ctor(s)

    // TODO IMM HI: xxx use something else instead of ProjectInfo?
    protected DeploymentStep(ProjectInfo projectInfo)
    {
      Guard.NotNull(projectInfo, "projectInfo");

      ProjectInfo = projectInfo;
    }

    #endregion

    #region Protected members

    protected override void DoPrepare()
    {
      // do nothing
    }

    protected override void DoExecute()
    {
      // do nothing
    }

    protected ProjectInfo ProjectInfo { get; private set; }

    #endregion
  }
}
