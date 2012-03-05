using System;
using System.Text.RegularExpressions;

namespace UberDeployer.Core.DbDiff
{
  public class DbVersion
  {
    private static readonly Regex _VersionRegex = new Regex(@"^(?<Major>[0-9]+)(\.(?<Minor>[0-9]+))?(\.(?<Revision>[0-9]+))?(\.(?<Build>[0-9]+))?(?<Tail>.*)$", RegexOptions.Compiled);

    #region Constructor(s)

    public DbVersion(int major, int minor = 0, int revision = 0, int build = 0, string tail = "")
    {
      Major = major;
      Minor = minor;
      Revision = revision;
      Build = build;
      Tail = tail;
    }

    #endregion

    #region Public methods

    public static DbVersion FromString(string versionStr)
    {
      if (string.IsNullOrEmpty(versionStr)) throw new ArgumentException("Argument can't be null nor empty.", "versionStr");

      Match match = _VersionRegex.Match(versionStr);

      if (!match.Success)
      {
        throw new FormatException("Version string is not in correct format. Expected: 'Major[.Minor][.Revision][.Build]'.");
      }

      string majorStr = match.Groups["Major"].Value;
      string minorStr = match.Groups["Minor"].Value;
      string revisionStr = match.Groups["Revision"].Value;
      string buildStr = match.Groups["Build"].Value;

      return
        new DbVersion(
          int.Parse(majorStr),
          !string.IsNullOrEmpty(minorStr) ? int.Parse(minorStr) : 0,
          !string.IsNullOrEmpty(revisionStr) ? int.Parse(revisionStr) : 0,
          !string.IsNullOrEmpty(buildStr) ? int.Parse(buildStr) : 0);
    }

    public bool IsSmallerThan(DbVersion otherVersion)
    {
      if (otherVersion == null) throw new ArgumentNullException("otherVersion");

      return Major < otherVersion.Major
          || (Major == otherVersion.Major && Minor < otherVersion.Minor)
          || (Minor == otherVersion.Minor && Revision < otherVersion.Revision)
          || (Revision == otherVersion.Revision && Build < otherVersion.Build)
          || (Build == otherVersion.Build && Tail.CompareTo(otherVersion.Tail) < 0);
    }

    #endregion

    #region Object overrides

    public override string ToString()
    {
      return string.Format("{0}.{1}.{2}.{3}", Major, Minor, Revision, Build);
    }

    #endregion

    #region Properties

    public int Major { get; private set; }

    public int Minor { get; private set; }

    public int Revision { get; private set; }

    public int Build { get; private set; }

    public string Tail { get; private set; }

    #endregion
  }
}
