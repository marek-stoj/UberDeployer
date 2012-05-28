using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using UberDeployer.Core.TeamCity.Models;
using ProjectInfo = UberDeployer.Core.TeamCity.Models.Project;

namespace UberDeployer.Core.TeamCity
{
  public class TeamCityClient : ITeamCityClient
  {
    private const string _RestApiPath_GetProjects = "/httpAuth/app/rest/projects";
    private const string _RestApiPathTemplate_DownloadArtifacts = "/httpAuth/downloadArtifacts.html?buildId=${buildId}";

    private readonly string _hostName;
    private readonly int _port;
    private readonly string _userName;
    private readonly string _password;

    #region Constructor(s)

    public TeamCityClient(string hostName, int port, string userName, string password)
    {
      if (string.IsNullOrEmpty(hostName)) throw new ArgumentException("Argument can't be null nor empty.", "hostName");
      if (port <= 0) throw new ArgumentException("Argument must be greater than 0.", "port");
      if (string.IsNullOrEmpty(userName)) throw new ArgumentException("Argument can't be null nor empty.", "userName");
      if (string.IsNullOrEmpty(password)) throw new ArgumentException("Argument can't be null nor empty.", "password");

      _hostName = hostName;
      _port = port;
      _userName = userName;
      _password = password;
    }

    #endregion

    #region ITeamCityClient Members

    public IEnumerable<Project> GetAllProjects()
    {
      string response = DownloadStringViaRestApi(_RestApiPath_GetProjects);
      ProjectsList projectsList = ParseResponse<ProjectsList>(response);

      if (projectsList.Projects == null)
      {
        throw new InternalException("'Projects property should never be null here.");
      }
      
      return projectsList.Projects;
    }

    // TODO IMM HI: optimize?
    public Project GetProjectByName(string projectName)
    {
      IEnumerable<Project> projects = GetAllProjects();

      return projects.SingleOrDefault(pi => pi.Name == projectName);
    }

    public ProjectDetails GetProjectDetails(Project project)
    {
      if (project == null) throw new ArgumentNullException("project");

      string response = DownloadStringViaRestApi(project.Href);
      ProjectDetails projectDetails = ParseResponse<ProjectDetails>(response);

      return projectDetails;
    }

    public ProjectConfigurationDetails GetProjectConfigurationDetails(ProjectConfiguration projectConfiguration)
    {
      if (projectConfiguration == null) throw new ArgumentNullException("projectConfiguration");

      string response = DownloadStringViaRestApi(projectConfiguration.Href);
      ProjectConfigurationDetails projectConfigurationDetails = ParseResponse<ProjectConfigurationDetails>(response);

      return projectConfigurationDetails;
    }

    public ProjectConfigurationBuildsList GetProjectConfigurationBuilds(ProjectConfigurationDetails projectConfigurationDetails, int startIndex, int maxCount)
    {
      if (projectConfigurationDetails == null) throw new ArgumentNullException("projectConfigurationDetails");

      string restApiPath =
        string.Format(
          "{0}?start={1}&count={2}",
          projectConfigurationDetails.BuildsLocation.Href,
          startIndex,
          maxCount);

      string response = DownloadStringViaRestApi(restApiPath);
      
      ProjectConfigurationBuildsList projectConfigurationBuildsList =
        ParseResponse<ProjectConfigurationBuildsList>(response);

      return projectConfigurationBuildsList;
    }

    public void DownloadArtifacts(ProjectConfigurationBuild projectConfigurationBuild, string destinationFilePath)
    {
      if (projectConfigurationBuild == null) throw new ArgumentNullException("projectConfigurationBuild");
      if (string.IsNullOrEmpty(destinationFilePath)) throw new ArgumentException("Argument can't be null nor empty.", "destinationFilePath");

      string apiUrl = _RestApiPathTemplate_DownloadArtifacts.Replace("${buildId}", projectConfigurationBuild.Id);
      
      DownloadDataViaRestApi(apiUrl, destinationFilePath);
    }

    #endregion

    #region REST API helpers

    private static T ParseResponse<T>(string response)
      where T : class
    {
      if (string.IsNullOrEmpty(response)) throw new ArgumentException("Argument can't be null nor empty.", "response");

      T responseObject = JsonConvert.DeserializeObject<T>(response);

      if (responseObject == null)
      {
        throw new InternalException("Parsed object is null.");
      }

      return responseObject;
    }

    private WebClient CreateWebClient()
    {
      // ReSharper disable CSharpWarnings::CS0612
      var webClient =
        new WebClient
          {
            Credentials = new NetworkCredential(_userName, _password),
            Proxy = GlobalProxySelection.GetEmptyWebProxy(),
          };
      // ReSharper restore CSharpWarnings::CS0612

      webClient.Headers.Add("Accept", "application/json");

      return webClient;
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

    #endregion
  }
}
