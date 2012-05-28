using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UberDeployer.Core.DataAccess;
using UberDeployer.Core.TeamCity;
using UberDeployer.CommonConfiguration;

namespace UberDeployer.Core.Tests.DataAccess
{
  [TestFixture]
  public class TeamCityArtifactsRepositoryTests
  {
    [Test]
    public void TeamCityArtifactsRepositoryConstructor_WhenTeamCityClientIsNull_ThrowsArgumentNullException()
    {
      Assert.Throws<ArgumentNullException>(() => new TeamCityArtifactsRepository(null));
    }

    [Test]
    public void GetArtifacts_WhenProjectNameIsNullOrEmpty_ThrowsArgumentException()
    {
      ITeamCityClient teamCityClient = ObjectFactory.Instance.CreateTeamCityClient();
      var artifactsRepository = new TeamCityArtifactsRepository(teamCityClient);

      Assert.Throws<ArgumentException>(() => artifactsRepository.GetArtifacts(null, "projectConfigurationName", "projectConfigurationBuildId", "destinationFilePath"));
    }

    [Test]
    public void GetArtifacts_WhenProjectConfigurationNameIsNullOrEmpty_ThrowsArgumentException()
    {
      ITeamCityClient teamCityClient = ObjectFactory.Instance.CreateTeamCityClient();
      var artifactsRepository = new TeamCityArtifactsRepository(teamCityClient);

      Assert.Throws<ArgumentException>(() => artifactsRepository.GetArtifacts("projectName", null, "projectConfigurationBuildId", "destinationFilePath"));
    }

    [Test]
    public void GetArtifacts_WhenProjectConfigurationBuildIdIsNullOrEmpty_ThrowsArgumentException()
    {
      ITeamCityClient teamCityClient = ObjectFactory.Instance.CreateTeamCityClient();
      var artifactsRepository = new TeamCityArtifactsRepository(teamCityClient);

      Assert.Throws<ArgumentException>(() => artifactsRepository.GetArtifacts("projectName", "projectConfigurationName", null, "destinationFilePath"));
    }

    [Test]
    public void GetArtifacts_WhenDestinationFilePathIsNullOrEmpty_ThrowsArgumentException()
    {
      ITeamCityClient teamCityClient = ObjectFactory.Instance.CreateTeamCityClient();
      var artifactsRepository = new TeamCityArtifactsRepository(teamCityClient);

      Assert.Throws<ArgumentException>(() => artifactsRepository.GetArtifacts("projectName", "projectConfigurationName", "projectConfigurationBuildId", null));
    }
  }
}
