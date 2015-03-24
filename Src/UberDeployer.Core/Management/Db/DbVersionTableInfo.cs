namespace UberDeployer.Core.Management.Db
{
  public class DbVersionTableInfo
  {
    public string TableName { get; set; }

    public string VersionColumnName { get; set; }

    public string MigrationColumnName { get; set; }
  }
}