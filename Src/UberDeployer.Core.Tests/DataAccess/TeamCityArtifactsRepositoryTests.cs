using System;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.DataAccess;
using UberDeployer.Core.TeamCity;

namespace UberDeployer.Core.Tests.DataAccess
{
  [TestFixture]
  public class TeamCityArtifactsRepositoryTests
  {
    // SUT
    private TeamCityArtifactsRepository _teamCityArtifactsRepository;

    private Mock<ITeamCityClient> _teamCityClientFake;

    [SetUp]
    public void SetUp()
    {
      _teamCityClientFake = new Mock<ITeamCityClient>(MockBehavior.Loose);
      _teamCityArtifactsRepository = new TeamCityArtifactsRepository(_teamCityClientFake.Object);
    }

    [Test]
    public void TeamCityArtifactsRepositoryConstructor_WhenTeamCityClientIsNull_ThrowsArgumentNullException()
    {
      Assert.Throws<ArgumentNullException>(() => new TeamCityArtifactsRepository(null));
    }

    [Test]
    public void GetArtifacts_WhenProjectNameIsNullOrEmpty_ThrowsArgumentException()
    {
      Assert.Throws<ArgumentException>(() => _teamCityArtifactsRepository.GetArtifacts(null, "projectConfigurationName", "projectConfigurationBuildId", "destinationFilePath"));
    }

    [Test]
    public void GetArtifacts_WhenProjectConfigurationNameIsNullOrEmpty_ThrowsArgumentException()
    {
      Assert.Throws<ArgumentException>(() => _teamCityArtifactsRepository.GetArtifacts("projectName", null, "projectConfigurationBuildId", "destinationFilePath"));
    }

    [Test]
    public void GetArtifacts_WhenProjectConfigurationBuildIdIsNullOrEmpty_ThrowsArgumentException()
    {
      Assert.Throws<ArgumentException>(() => _teamCityArtifactsRepository.GetArtifacts("projectName", "projectConfigurationName", null, "destinationFilePath"));
    }

    [Test]
    public void GetArtifacts_WhenDestinationFilePathIsNullOrEmpty_ThrowsArgumentException()
    {
      Assert.Throws<ArgumentException>(() => _teamCityArtifactsRepository.GetArtifacts("projectName", "projectConfigurationName", "projectConfigurationBuildId", null));
    }
  }
}
