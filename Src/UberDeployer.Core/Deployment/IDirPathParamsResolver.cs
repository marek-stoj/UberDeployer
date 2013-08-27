namespace UberDeployer.Core.Deployment
{
  public interface IDirPathParamsResolver
  {    
    string ResolveParams(string dirPath, string projectName);
  }
}