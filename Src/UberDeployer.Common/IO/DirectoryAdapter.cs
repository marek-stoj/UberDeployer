namespace UberDeployer.Common.IO
{
  public class DirectoryAdapter : IDirectoryAdapter
  {
    public bool Exists(string path)
    {
      return System.IO.Directory.Exists(path);
    }
  }
}
