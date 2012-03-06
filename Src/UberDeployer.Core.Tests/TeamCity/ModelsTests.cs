using System.Collections.Generic;
using NUnit.Framework;
using UberDeployer.Core.TeamCity.Models;

namespace UberDeployer.Core.Tests.TeamCity
{
  [TestFixture]
  public class ModelsTests
  {
    #region Project

    [Test]
    public void Project_ToString_ReturnCorrectlyFormatedValue()
    {
      string id = "id";
      string name = "name";
      string href = "href";
      string expectedResult = string.Format("Id: {0}, Name: {1}, Href: {2}", id, name, href);

      var project = new Project
      {
        Id = id,
        Name = name,
        Href = href
      };

      Assert.AreEqual(expectedResult, project.ToString());
    }

    #endregion Project

    #region ProjectConfiguration

    [Test]
    public void ProjectConfiguration_ToString_ReturnCorrectlyFormatedValue()
    {
      string id = "1";
      string name = "2";
      string href = "3";
      string webUrl = "4";
      string projectId = "5";
      string projectName = "6";
      string expectedResult = string.Format(
                                            "Id: {0}, Name: {1}, Href: {2}, WebUrl: {3}, ProjectId: {4}, ProjectName: {5}",
                                            id,
                                            name,
                                            href,
                                            webUrl,
                                            projectId,
                                            projectName);

      var projectConfiguration = new ProjectConfiguration
                                    {
                                      Id = id,
                                      Name = name,
                                      Href = href,
                                      WebUrl = webUrl,
                                      ProjectId = projectId,
                                      ProjectName = projectName
                                    };

      Assert.AreEqual(expectedResult, projectConfiguration.ToString());
    }

    #endregion ProjectConfiguration

    #region ProjectConfigurationBuild

    [Test]
    public void ProjectConfigurationBuild_ToString_ReturnCorrectlyFormatedValue()
    {
      string id = "id";
      string buildTypeId = "buildTypeId";
      string number = "number";
      BuildStatus status = BuildStatus.Success;
      string webUrl = "webUrl";
      string expectedResult = string.Format("Id: {0}, BuildTypeId: {1}, Number: {2}, Status: {3}, WebUrl: {4}",
                                            id,
                                            buildTypeId,
                                            number,
                                            status,
                                            webUrl);

      var projectConfigurationBuild = new ProjectConfigurationBuild
      {
        Id = id,
        BuildTypeId = buildTypeId,
        Number = number,
        Status = status,
        WebUrl = webUrl
      };

      Assert.AreEqual(expectedResult, projectConfigurationBuild.ToString());
    }

    #endregion ProjectConfigurationBuild

    #region ProjectConfigurationBuildsList

    [Test]
    public void ProjectConfigurationBuildsList_ToString_ReturnCorrectlyFormatedValue()
    {
      List<ProjectConfigurationBuild> builds = new List<ProjectConfigurationBuild>();
      string expectedResult = string.Format("BuildsCount: {0}", builds != null ? builds.Count : 0);

      var projectConfigurationBuildsList = new ProjectConfigurationBuildsList
      {
        Builds = builds
      };

      Assert.AreEqual(expectedResult, projectConfigurationBuildsList.ToString());
    }

    #endregion ProjectConfigurationBuildsList

    #region ProjectConfigurationsList

    [Test]
    public void ProjectConfigurationsList_ToString_ReturnCorrectlyFormatedValue()
    {
      List<ProjectConfiguration> configurations = new List<ProjectConfiguration>();
      string expectedResult = string.Format("ConfigurationsCount: {0}", configurations != null ? configurations.Count : 0);

      var projectConfigurationsList = new ProjectConfigurationsList
      {
        Configurations = configurations
      };

      Assert.AreEqual(expectedResult, projectConfigurationsList.ToString());
    }

    #endregion ProjectConfigurationsList

    #region ProjectDetails

    [Test]
    public void ProjectDetails_ToString_ReturnCorrectlyFormatedValue()
    {
      string projectId = "projectId";
      string projectName = "projectName";
      string projectHref = "projectHref";
      string projectWebUrl = "projectWebUrl";
      bool isProjectArchived = true;
      ProjectConfigurationsList configurationsList = new ProjectConfigurationsList
                                                      {
                                                        Configurations = new List<ProjectConfiguration>()
                                                      };

      string expectedResult = string.Format(
                                            "ProjectId: {0}, ProjectName: {1}, ProjectHref: {2}, ProjectWebUrl: {3}, IsProjectArchived: {4}, ConfigurationsCount: {5}",
                                            projectId,
                                            projectName,
                                            projectHref,
                                            projectWebUrl,
                                            isProjectArchived,
                                            configurationsList != null && configurationsList.Configurations != null ? configurationsList.Configurations.Count : 0);

      var projectDetails = new ProjectDetails
      {
        ProjectId = projectId,
        ProjectName = projectName,
        ProjectHref = projectHref,
        ProjectWebUrl = projectWebUrl,
        IsProjectArchived = isProjectArchived,
        ConfigurationsList = configurationsList
      };

      Assert.AreEqual(expectedResult, projectDetails.ToString());
    }

    #endregion ProjectDetails

    #region ProjectsList

    [Test]
    public void ProjectsList_ToString_ReturnCorrectlyFormatedValue()
    {
      List<Project> projects = new List<Project>();
      string expectedResult = string.Format("ProjectsCount: {0}", projects != null ? projects.Count : 0);

      var projectsList = new ProjectsList
      {
        Projects = projects
      };

      Assert.AreEqual(expectedResult, projectsList.ToString());
    }

    #endregion ProjectsList
  }
}
