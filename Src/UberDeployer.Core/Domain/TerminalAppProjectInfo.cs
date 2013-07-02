using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UberDeployer.Common.IO;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain.Input;
using System.Linq;

namespace UberDeployer.Core.Domain
{
  public class TerminalAppProjectInfo : ProjectInfo
  {
    private static readonly Regex _VersionedFoldeRegex = new Regex("^(?<Major>[0-9]+)\\.(?<Minor>[0-9]+)\\.(?<Revision>[0-9]+)\\.(?<Build>[0-9]+)(\\s*)?(?<Custom>.*?)(\\s*)?(\\.(?<Marker>[0-9]+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    #region Nested types

    private class VersionedFolder
    {
      public VersionedFolder(int major, int minor, int revision, int build, string custom, int? marker = null)
      {
        Major = major;
        Minor = minor;
        Revision = revision;
        Build = build;
        Custom = custom;
        Marker = marker;
      }

      public bool IsSmallerThan(VersionedFolder other)
      {
        Guard.NotNull(other, "other");

        return Major < other.Major
            || (Major == other.Major && Minor < other.Minor)
            || (Minor == other.Minor && Revision < other.Revision)
            || (Revision == other.Revision && Build < other.Build)
            || (Build == other.Build && !Marker.HasValue && other.Marker.HasValue)
            || (Build == other.Build && Marker.HasValue && other.Marker.HasValue && Marker.Value < other.Marker.Value);
      }

      private bool Equals(VersionedFolder other)
      {
        return Major == other.Major && Minor == other.Minor && Revision == other.Revision && Build == other.Build && string.Equals(Custom, other.Custom) && Marker == other.Marker;
      }

      public override bool Equals(object obj)
      {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((VersionedFolder)obj);
      }

      public override int GetHashCode()
      {
        unchecked
        {
          int hashCode = Major;
          hashCode = (hashCode * 397) ^ Minor;
          hashCode = (hashCode * 397) ^ Revision;
          hashCode = (hashCode * 397) ^ Build;
          hashCode = (hashCode * 397) ^ (Custom != null ? Custom.GetHashCode() : 0);
          hashCode = (hashCode * 397) ^ Marker.GetHashCode();
          return hashCode;
        }
      }

      // ReSharper disable MemberCanBePrivate.Local

      public int Major { get; private set; }

      public int Minor { get; private set; }

      public int Revision { get; private set; }

      public int Build { get; private set; }

      public string Custom { get; private set; }

      public int? Marker { get; private set; }

      // ReSharper restore MemberCanBePrivate.Local
    }

    #endregion

    #region Constructor(s)

    public TerminalAppProjectInfo(string name, string artifactsRepositoryName, string artifactsRepositoryDirName, bool artifactsAreNotEnvironmentSpecific, string terminalAppName, string terminalAppDirName, string terminalAppExeName)
      : base(name, artifactsRepositoryName, artifactsRepositoryDirName, artifactsAreNotEnvironmentSpecific)
    {
      Guard.NotNullNorEmpty(terminalAppName, "terminalAppName");
      Guard.NotNullNorEmpty(terminalAppDirName, "terminalAppDirName");
      Guard.NotNullNorEmpty(terminalAppExeName, "terminalAppExeName");

      TerminalAppName = terminalAppName;
      TerminalAppDirName = terminalAppDirName;
      TerminalAppExeName = terminalAppExeName;
    }

    #endregion

    #region Overrides of ProjectInfo

    public override ProjectType Type
    {
      get { return ProjectType.TerminalApp; }
    }

    public override InputParams CreateEmptyInputParams()
    {
      return new TerminalAppInputParams();
    }

    public override DeploymentTask CreateDeploymentTask(IObjectFactory objectFactory)
    {
      if (objectFactory == null)
      {
        throw new ArgumentNullException("objectFactory");
      }

      return
        new DeployTerminalAppDeploymentTask(
          objectFactory.CreateProjectInfoRepository(),
          objectFactory.CreateEnvironmentInfoRepository(),
          objectFactory.CreateArtifactsRepository());
    }

    public override IEnumerable<string> GetTargetFolders(IObjectFactory objectFactory, EnvironmentInfo environmentInfo)
    {
      Guard.NotNull(objectFactory, "objectFactory");
      Guard.NotNull(environmentInfo, "environmentInfo");

      string baseDirPath =
        environmentInfo.GetTerminalServerNetworkPath(
          Path.Combine(environmentInfo.TerminalAppsBaseDirPath, TerminalAppDirName));

      string latestVersionDirPath =
        FindLatestVersionDirPath(
          objectFactory.CreateDirectoryAdapter(),
          baseDirPath);

      return
        !string.IsNullOrEmpty(latestVersionDirPath)
          ? new[] { latestVersionDirPath }
          : new[] { baseDirPath };
    }

    public override string GetMainAssemblyFileName()
    {
      return TerminalAppExeName;
    }

    #endregion

    #region Private methods

    private static string FindLatestVersionDirPath(IDirectoryAdapter directoryAdapter, string baseDirPath)
    {
      Guard.NotNull(directoryAdapter, "directoryAdapter");
      Guard.NotNullNorEmpty(baseDirPath, "baseDirPath");

      string[] subDirs =
        directoryAdapter.GetDirectories(baseDirPath, "*.*", SearchOption.TopDirectoryOnly);

      var versionedFolders = new List<Tuple<VersionedFolder, string>>();

      foreach (string subDirPath in subDirs)
      {
        string subDir = Path.GetFileName(subDirPath) ?? "";
        Match match = _VersionedFoldeRegex.Match(subDir);

        if (!match.Success)
        {
          continue;
        }

        string majorStr = match.Groups["Major"].Value;
        string minorStr = match.Groups["Minor"].Value;
        string revisionStr = match.Groups["Revision"].Value;
        string buildStr = match.Groups["Build"].Value;
        string customStr = match.Groups["Custom"].Value;
        string markerStr = match.Groups["Marker"].Value;

        var versionedFolder =
          new VersionedFolder(
            int.Parse(majorStr),
            int.Parse(minorStr),
            int.Parse(revisionStr),
            int.Parse(buildStr),
            customStr,
            !string.IsNullOrEmpty(markerStr) ? int.Parse(markerStr) : 0);

        versionedFolders.Add(new Tuple<VersionedFolder, string>(versionedFolder, subDirPath));
      }

      versionedFolders.Sort(
        (folder, otherFolder) =>
        Equals(folder.Item1, otherFolder.Item1)
          ? 0
          : folder.Item1.IsSmallerThan(otherFolder.Item1)
              ? 1
              : -1);

      Tuple<VersionedFolder, string> latestVersionedFolder =
        versionedFolders.FirstOrDefault();

      return
        latestVersionedFolder != null
          ? latestVersionedFolder.Item2
          : null;
    }

    #endregion

    #region Properties

    public string TerminalAppName { get; private set; }

    public string TerminalAppDirName { get; private set; }

    public string TerminalAppExeName { get; private set; }

    #endregion
  }
}
