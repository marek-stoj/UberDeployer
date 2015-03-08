using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using UberDeployer.Core.TeamCity;
using UberDeployer.Core.TeamCity.ApiModels;

namespace UberDeployer.Core.Tests.TeamCity
{
  [TestFixture]
  [Ignore("Tests should be run manually, because external teamcity address is used.")]
  public class TeamCityRestClientTests
  {
    private TeamCityRestClient _sut;

    [TestFixtureSetUp]
    public void TestFixtureSetup()
    {
      _sut = new TeamCityRestClient(new Uri("http://teamcity.jetbrains.com"));
    }

    [Test]
    public void GetAllProjects_returns_projects()
    {
      // act
      List<TeamCityProject> projects = _sut.GetAllProjects().ToList();

      // assert
      Assert.That(projects != null);
      Assert.That(projects.Count > 1);
      Assert.That(projects.Any(x => x.Name == "Kotlin"));
    }

    [Test]
    public void GetProject_returns_specified_project()
    {
      // assign
      const string projectName = "Kotlin";

      // act
      TeamCityProject project = _sut.GetProject(projectName);

      // assert
      Assert.That(project.Id == projectName);
      Assert.That(project.Name == projectName);
    }

    [Test]
    public void GetBuildTypes_returns_project_build_types()
    {
      // act
      List<TeamCityBuildType> buildTypes = _sut.GetBuildTypes("Kotlin").ToList();

      // assert
      Assert.That(buildTypes != null);
      Assert.That(buildTypes.Count > 1);
      Assert.That(buildTypes.Any(x => x.Id == "bt345"));
    }

    [Test]
    public void GetBuildTypes_returns_build_type_branches()
    {
      // act
      List<TeamCityBuildType> buildTypes = _sut.GetBuildTypes("Kotlin").ToList();

      // assert
      Assert.That(buildTypes != null);
      Assert.That(buildTypes[0].Branches != null);
      Assert.That(buildTypes[0].Branches.Count > 1);
    }

    [Test]
    public void GetBuilds_returns_buildType_builds()
    {
      // TODO LK: write test
      // assign
      

      // act


      // assert
    }
  }
}