using System;
using NUnit.Framework;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.Tests.Domain
{
  [TestFixture]
  public class IisAppPoolInfoTests
  {
    [Test]
    public void Test_IisAppPoolInfoTests_Thows_When_Name_null()
    {
      Assert.Throws<ArgumentException>(
        () =>
          { new IisAppPoolInfo(null, IisAppPoolVersion.V2_0, IisAppPoolMode.Classic); });
    }

    [Test]
    public void Test_ToString_PrintsProperString()
    {
      var info = new IisAppPoolInfo("name", IisAppPoolVersion.V2_0, IisAppPoolMode.Classic);
      Assert.AreEqual("Name: 'name'. Version: 'V2_0'. Mode: 'Classic'.", info.ToString());
    }

  }
}
