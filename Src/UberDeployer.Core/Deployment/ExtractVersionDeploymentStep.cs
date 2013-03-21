using System;
using System.Diagnostics;
using System.IO;
using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Core.Deployment
{
  public class ExtractVersionDeploymentStep : DeploymentStep  
  {
    private readonly Lazy<string> _binariesDirPathProvider;
    private readonly string _terminalAppExeName;

    private string _resolvedVersion;

    public ExtractVersionDeploymentStep(Lazy<string> binariesDirPathProvider, string terminalAppExeName)
    {
      Guard.NotNull(binariesDirPathProvider);
      Guard.NotNullNorEmpty(terminalAppExeName);

      _binariesDirPathProvider = binariesDirPathProvider;
      _terminalAppExeName = terminalAppExeName;
    }

    public override string Description
    {
      get { return string.Format("Extract version info of file '{0}/{1}'.", _binariesDirPathProvider.Value, _terminalAppExeName); }
    }

    public string Version
    {
      get { return _resolvedVersion; }
    }

    protected override void DoExecute()
    {
      if (string.IsNullOrEmpty(_binariesDirPathProvider.Value))
      {
        throw new InternalException("BinariesDirPathProvider should have a value at this point.");
      }

      string path = Path.Combine(_binariesDirPathProvider.Value, _terminalAppExeName);
      FileVersionInfo version = FileVersionInfo.GetVersionInfo(path);

      if (version == null || string.IsNullOrEmpty(version.ProductVersion))
      {
        throw new InternalException(string.Format("Couldn't find file version number for file {0}/{1}", _binariesDirPathProvider.Value, _terminalAppExeName));
      }

      _resolvedVersion = version.ProductVersion;
    }
  }
}
