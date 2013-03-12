namespace UberDeployer.Core.Management.MsDeploy
{
  public interface IMsDeploy
  {
    void Run(string[] args, out string stdout);

    void CreateIisAppManifestFile(string localWebAppPath, string outMsDeployManifestFilePath);
  }
}
