using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Core.Domain
{
  public class WebAppProjectConfigurationOverride
  {
    public WebAppProjectConfigurationOverride(string projectName, string appPoolId = null, string webSiteName = null, string webAppDirName = null, string webAppName = null)
    {
      Guard.NotNullNorEmpty(projectName, "projectName");

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
