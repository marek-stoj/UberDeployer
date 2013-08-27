namespace UberDeployer.Core.Deployment
{
  public interface IDirPathParamsResolver
  {
    string ResolveProjectName(string dirPath, string projectName);

    string ResolveCurrentDate(string dirPath, string dateFormat);

    string ResolveOrderNumber(string packageDirPath);
  }
}