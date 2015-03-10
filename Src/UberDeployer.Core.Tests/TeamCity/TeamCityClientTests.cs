using System.Linq;

using NUnit.Framework;

using UberDeployer.Core.TeamCity;
using UberDeployer.Core.TeamCity.Models;

namespace UberDeployer.Core.Tests.TeamCity
{
  [TestFixture]
  [Ignore("Tests should be run manually, because external teamcity address is used.")]
  public class TeamCityClientTests
  {
    private TeamCityClient _sut;

    [TestFixtureSetUp]
    public void TestFixtureSetup()
    {
      _sut = new TeamCityClient("teamcity.jetbrains.com", 80);
    }

    [Test]
    public void GetProjectConfigurationDetails_returns_feature_branches()
    {
      var projectConfigurationDetails = _sut.GetProjectConfigurationDetails(
        new ProjectConfiguration
        {
          Href = "/guestAuth/app/rest/buildTypes/id:bt345"
        });

      Assert.That(projectConfigurationDetails.Branches != null);
      Assert.That(projectConfigurationDetails.Branches.Count > 1);
      Assert.That(projectConfigurationDetails.Branches.Branches.Any(x => x.IsDefault));
    }
  }
}