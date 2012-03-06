using System;
using NUnit.Framework;
using UberDeployer.Core.DataAccess.Xml;

namespace UberDeployer.Core.Tests.Core.DataAccess.Xml
{
  [TestFixture]
  public class XmlProjectInfoRepositoryTests
  {
    [Test]
    public void XmlProjectInfoRepository_Throws_When_Argument_null()
    {
      Assert.Throws<ArgumentException>(() => new XmlProjectInfoRepository(null));
    }

    [Test]
    public void XmlProjectInfoRepository_Doesnt_Throw_When_()
    {
      new XmlProjectInfoRepository("legit_path");
    }

    [Test]
    public void GetByName_Throws_When_Argument_null()
    {
      var repo = new XmlProjectInfoRepository("another_legit_path");
      Assert.Throws<ArgumentException>(() => repo.GetByName(null));
    }
  }
}
