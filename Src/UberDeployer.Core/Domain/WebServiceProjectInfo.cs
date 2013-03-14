namespace UberDeployer.Core.Domain
{
  public class WebServiceProjectInfo : WebAppProjectInfo
  {
    public WebServiceProjectInfo(string name, string artifactsRepositoryName, string artifactsRepositoryDirName, bool artifactsAreNotEnvironmentSpecific, string webAppName, string webAppDirName)
      : base(name, artifactsRepositoryName, artifactsRepositoryDirName, artifactsAreNotEnvironmentSpecific, webAppName, webAppDirName)
    {
    }

    public override ProjectType Type
    {
      get { return ProjectType.WebService; }
    }
  }
}
