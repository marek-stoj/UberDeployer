using System.Collections.Generic;
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

    public string[] GetDirectories(string path)
    {
      return Directory.GetDirectories(path);
    }

    public void Delete(string path, bool recursive)
    {
      Directory.Delete(path, recursive);
    }

    public IEnumerable<string> GetFiles(string path, string searchPattern, SearchOption searchOption)
    {
      return Directory.GetFiles(path, searchPattern, searchOption);
    }

    public IEnumerable<string> GetFiles(string path)
    {
      return Directory.GetFiles(path);
    }

    public void CreateDirectory(string path)
    {
      Directory.CreateDirectory(path);
    }
  }
}
