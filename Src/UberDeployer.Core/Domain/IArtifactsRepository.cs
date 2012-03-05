namespace UberDeployer.Core.Domain
{
  public interface IArtifactsRepository
  {
    void GetArtifacts(string projectName, string projectConfigurationName, string projectConfigurationBuildId, string destinationFilePath);
  }
}
