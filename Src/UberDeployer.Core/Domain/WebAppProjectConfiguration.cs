using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Core.Domain
{
  public class WebAppProjectConfiguration
  {
    public WebAppProjectConfiguration(string projectName, string appPoolId, string webSiteName, string webAppDirName, string webAppName = null)
    {
      Guard.NotNullNorEmpty(projectName, "projectName");
      Guard.NotNullNorEmpty(appPoolId, "appPoolId");
      Guard.NotNullNorEmpty(webSiteName, "webSiteName");
      Guard.NotNullNorEmpty(webAppDirName, "webAppDirName");

      ProjectName = projectName;
      AppPoolId = appPoolId;
      WebSiteName = webSiteName;
      WebAppDirName = webAppDirName;
      WebAppName = webAppName;
    }

    public string ProjectName { get; private set; }

    public string AppPoolId { get; private set; }

    public string WebSiteName { get; private set; }

    public string WebAppDirName { get; private set; }
    
    public string WebAppName { get; private set; }
  }
}
