using System;
using System.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Management.Shortcut;

namespace UberDeployer.Core.Deployment
{
  public class UpdateApplicationShortcutDeploymentStep : DeploymentStep
  {
    private readonly string _terminalAppsShortcutFolder;
    private readonly Lazy<string> _versionDeploymentDirPath;
    private readonly string _terminalAppExeName;
    private readonly string _projectName;

    public UpdateApplicationShortcutDeploymentStep(
      string terminalAppsShortcutFolder, 
      Lazy<string> versionDeploymentDirPath, 
      string terminalAppExeName, 
      string projectName)
    {
      Guard.NotNullNorEmpty(terminalAppsShortcutFolder, "terminalAppsShortcutFolder");
      Guard.NotNull(versionDeploymentDirPath, "versionDeploymentDirPath");
      Guard.NotNullNorEmpty(terminalAppExeName, "terminalAppExeName");
      Guard.NotNullNorEmpty(projectName, "projectName");

      _terminalAppsShortcutFolder = terminalAppsShortcutFolder;
      _versionDeploymentDirPath = versionDeploymentDirPath;
      _terminalAppExeName = terminalAppExeName;
      _projectName = projectName;
    }

    public override string Description
    {
      get
      {
        return string.Format(
          "Update application shortcut for application '{0}' in folder '{1}' to '{2}\\{3}'.",
          _projectName,
          _terminalAppsShortcutFolder,
          _versionDeploymentDirPath.Value,
          _terminalAppExeName);
      }
    }

    protected override void DoExecute()
    {
      if (string.IsNullOrEmpty(_versionDeploymentDirPath.Value))
      {
        throw new DeploymentTaskException("_versionDeploymentDirPath not created.");
      }

      // check if file exists
      string targetPath = Path.Combine(_versionDeploymentDirPath.Value, _terminalAppExeName);
      if (!File.Exists(targetPath))
      {
        throw new DeploymentTaskException(string.Format("Target file {0} does not exist when creating shortcut", targetPath));
      }

      string shortcutPath = Path.Combine(_terminalAppsShortcutFolder, _projectName + ".lnk");
      using (var shortcut = new ShellShortcut(shortcutPath))
      {
        shortcut.Path = targetPath;
        shortcut.WorkingDirectory = Path.GetDirectoryName(_versionDeploymentDirPath.Value);
        shortcut.Description = _projectName;
        shortcut.Save();
      }
    }
  }
}