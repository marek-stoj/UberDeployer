using System.Collections.Generic;

namespace UberDeployer.Core.Domain
{
  public class WebServiceProjectInfo : WebAppProjectInfo
  {
    public WebServiceProjectInfo(string name, string artifactsRepositoryName, IEnumerable<string> allowedEnvironmentNames, string artifactsRepositoryDirName, bool artifactsAreNotEnvironmentSpecific)
      : base(name, artifactsRepositoryName, allowedEnvironmentNames, artifactsRepositoryDirName, artifactsAreNotEnvironmentSpecific)
    {
    }

    public override ProjectType Type
    {
      get { return ProjectType.WebService; }
    }
  }
}
