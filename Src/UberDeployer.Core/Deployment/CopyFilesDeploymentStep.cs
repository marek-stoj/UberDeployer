using System;
using UberDeployer.Common.IO;
using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Core.Deployment
{
  public class CopyFilesDeploymentStep : DeploymentStep
  {
    private readonly IDirectoryAdapter _directoryAdapter;
    private readonly Lazy<string> _srcDirPathProvider;
    private readonly Lazy<string> _dstDirPath;

    #region Constructor(s)

    public CopyFilesDeploymentStep(
      IDirectoryAdapter directoryAdapter,
      Lazy<string> srcDirPathProvider,
      Lazy<string> dstDirPath)
    {
      Guard.NotNull(directoryAdapter, "directoryAdapter");
      Guard.NotNull(srcDirPathProvider, "srcDirPathProvider");
      Guard.NotNull(dstDirPath, "srcDirPathProvider");

      _directoryAdapter = directoryAdapter;
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

      if (!_directoryAdapter.Exists(_dstDirPath.Value))
      {
        _directoryAdapter.CreateDirectory(_dstDirPath.Value);
      }

      _directoryAdapter.CopyAll(_srcDirPathProvider.Value, _dstDirPath.Value);
    }

    public override string Description
    {
      get { return string.Format("Copy files to '{0}' from '{1}'.", _dstDirPath.Value, _srcDirPathProvider.Value); }
    }

    #endregion
  }
}
