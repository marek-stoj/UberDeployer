namespace UberDeployer.Core.Domain
{
  public class WebServiceProjectInfo : WebAppProjectInfo
  {
    public WebServiceProjectInfo(string name, string artifactsRepositoryName, string artifactsRepositoryDirName, bool artifactsAreNotEnvironmentSpecific, string iisSiteName, string webAppName, string webAppDirName, IisAppPoolInfo appPoolInfo)
      : base(name, artifactsRepositoryName, artifactsRepositoryDirName, artifactsAreNotEnvironmentSpecific, iisSiteName, webAppName, webAppDirName, appPoolInfo)
    {
    }
  }
}
