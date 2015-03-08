using System;
using System.Collections.Generic;
using System.Net;

using Newtonsoft.Json.Linq;

using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.TeamCity.ApiModels;

namespace UberDeployer.Core.TeamCity
{
  public interface ITeamCityRestClient
  {
    IEnumerable<TeamCityProject> GetAllProjects();

    TeamCityProject GetProject(string projectName);

    IEnumerable<TeamCityBuildType> GetBuildTypes(string projectName);

    IEnumerable<TeamCityBuild> GetBuilds(string buildTypeId, TeamCityBuildParams teamCityBuildParams);
  }

  public class TeamCityRestClient : ITeamCityRestClient
  {
    private const string _guestAuthenticationType = "guestAuth";

    private const string _httpAuthenticationType = "httpAuth";

    private const string _basePathTemplate = "app/rest";

    private readonly string _url;

    private readonly string _basePath;

    private readonly string _userName;

    private readonly string _password;

    private readonly bool _isGuestMode;

    public TeamCityRestClient(Uri teamCityUrl)
    {
      Guard.NotNull(teamCityUrl, "teamCityUrl");

      _url = teamCityUrl.ToString().TrimEnd('/');

      _basePath = string.Format("{0}/{1}", _userName == null ? _guestAuthenticationType : _httpAuthenticationType, _basePathTemplate);

      _isGuestMode = _userName == null;
    }

    public TeamCityRestClient(Uri teamCityUrl, string userName, string password)
      :this(teamCityUrl)
    {
      Guard.NotNullNorEmpty(userName, "userName");
      Guard.NotNullNorEmpty(password, "password");

      _userName = userName;
      _password = password;
    }

    public IEnumerable<TeamCityProject> GetAllProjects()
    {
      var response = ExecuteRequest("projects");

      var projects = ParseResponse<List<TeamCityProject>>(response["project"]);

      return projects;
    }

    public TeamCityProject GetProject(string projectName)
    {
      var response = ExecuteRequest("projects/" + projectName);

      var project = ParseResponse<TeamCityProject>(response);

      return project;
    }

    public IEnumerable<TeamCityBuildType> GetBuildTypes(string projectName)
    {
      var response = ExecuteRequest(string.Format("projects/{0}/buildTypes", projectName));

      var buildTypes = ParseResponse<List<TeamCityBuildType>>(response["buildType"]);

      foreach (var buildType in buildTypes)
      {
        var branchesReponse = ExecuteRequest(string.Format("buildTypes/id:{0}/branches", buildType.Id));

        buildType.Branches = ParseResponse<List<TeamCityBuildTypeBranch>>(branchesReponse["branch"]);
      }

      return buildTypes;
    }

    public IEnumerable<TeamCityBuild> GetBuilds(string buildTypeId, TeamCityBuildParams teamCityBuildParams)
    {
      string branchLocator = string.IsNullOrWhiteSpace(teamCityBuildParams.BranchName) ? string.Empty : string.Format("locator=branch:(name:{0})&", teamCityBuildParams.BranchName);
      var response =
        ExecuteRequest(
          string.Format("buildTypes/id:{0}/builds?{1}start={2}&count={3}", buildTypeId, branchLocator, teamCityBuildParams.Skip.ToString(), teamCityBuildParams.Take.ToString()));

      var builds = ParseResponse<List<TeamCityBuild>>(response["build"]);

      return builds;
    }

    public void DownloadArtifacts(string buildId, string destinationFilePath)
    {
      Guard.NotNullNorEmpty(buildId, "buildTypeId");
      Guard.NotNullNorEmpty(destinationFilePath, "destinationFilePath");

      string authenticationType = _isGuestMode ? _guestAuthenticationType : _httpAuthenticationType;
      string requestUrl = string.Format("{0}/{1}/downloadArtifacts.html?buildId={2}", _url, authenticationType, buildId);

      using (var webClient = CreateWebClient())
      {
        webClient.DownloadFile(requestUrl, destinationFilePath);
      }
    }

    private JObject ExecuteRequest(string requestPath)
    {
      string requestUrl = CreateRequestUrl(requestPath);

      string response;
      using (var webClient = CreateWebClient())
      {
        response = webClient.DownloadString(requestUrl);
      }

      return JObject.Parse(response);
    }

    private static T ParseResponse<T>(JToken response)
      where T : class
    {
      T responseObject = response.ToObject<T>();

      if (responseObject == null)
      {
        throw new InternalException("Parsed object is null.");
      }

      return responseObject;
    }

    private string CreateRequestUrl(string requestPath)
    {
      return string.Format("{0}/{1}/{2}", _url, _basePath, requestPath);
    }

    private WebClient CreateWebClient()
    {
      var webClient =
        new Http10WebClient
        {
          Proxy = GlobalProxySelection.GetEmptyWebProxy(),
        };

      if (_isGuestMode == false)
      {
        webClient.Credentials = new NetworkCredential(_userName, _password);
      }

      webClient.Headers.Add("Accept", "application/json");

      return webClient;
    }
  }
}
