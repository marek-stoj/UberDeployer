namespace UberDeployer.Core.Domain
{
  public class WebServiceProjectInfo : WebAppProjectInfo
  {
    public WebServiceProjectInfo(string name, string artifactsRepositoryName, string artifactsRepositoryDirName, bool artifactsAreNotEnvironmentSpecific)
      : base(name, artifactsRepositoryName, artifactsRepositoryDirName, artifactsAreNotEnvironmentSpecific)
    {
    }

    public override ProjectType Type
    {
      get { return ProjectType.WebService; }
    }
  }
}
