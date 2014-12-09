using System;
using System.Linq;
using UberDeployer.Common;
using UberDeployer.Common.IO;
using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Core.Deployment
{
  public class CleanDirectoryDeploymentStep : DeploymentStep
  {
    private readonly IDirectoryAdapter _directoryAdapter;
    private readonly IFileAdapter _fileAdapter;
    private readonly Lazy<string> _dstDirPath;
    private readonly string[] _excludedDirs;

    private const int _DeleteDstDirRetriesCount = 4;
    private const int _DeleteDstDirRetryDelay = 500;

    public CleanDirectoryDeploymentStep(IDirectoryAdapter directoryAdapter, IFileAdapter fileAdapter, Lazy<string> dstDirPath, string[] excludedDirs)
    {
      Guard.NotNull(dstDirPath, "dstDirPath");
      Guard.NotNull(fileAdapter, "fileAdapter");
      Guard.NotNull(directoryAdapter, "directoryAdapter");

      _dstDirPath = dstDirPath;
      _directoryAdapter = directoryAdapter;
      _excludedDirs = excludedDirs;
      _fileAdapter = fileAdapter;
    }

    public override string Description
    {
      get { return String.Format("Cleaning files from '{0}'.", _dstDirPath.Value); }
    }

    protected override void DoExecute()
    {
      if (_directoryAdapter.Exists(_dstDirPath.Value))
      {
        RetryUtils.RetryOnException(
          new[] { typeof(UnauthorizedAccessException) },
          _DeleteDstDirRetriesCount,
          _DeleteDstDirRetryDelay,
          () =>
          {
            _directoryAdapter.GetDirectories(_dstDirPath.Value)
              .Where(x => !_excludedDirs.Any(x.EndsWith))
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

    }
  }
}