using System;
using System.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Deployment
{
  public class PrepareVersionedFolderDeploymentStep : DeploymentStep
  {
    private readonly string _terminalAppsBaseDirPath;
    private readonly string _projectName;
    private readonly Lazy<string> _version;

    private string _createdVersionedFolderPath;

    public PrepareVersionedFolderDeploymentStep(ProjectInfo projectInfo, string terminalAppsBaseDirPath, string projectName, Lazy<string> version)
      : base(projectInfo)
    {
      Guard.NotNullNorEmpty(terminalAppsBaseDirPath, "terminalAppsBaseDirPath");
      Guard.NotNullNorEmpty(projectName, "projectName");
      Guard.NotNull(version, "version");

      _terminalAppsBaseDirPath = terminalAppsBaseDirPath;
      _projectName = projectName;
      _version = version;
    }

    public override string Description
    {
      get
      {
        return string.Format("Prepare folder for project '{0}' in base path '{1}' for version '{2}'.", _projectName, _terminalAppsBaseDirPath, _version.Value);
      }
    }

    protected override void DoExecute()
    {
      string path = Path.Combine(_terminalAppsBaseDirPath, _projectName, _version.Value);

      if (!Directory.Exists(_terminalAppsBaseDirPath))
      {
        throw new DeploymentTaskException(string.Format("Terminal apps base folder '{0}' does not exist!", _terminalAppsBaseDirPath));
      }

      int uniqueMarker = 1;

      while (Directory.Exists(path))
      {
        path = Path.Combine(_terminalAppsBaseDirPath, _projectName, _version.Value + "." + (uniqueMarker++));
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