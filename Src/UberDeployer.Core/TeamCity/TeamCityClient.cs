using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.TeamCity.Models;
using ProjectInfo = UberDeployer.Core.TeamCity.Models.Project;

namespace UberDeployer.Core.TeamCity
{
  public class TeamCityClient : ITeamCityClient
  {
    private readonly string _teamCityBasePath;

    private readonly bool _isAuthenticationRequired;

    private const string _RestApiPathTemplate_DownloadArtifacts = "/httpAuth/downloadArtifacts.html?buildId=${buildId}";

    private readonly string _hostName;

    private readonly int _port;

    private readonly string _userName;

    private readonly string _password;

    public TeamCityClient(string hostName, int port, string userName, string password)
    {
      Guard.NotNullNorEmpty(hostName, "hostName");
      Guard.NotNullNorEmpty(userName, "userName");
      Guard.NotNullNorEmpty(password, "password");

      if (port <= 0)
      {
        throw new ArgumentException("Argument must be greater than 0.", "port");
      }

      _hostName = hostName;
      _port = port;
      _userName = userName;
      _password = password;

      _teamCityBasePath = "/httpAuth/app/rest/";
      _isAuthenticationRequired = true;
    }

    public TeamCityClient(string hostName, int port)
    {
      Guard.NotNullNorEmpty(hostName, "hostName");

      if (port <= 0)
      {
        throw new ArgumentException("Argument must be greater than 0.", "port");
      }

      _hostName = hostName;
      _port = port;

      _teamCityBasePath = "guestAuth/app/rest";
      _isAuthenticationRequired = false;
    }

    public IEnumerable<Project> GetAllProjects()
    {
      var projectsList = ExecuteWebRequest<ProjectsList>(CreateRestApiPath("projects"));

      if (projectsList.Projects == null)
      {
        throw new InternalException("'Projects property should never be null here.");
      }

      return projectsList.Projects;
    }

    public Project GetProjectByName(string projectName)
    {
      var project = ExecuteWebRequest<Project>(CreateRestApiPath("projects/" + projectName));

      return project;
    }

    public ProjectDetails GetProjectDetails(Project project)
    {
      Guard.NotNull(project, "project");

      var projectDetails = ExecuteWebRequest<ProjectDetails>(project.Href);

      return projectDetails;
    }

    public ProjectConfigurationDetails GetProjectConfigurationDetails(ProjectConfiguration projectConfiguration)
    {
      Guard.NotNull(projectConfiguration, "projectConfiguration");

      var projectConfigurationDetails = ExecuteWebRequest<ProjectConfigurationDetails>(projectConfiguration.Href);

      projectConfigurationDetails.Branches = ExecuteWebRequest<ProjectBranchList>(projectConfiguration.Href + "/branches");

      return projectConfigurationDetails;
    }

    public ProjectConfigurationBuildsList GetProjectConfigurationBuilds(ProjectConfigurationDetails projectConfigurationDetails, int startIndex, int maxCount)
    {
      Guard.NotNull(projectConfigurationDetails, "projectConfigurationDetails");

      string restApiPath =
        string.Format(
          "{0}?start={1}&count={2}",
          projectConfigurationDetails.BuildsLocation.Href,
          startIndex,
          maxCount);

      var projectConfigurationBuildsList = ExecuteWebRequest<ProjectConfigurationBuildsList>(restApiPath);

      return projectConfigurationBuildsList;
    }

    public void DownloadArtifacts(ProjectConfigurationBuild projectConfigurationBuild, string destinationFilePath)
    {
      Guard.NotNull(projectConfigurationBuild, "projectConfigurationBuild");
      Guard.NotNullNorEmpty(destinationFilePath, "destinationFilePath");

      string apiUrl = _RestApiPathTemplate_DownloadArtifacts.Replace("${buildId}", projectConfigurationBuild.Id);

      DownloadDataViaRestApi(apiUrl, destinationFilePath);
    }

    private T ExecuteWebRequest<T>(string restApiPath) where T : class
    {
      string response = DownloadStringViaRestApi(restApiPath);

      return ParseResponse<T>(response);
    }

    private static T ParseResponse<T>(string response)
      where T : class
    {
      T responseObject = JsonConvert.DeserializeObject<T>(response);

      if (responseObject == null)
      {
        throw new InternalException("Parsed object is null.");
      }

      return responseObject;
    }

    private string DownloadStringViaRestApi(string restApiPath)
    {
      string restApiUrl = CreateRestApiUrl(restApiPath);

      using (var webClient = CreateWebClient())
      {
        return webClient.DownloadString(restApiUrl);
      }
    }

    private void DownloadDataViaRestApi(string restApiPath, string destinationFilePath)
    {
      string restApiUrl = CreateRestApiUrl(restApiPath);

      using (var webClient = CreateWebClient())
      {
        webClient.DownloadFile(restApiUrl, destinationFilePath);
      }
    }

    private WebClient CreateWebClient()
    {
      var webClient =
        new Http10WebClient
        {
          Proxy = GlobalProxySelection.GetEmptyWebProxy(),
        };

      if (_isAuthenticationRequired)
      {
        webClient.Credentials = new NetworkCredential(_userName, _password);
      }

      webClient.Headers.Add("Accept", "application/json");

      return webClient;
    }

    private string CreateRestApiPath(string resourceName)
    {
      return _teamCityBasePath + resourceName;
    }

    private string CreateRestApiUrl(string restApiPath)
    {
      return
        string.Format(
          "http://{0}:{1}{2}",
          _hostName,
          _port,
          restApiPath);
    }
  }
}
