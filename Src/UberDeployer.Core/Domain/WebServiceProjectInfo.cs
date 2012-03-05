namespace UberDeployer.Core.Domain
{
  public class WebServiceProjectInfo : WebAppProjectInfo
  {
    public WebServiceProjectInfo(string name, string artifactsRepositoryName, string artifactsRepositoryDirName, string iisSiteName, string webAppName, string webAppDirName, IisAppPoolInfo appPoolInfo)
      : base(name, artifactsRepositoryName, artifactsRepositoryDirName, iisSiteName, webAppName, webAppDirName, appPoolInfo)
    {
    }
  }
}
