using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UberDeployer.Core.DataAccess.Xml;

namespace UberDeployer.Core.Tests.Core.DataAccess.Xml
{
  [TestFixture]
  public class XmlEnvironmentInfoRepositoryTests
  {
    [Test]
    public void XmlEnvironmentInfoRepository_Throws_When_Argument_null()
    {
      Assert.Throws<ArgumentException>(() => new XmlEnvironmentInfoRepository(null));
    }

    [Test]
    public void XmlEnvironmentInfoRepository_Doesnt_Throw_When_()
    {
      new XmlEnvironmentInfoRepository("legit_path");
    }

    [Test]
    public void GetByName_Throws_When_Argument_null()
    {
      var repo = new XmlEnvironmentInfoRepository("another_legit_path");
      Assert.Throws<ArgumentException>(() => repo.FindByName(null));
    }
  }
}
