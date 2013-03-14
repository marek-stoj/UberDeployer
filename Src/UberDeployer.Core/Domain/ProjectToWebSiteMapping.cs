using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Core.Domain
{
  public class ProjectToWebSiteMapping
  {
    #region Constructor(s)

    public ProjectToWebSiteMapping(string projectName, string webSiteName)
    {
      Guard.NotNullNorEmpty(projectName, "projectName");
      Guard.NotNullNorEmpty(webSiteName, "webSiteName");

      ProjectName = projectName;
      WebSiteName = webSiteName;
    }

    #endregion

    #region Overrides of Object

    public override string ToString()
    {
      return string.Format("{0} -> {1}", ProjectName, WebSiteName);
    }

    #endregion

    #region Properties

    public string ProjectName { get; private set; }

    public string WebSiteName { get; private set; }

    #endregion
  }
}
