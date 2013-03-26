using System.IO;

namespace UberDeployer.Common.IO
{
  public interface IDirectoryAdapter
  {
    bool Exists(string path);
    
    string[] GetDirectories(string path, string searchPattern, SearchOption searchOption);
  }
}
