namespace ProjectDepsVisualizer.Core
{
  public interface ISvnClient
  {
    string GetFileContents(string relativeDirectiry);

    string GetFileContentsByExt(string buildFilePath, string p);

    string[] GetRepositoryNames();
  }
}
