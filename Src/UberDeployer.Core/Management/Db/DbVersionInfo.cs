namespace UberDeployer.Core.Management.Db
{
  public class DbVersionInfo
  {
    public string Version { get; set; }

    public bool IsMigrated { get; set; }

    public string[] GetRunnedVersions()
    {
      if (IsMigrated)
      {
        return new[]
          {
            Version,
            string.Join(".", Version, "migration")
          };
      }

      return new[] { Version };
    }
  }
}