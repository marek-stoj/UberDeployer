using System.IO;

namespace UberDeployer.Common.IO
{
  public class DirectoryAdapter : IDirectoryAdapter
  {
    public bool Exists(string path)
    {
      return Directory.Exists(path);
    }

    public string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
    {
      return Directory.GetDirectories(path, searchPattern, searchOption);
    }
  }
}
