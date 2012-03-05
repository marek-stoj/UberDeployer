namespace UberDeployer.Core.Management.MsDeploy
{
  public interface IMsDeploy
  {
    void Run(string[] args, out string consoleOutput);
    void CreateIisAppManifestFile(string localWebAppPath, string outMsDeployManifestFilePath);
  }
}