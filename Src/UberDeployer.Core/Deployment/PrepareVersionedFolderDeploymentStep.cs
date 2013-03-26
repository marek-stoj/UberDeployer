using System;
using System.IO;
using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Core.Deployment
{
  public class PrepareVersionedFolderDeploymentStep : DeploymentStep
  {
    private readonly string _projectName;
    private readonly string _baseDirPath;
    private readonly string _dirName;
    private readonly Lazy<string> _version;

    private string _createdVersionedFolderPath;

    public PrepareVersionedFolderDeploymentStep(string projectName, string baseDirPath, string dirName, Lazy<string> version)
    {
      Guard.NotNullNorEmpty(projectName, "projectName");
      Guard.NotNullNorEmpty(baseDirPath, "baseDirPath");
      Guard.NotNullNorEmpty(dirName, "dirName");
      Guard.NotNull(version, "version");

      _projectName = projectName;
      _dirName = dirName;
      _baseDirPath = baseDirPath;
      _version = version;
    }

    public override string Description
    {
      get
      {
        return string.Format("Prepare folder for project '{0}' in path '{1}\\{2}' for version '{3}'.", _projectName, _baseDirPath, _dirName, _version.Value);
      }
    }

    protected override void DoExecute()
    {
      string path = Path.Combine(_baseDirPath, _dirName, _version.Value);

      if (!Directory.Exists(_baseDirPath))
      {
        throw new DeploymentTaskException(string.Format("Terminal apps base folder '{0}' does not exist!", _baseDirPath));
      }

      int uniqueMarker = 1;

      while (Directory.Exists(path))
      {
        path = Path.Combine(_baseDirPath, _dirName, _version.Value + "." + (uniqueMarker++));
      }

      Directory.CreateDirectory(path);

      _createdVersionedFolderPath = path;
    }

    public string VersionDeploymentDirPath
    {
      get
      {
        if (!IsPrepared)
        {
          throw new InvalidOperationException("Step has not been prepared yet.");
        }

        return _createdVersionedFolderPath;
      }
    }    
  }
}
