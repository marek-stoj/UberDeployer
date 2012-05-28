using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using UberDeployer.Core.Management.Db;

namespace UberDeployer.Core.Tests.Management.Db
{
  [TestFixture]
  public class DbVersionProviderTests
  {
    private const string _DbName = "UberDeployer";
    private const string _TableName = "VersionTable";
    private const string _ColumnName = "VersionColumn";

    private readonly string _SqlServerName = ConfigurationManager.AppSettings["SqlServerName"];
    private DbVersionProvider _dbVersionProvider;

    private static readonly IEnumerable<DbVersionTableInfo> _versionTableInfos =
      new List<DbVersionTableInfo>
        {
          new DbVersionTableInfo
            {
              TableName = _TableName,
              ColumnName = _ColumnName
            },
          new DbVersionTableInfo
            {
              TableName = "VERSIONHISTORY",
              ColumnName = "DBLabel"
            }
        };

    [SetUp]
    public void SetUp()
    {
      _dbVersionProvider = new DbVersionProvider(_versionTableInfos);
    }

    [Test]
    public void Constructor_checks_arguments()
    {
      Assert.Throws<ArgumentNullException>(() => new DbVersionProvider(null));
    }

    [Test]
    [Sequential]
    public void GetVersions_fails_on_null_or_empty_dbName([Values("", null)] string dbName)
    {
      Assert.Throws<ArgumentException>(() => _dbVersionProvider.GetVersions(dbName, _SqlServerName));
    }

    [Test]
    [Sequential]
    public void GetVersions_fails_on_null_or_empty_sqlServerName([Values("", null)] string sqlServerName)
    {
      Assert.Throws<ArgumentException>(() => _dbVersionProvider.GetVersions(_DbName, sqlServerName));
    }

    // TODO IMM HI: use in-memory sqlite
    [Test]
    [Ignore]
    public void GetVersions_gets_all_versions_from_specified_database_table()
    {
      // Arrange      
      string[] expectedVersions = new[] { "1.0", "1.5", "2.0" };

      try
      {
        DbVersionProviderTestUtils.CreateAndFillTable(_TableName, _ColumnName, expectedVersions);

        var dbVersionProvider = new DbVersionProvider(
          new List<DbVersionTableInfo>
            {
              new DbVersionTableInfo
                {
                  TableName = _TableName,
                  ColumnName = _ColumnName
                }
            });

        // Act
        List<string> currentVersions =
          dbVersionProvider.GetVersions(_DbName, _SqlServerName).ToList();

        // Assert
        Assert.AreEqual(expectedVersions.Length, currentVersions.Count());

        foreach (var expectedVersion in expectedVersions)
        {
          Assert.IsTrue(currentVersions.Contains(expectedVersion));
        }
      }
      finally
      {
        DbVersionProviderTestUtils.DropTableIfExists(_TableName);
      }
    }

    // TODO IMM HI: use in-memory sqlite
    [Test]
    [Ignore]
    public void GetVersions_returns_empty_list_when_specified_version_table_was_not_found()
    {
      try
      {
        // Arrange
        DbVersionProviderTestUtils.CreateAndFillTable(_TableName, _ColumnName, new[] { "1.2" });

        var dbVersionProvider = new DbVersionProvider(
          new List<DbVersionTableInfo>
            {
              new DbVersionTableInfo
                {
                  TableName = "OtherTableName",
                  ColumnName = "OtherColumnName"
                }
            });

        // Act
        var versions = dbVersionProvider.GetVersions(_DbName, _SqlServerName);

        // Assert
        Assert.IsFalse(versions.Any());
      }
      finally
      {
        DbVersionProviderTestUtils.DropTableIfExists(_TableName);
      }
    }
  }
}
