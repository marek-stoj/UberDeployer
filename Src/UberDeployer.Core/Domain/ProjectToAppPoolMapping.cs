using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Core.Domain
{
  public class ProjectToAppPoolMapping
  {
    #region Constructor(s)

    public ProjectToAppPoolMapping(string projectName, string appPoolId)
    {
      Guard.NotNullNorEmpty(projectName, "projectName");
      Guard.NotNullNorEmpty(appPoolId, "appPoolId");

      ProjectName = projectName;
      AppPoolId = appPoolId;
    }

    #endregion

    #region Overrides of Object

    public override string ToString()
    {
      return string.Format("{0} -> {1}", ProjectName, AppPoolId);
    }

    #endregion

    #region Properties

    public string ProjectName { get; private set; }

    public string AppPoolId { get; private set; }

    #endregion
  }
}
