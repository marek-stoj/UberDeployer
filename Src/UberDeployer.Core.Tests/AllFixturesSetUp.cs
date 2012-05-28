using NUnit.Framework;
using UberDeployer.CommonConfiguration;

namespace UberDeployer.Core.Tests
{
  [SetUpFixture]
  public class AllFixturesSetUp
  {
    [SetUp]
    public void SetUp()
    {
      Bootstraper.Bootstrap();
    }
  }
}
