using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UberDeployer.Core.Tests.Deployment
{
  using NUnit.Framework;

  using UberDeployer.Core.DbDiff;
  using UberDeployer.Core.Deployment;

  [TestFixture]
  public class DbScriptToRunTests
  {
    [Test]
    public void missing_version_insert_fails()
    {
      const string scriptWithMissingVersionInsert = "USE Rahl DROP Stuff";

      Assert.IsFalse(DbScriptToRun.IsVersionInsertPresent(new DbVersion(1, 0), scriptWithMissingVersionInsert));
    }

    [Test]
    public void wrong_version_insert_fails()
    {
      const string scriptWithWrongVersionInsert = "INSERT INTO VERSION VALUES('1.5.0.0')";

      Assert.IsFalse(DbScriptToRun.IsVersionInsertPresent(new DbVersion(1, 4), scriptWithWrongVersionInsert));
    }

    [Test]
    public void correct_version_insert_succeeds()
    {
      const string scriptWithVersionInsert = "INSERT INTO VERSION VALUES('1.5.0.0')";

      Assert.IsTrue(DbScriptToRun.IsVersionInsertPresent(new DbVersion(1, 5), scriptWithVersionInsert));
    }

    [Test]
    public void correct_version_insert_split_into_lines_succeeds()
    {
      const string scriptWithVersionInsert = "INSERT \n\r INTO \n\r [VERSION] VALUES('1.5.0.0')";

      Assert.IsTrue(DbScriptToRun.IsVersionInsertPresent(new DbVersion(1, 5), scriptWithVersionInsert));
    }

    [Test]
    public void revision_number_is_checked()
    {
      const string scriptWithVersionInsert = "INSERT \n\r INTO \n\r [VERSION] VALUES('1.4')";

      Assert.IsFalse(DbScriptToRun.IsVersionInsertPresent(new DbVersion(1, 4, 1), scriptWithVersionInsert));
    }

    [Test]
    public void build_number_is_checked()
    {
      const string scriptWithVersionInsert = "INSERT \n\r INTO \n\r [VERSION] VALUES('1.4.1')";

      Assert.IsFalse(DbScriptToRun.IsVersionInsertPresent(new DbVersion(1, 4, 1, 2), scriptWithVersionInsert));
    }

    [Test]
    public void revision_number_is_checked_positive()
    {
      const string scriptWithVersionInsert = "INSERT \n\r INTO \n\r [VERSION] VALUES('1.4.1.0')";

      Assert.IsTrue(DbScriptToRun.IsVersionInsertPresent(new DbVersion(1, 4, 1), scriptWithVersionInsert));
    }

    [Test]
    public void build_number_is_checked_positive()
    {
      const string scriptWithVersionInsert = "INSERT \n\r INTO \n\r [VERSION] VALUES('1.4.1.2')";

      Assert.IsTrue(DbScriptToRun.IsVersionInsertPresent(new DbVersion(1, 4, 1, 2), scriptWithVersionInsert));
    }
  }
}
