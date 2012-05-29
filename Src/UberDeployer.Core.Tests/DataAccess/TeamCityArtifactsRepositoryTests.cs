using System;
using Moq;
using NUnit.Framework;
using UberDeployer.Core.DataAccess;
using UberDeployer.Core.TeamCity;
using UberDeployer.CommonConfiguration;

namespace UberDeployer.Core.Tests.DataAccess
{
  [TestFixture]
  public class TeamCityArtifactsRepositoryTests
  {
    private Mock<ITeamCityClient> _teamCityClient;

    // SUT
    private TeamCityArtifactsRepository _teamCityArtifactsRepository;

    [SetUp]
    public void SetUp()
    {
      _teamCityClient = new Mock<ITeamCityClient>(MockBehavior.Loose);
      _teamCityArtifactsRepository = new TeamCityArtifactsRepository(_teamCityClient.Object);
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
