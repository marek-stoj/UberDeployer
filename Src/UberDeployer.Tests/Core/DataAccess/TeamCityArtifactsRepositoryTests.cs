using System;

using Moq;

using NUnit.Framework;

using UberDeployer.Core.DataAccess;
using UberDeployer.Core.TeamCity;

namespace UberDeployer.Tests.Core.DataAccess
{
  [TestFixture]
  public class TeamCityArtifactsRepositoryTests
  {
    private Mock<ITeamCityRestClient> _teamCityClient;

    // SUT
    private TeamCityArtifactsRepository _teamCityArtifactsRepository;

    [SetUp]
    public void SetUp()
    {
      _teamCityClient = new Mock<ITeamCityRestClient>(MockBehavior.Loose);
      _teamCityArtifactsRepository = new TeamCityArtifactsRepository(_teamCityClient.Object);
    }

    [Test]
    public void TeamCityArtifactsRepositoryConstructor_WhenTeamCityClientIsNull_ThrowsArgumentNullException()
    {
      Assert.Throws<ArgumentNullException>(() => new TeamCityArtifactsRepository(null));
    }

    [Test]
    public void GetArtifacts_WhenProjectConfigurationBuildIdIsNullOrEmpty_ThrowsArgumentException()
    {
      Assert.Throws<ArgumentException>(() => _teamCityArtifactsRepository.GetArtifacts(null, "destinationFilePath"));
    }

    [Test]
    public void GetArtifacts_WhenDestinationFilePathIsNullOrEmpty_ThrowsArgumentException()
    {
      Assert.Throws<ArgumentException>(() => _teamCityArtifactsRepository.GetArtifacts("projectConfigurationBuildId", null));
    }
  }
}
