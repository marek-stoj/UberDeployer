using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using log4net;

namespace UberDeployer.Common.IO
{
  public class DirectoryAdapter : IDirectoryAdapter
  {
    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    
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
      _log.DebugFormat("Deleting path {0}, recursive {1}", path, recursive);

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

    public void CopyAll(string srcPath, string dstPath)
    {
      foreach (string filePath in Directory.GetFiles(srcPath, "*.*", SearchOption.TopDirectoryOnly))
      {
        string fileName = Path.GetFileName(filePath);

        if (string.IsNullOrEmpty(fileName))
        {
          throw new Exception(string.Format("Unexpected lack of a file name in a path. File path: '{0}'.", filePath));
        }

        string dstFilePath = Path.Combine(dstPath, fileName);

        File.Copy(filePath, dstFilePath, true);
      }

      foreach (string dirPath in Directory.GetDirectories(srcPath, "*", SearchOption.TopDirectoryOnly))
      {
        string dirName = Path.GetFileName(dirPath);

        if (string.IsNullOrEmpty(dirName))
        {
          throw new Exception(string.Format("Unexpected lack of a directory name in a path. Directory path: '{0}'.", dirPath));
        }

        string dstSubDirPath = Path.Combine(dstPath, dirName);

        if (!Directory.Exists(dstSubDirPath))
        {
          Directory.CreateDirectory(dstSubDirPath);
        }

        CopyAll(dirPath, dstSubDirPath);
      }
    }
  }
}
