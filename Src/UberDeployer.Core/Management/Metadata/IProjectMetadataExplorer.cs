namespace UberDeployer.Core.Management.Metadata
{
  public interface IProjectMetadataExplorer
  {
    ProjectMetadata GetProjectMetadata(string projectName, string environmentName);
  }
}
