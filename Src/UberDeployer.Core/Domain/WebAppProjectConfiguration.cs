using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Core.Domain
{
  public class WebAppProjectConfiguration
  {
    #region Constructor(s)

    public WebAppProjectConfiguration(string projectName, string appPoolId, string webSiteName, string webAppName = null)
    {
      Guard.NotNullNorEmpty(appPoolId, "appPoolId");
      Guard.NotNullNorEmpty(projectName, "projectName");
      Guard.NotNullNorEmpty(webSiteName, "webSiteName");

      ProjectName = projectName;
      AppPoolId = AppPoolId;
      WebSiteName = webSiteName;
      WebAppName = WebAppName;
    }

    #endregion

    #region Properties

    public string ProjectName { get; private set; }

    public string AppPoolId { get; private set; }

    public string WebSiteName { get; private set; }
    
    public string WebAppName { get; private set; }
    
    #endregion
  }
}
