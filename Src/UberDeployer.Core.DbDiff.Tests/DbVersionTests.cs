using NUnit.Framework;

namespace UberDeployer.Core.DbDiff.Tests
{
  [TestFixture]
  public class DbVersionTests
  {
    [Test]
    [TestCase("1", "2")]
    [TestCase("1.1", "1.2")]
    [TestCase("1.1.1", "1.1.2")]
    [TestCase("1.1.1.1", "1.1.1.2")]
    [TestCase("1.1.1.1_alpha", "1.1.1.1_beta")]
    [TestCase("1.1.1.1_alpha", "1.1.1.1_Beta")]
    [TestCase("1.5.0.1", "1.34.0")]
    public void Test_CompareTo_first_smaller(string dbVersionStr1, string dbVersionStr2)
    {
      DbVersion dbVersion1 = DbVersion.FromString(dbVersionStr1);
      DbVersion dbVersion2 = DbVersion.FromString(dbVersionStr2);

      Assert.AreEqual(-1, dbVersion1.CompareTo(dbVersion2));
      Assert.AreEqual(1, dbVersion2.CompareTo(dbVersion1));
    }

    [Test]
    [TestCase("0", "0")]
    [TestCase("0.0", "0.0")]
    [TestCase("0.0.0", "0.0.0")]
    [TestCase("0.0.0.0", "0.0.0.0")]
    [TestCase("0.0.0.0_tail", "0.0.0.0_tail")]
    [TestCase("1.2.3.4", "1.2.3.4")]
    [TestCase("4.3", "4.3")]
    [TestCase("4.3beta", "4.3beta")]
    [TestCase("4.3.notrans", "4.3.NoTrans")]
    public void Test_CompareTo_equal(string dbVersionStr1, string dbVersionStr2)
    {
      DbVersion dbVersion1 = DbVersion.FromString(dbVersionStr1);
      DbVersion dbVersion2 = DbVersion.FromString(dbVersionStr2);

      Assert.AreEqual(0, dbVersion1.CompareTo(dbVersion2));
      Assert.AreEqual(0, dbVersion2.CompareTo(dbVersion1));
    }
  }
}
