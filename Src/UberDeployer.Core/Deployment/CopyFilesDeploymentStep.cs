using System;
using System.IO;
using System.Linq;

namespace UberDeployer.Core.Deployment
{
  public class CopyFilesDeploymentStep : DeploymentStep
  {
    private readonly string _srcDirPath;
    private readonly string _dstDirPath;

    #region Constructor(s)

    public CopyFilesDeploymentStep(string srcDirPath, string dstDirPath)
    {
      if (string.IsNullOrEmpty(dstDirPath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "dstDirPath");
      }

      if (string.IsNullOrEmpty(srcDirPath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "srcDirPath");
      }

      _srcDirPath = srcDirPath;
      _dstDirPath = dstDirPath;
    }

    #endregion

    #region Overrides of DeploymentStep

    protected override void DoExecute()
    {
      if (!Directory.Exists(_srcDirPath))
      {
        throw new DeploymentTaskException(string.Format("Source directory doesn't exist: '{0}'.", _srcDirPath));
      }

      if (Directory.Exists(_dstDirPath))
      {
        Directory.GetDirectories(_dstDirPath)
          .ToList()
          .ForEach(dirPath => Directory.Delete(dirPath, true));

        Directory.GetFiles(_dstDirPath)
          .ToList()
          .ForEach(File.Delete);
      }
      else
      {
        Directory.CreateDirectory(_dstDirPath);
      }

      CopyAll(_srcDirPath, _dstDirPath);
    }

    public override string Description
    {
      get { return string.Format("Copy files to '{0}' from '{1}'.", _dstDirPath, _srcDirPath); }
    }

    #endregion

    #region Private helper methods

    private void CopyAll(string srcDirPath, string dstDirPath)
    {
      foreach (string filePath in Directory.GetFiles(srcDirPath, "*.*", SearchOption.TopDirectoryOnly))
      {
        string fileName = Path.GetFileName(filePath);

        if (string.IsNullOrEmpty(fileName))
        {
          throw new InternalException(string.Format("Unexpected lack of a file name in a path. File path: '{0}'.", filePath));
        }

        string dstFilePath = Path.Combine(dstDirPath, fileName);

        File.Copy(filePath, dstFilePath);
      }

      foreach (string dirPath in Directory.GetDirectories(srcDirPath, "*", SearchOption.TopDirectoryOnly))
      {
        string dirName = Path.GetFileName(dirPath);

        if (string.IsNullOrEmpty(dirName))
        {
          throw new InternalException(string.Format("Unexpected lack of a directory name in a path. Directory path: '{0}'.", dirPath));
        }

        string dstSubDirPath = Path.Combine(dstDirPath, dirName);

        Directory.CreateDirectory(dstSubDirPath);

        CopyAll(dirPath, dstSubDirPath);
      }
    }

    #endregion
  }
}
