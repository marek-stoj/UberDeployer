using System;
using System.IO;
using Ionic.Zip;
using UberDeployer.Core.Domain;

// TODO IMM HI: review
namespace UberDeployer.Core.Deployment
{
  public class BackupFilesDeploymentStep : DeploymentStep
  {
    private const string _BackupExtension = ".zip";
    private const int _DefaultMaxBackupCount = 1;

    private readonly string _destinationPath;
    private readonly int _maxBackupNumber;

    #region Constructor(s)

    public BackupFilesDeploymentStep(string destinationPath, int maxBackupNumber)
    {
      if (string.IsNullOrEmpty(destinationPath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "destinationPath");
      }

      if (!Path.IsPathRooted(destinationPath))
      {
        throw new ArgumentException("Destination path must be an absolute path.", "destinationPath");
      }

      _destinationPath = destinationPath;
      _maxBackupNumber = maxBackupNumber;
    }

    public BackupFilesDeploymentStep(string destinationPath)
      : this(destinationPath, _DefaultMaxBackupCount)
    {
    }

    #endregion

    #region Overrides of DeploymentStep

    public override string Description
    {
      get { return string.Format("Backup previously deployed files from '{0}'.", _destinationPath); }
    }

    protected override void DoExecute()
    {
      if (!Directory.Exists(_destinationPath))
      {
        throw new InvalidOperationException(string.Format("Specified path does not exist: '{0}'.", _destinationPath));
      }

      string parentDirPath = Path.GetDirectoryName(_destinationPath);

      if (string.IsNullOrEmpty(parentDirPath))
      {
        throw new Exception(string.Format("Invalid path (couldn't get parent directory): '{0}'.", _destinationPath));
      }

      string destDirName = Path.GetFileName(_destinationPath);

      if (_maxBackupNumber > 1)
      {
        string searchPattern = string.Format("{0}.bak.*{1}", destDirName, _BackupExtension);
        string[] files = Directory.GetFiles(parentDirPath, searchPattern, SearchOption.TopDirectoryOnly);

        for (int fileIndex = files.Length; fileIndex > 0; fileIndex--)
        {
          if (fileIndex < _maxBackupNumber)
          {
            string destFilePath =
              string.Format(
                "{0}{1}{2}.bak.{3:000}{4}",
                parentDirPath,
                Path.DirectorySeparatorChar,
                destDirName,
                fileIndex, _BackupExtension);

            if (File.Exists(destFilePath))
            {
              File.Delete(destFilePath);
            }

            File.Move(files[fileIndex - 1], destFilePath);
          }
        }
      }

      using (var zip = new ZipFile())
      {
        zip.AddDirectory(_destinationPath);

        string zipFilePath =
          string.Format(
            "{0}{1}{2}.bak.000{3}",
            parentDirPath,
            Path.DirectorySeparatorChar,
            destDirName, _BackupExtension);

        zip.Save(zipFilePath);
      }
    }

    #endregion
  }
}
