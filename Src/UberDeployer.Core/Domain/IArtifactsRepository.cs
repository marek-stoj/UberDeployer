namespace UberDeployer.Core.Domain
{
  public interface IArtifactsRepository
  {
    void GetArtifacts(string projectConfigurationBuildId, string destinationFilePath);
  }
}
