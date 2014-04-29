using System.Collections.Generic;

namespace UberDeployer.Core.Domain
{
  public class WebServiceProjectInfo : WebAppProjectInfo
  {
    public WebServiceProjectInfo(string name, string artifactsRepositoryName, IEnumerable<string> allowedEnvironmentNames, string artifactsRepositoryDirName, bool artifactsAreNotEnvironmentSpecific, string appPoolId, string webSiteName, string webAppDirName, string webAppName = null)
      : base(name, artifactsRepositoryName, allowedEnvironmentNames, artifactsRepositoryDirName, artifactsAreNotEnvironmentSpecific, appPoolId, webSiteName, webAppDirName, webAppName)
    {
    }

    public override ProjectType Type
    {
      get { return ProjectType.WebService; }
    }
  }
}
