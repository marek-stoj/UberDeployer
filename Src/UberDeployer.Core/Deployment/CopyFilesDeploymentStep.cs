using System;
using System.IO;
using System.Linq;
using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Core.Deployment
{
  public class CopyFilesDeploymentStep : DeploymentStep
  {
    private readonly Lazy<string> _srcDirPathProvider;
    private readonly Lazy<string> _dstDirPath;

    #region Constructor(s)

    public CopyFilesDeploymentStep(Lazy<string> srcDirPathProvider, Lazy<string> dstDirPath)
    {
      Guard.NotNull(srcDirPathProvider, "srcDirPathProvider");
      Guard.NotNull(dstDirPath, "srcDirPathProvider");      

      _srcDirPathProvider = srcDirPathProvider;
      _dstDirPath = dstDirPath;
    }

    #endregion

    #region Overrides of DeploymentStep

    protected override void DoExecute()
    {
      if (!Directory.Exists(_srcDirPathProvider.Value))
      {
        throw new DeploymentTaskException(string.Format("Source directory doesn't exist: '{0}'.", _srcDirPathProvider));
      }

      if (Directory.Exists(_dstDirPath.Value))
      {
        Directory.GetDirectories(_dstDirPath.Value)
          .ToList()
          .ForEach(dirPath => Directory.Delete(dirPath, true));

        Directory.GetFiles(_dstDirPath.Value)
          .ToList()
          .ForEach(File.Delete);
      }
      else
      {
        Directory.CreateDirectory(_dstDirPath.Value);
      }

      CopyAll(_srcDirPathProvider.Value, _dstDirPath.Value);
    }

    public override string Description
    {
      get { return string.Format("Copy files to '{0}' from '{1}'.", _dstDirPath.Value, _srcDirPathProvider.Value); }
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
