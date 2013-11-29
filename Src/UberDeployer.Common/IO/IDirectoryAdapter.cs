using System.Collections.Generic;
using System.IO;

namespace UberDeployer.Common.IO
{
  public interface IDirectoryAdapter
  {
    bool Exists(string path);
    
    string[] GetDirectories(string path, string searchPattern, SearchOption searchOption);

    string[] GetDirectories(string path);
    
    void Delete(string path, bool recursive);

    IEnumerable<string> GetFiles(string path, string searchPattern, SearchOption searchOption);

    IEnumerable<string> GetFiles(string path);
   
    void CreateDirectory(string path);
  }
}
