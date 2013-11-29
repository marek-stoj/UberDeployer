using System;
using System.IO;
using System.Linq;
using UberDeployer.Common;
using UberDeployer.Common.IO;
using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Core.Deployment
{
  public class CopyFilesDeploymentStep : DeploymentStep
  {
    private const int _DeleteDstDirRetriesCount = 4;
    private const int _DeleteDstDirRetryDelay = 500;

    private readonly IDirectoryAdapter _directoryAdapter;
    private readonly IFileAdapter _fileAdapter;
    private readonly Lazy<string> _srcDirPathProvider;
    private readonly Lazy<string> _dstDirPath;

    #region Constructor(s)

    public CopyFilesDeploymentStep(IDirectoryAdapter directoryAdapter, IFileAdapter fileAdapter, Lazy<string> srcDirPathProvider, Lazy<string> dstDirPath)
    {
      Guard.NotNull(directoryAdapter, "directoryAdapter");
      Guard.NotNull(fileAdapter, "fileAdapter");
      Guard.NotNull(srcDirPathProvider, "srcDirPathProvider");
      Guard.NotNull(dstDirPath, "srcDirPathProvider");

      _directoryAdapter = directoryAdapter;
      _fileAdapter = fileAdapter;
      _srcDirPathProvider = srcDirPathProvider;
      _dstDirPath = dstDirPath;
    }

    #endregion

    #region Overrides of DeploymentStep

    protected override void DoExecute()
    {
      if (!_directoryAdapter.Exists(_srcDirPathProvider.Value))
      {
        throw new DeploymentTaskException(string.Format("Source directory doesn't exist: '{0}'.", _srcDirPathProvider));
      }

      if (_directoryAdapter.Exists(_dstDirPath.Value))
      {
        RetryUtils.RetryOnException(
          new[] { typeof(UnauthorizedAccessException) },
          _DeleteDstDirRetriesCount,
          _DeleteDstDirRetryDelay,
          () =>
          {
            _directoryAdapter.GetDirectories(_dstDirPath.Value)
              .ToList()
              .ForEach(dirPath => _directoryAdapter.Delete(dirPath, true));

            _directoryAdapter.GetFiles(_dstDirPath.Value)
              .ToList()
              .ForEach(_fileAdapter.Delete);
          });
      }
      else
      {
        _directoryAdapter.CreateDirectory(_dstDirPath.Value);
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
      foreach (string filePath in _directoryAdapter.GetFiles(srcDirPath, "*.*", SearchOption.TopDirectoryOnly))
      {
        string fileName = Path.GetFileName(filePath);

        if (string.IsNullOrEmpty(fileName))
        {
          throw new InternalException(string.Format("Unexpected lack of a file name in a path. File path: '{0}'.", filePath));
        }

        string dstFilePath = Path.Combine(dstDirPath, fileName);

        _fileAdapter.Copy(filePath, dstFilePath);
      }

      foreach (string dirPath in _directoryAdapter.GetDirectories(srcDirPath, "*", SearchOption.TopDirectoryOnly))
      {
        string dirName = Path.GetFileName(dirPath);

        if (string.IsNullOrEmpty(dirName))
        {
          throw new InternalException(string.Format("Unexpected lack of a directory name in a path. Directory path: '{0}'.", dirPath));
        }

        string dstSubDirPath = Path.Combine(dstDirPath, dirName);

        _directoryAdapter.CreateDirectory(dstSubDirPath);

        CopyAll(dirPath, dstSubDirPath);
      }
    }

    #endregion
  }
}
