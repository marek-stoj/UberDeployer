using System;
using System.Globalization;
using UberDeployer.Agent.Proxy.Dto.TeamCity;

namespace UberDeployer.WinApp.ViewModels
{
  public class ProjectConfigurationBuildInListViewModel
  {
    public ProjectConfigurationBuild ProjectConfigurationBuild { get; set; }

    private DateTime? GetStartDateTime()
    {
      string startDateStr = ProjectConfigurationBuild.StartDate;
      DateTime startDateTime;

      if (string.IsNullOrEmpty(startDateStr)
       || !DateTime.TryParseExact(startDateStr, "yyyyMMddTHHmmsszzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDateTime))
      {
        return null;
      }

      return startDateTime;
    }

    public string Id
    {
      get { return ProjectConfigurationBuild.Id; }
    }

    public string Number
    {
      get { return ProjectConfigurationBuild.Number; }
    }

    public string StartDate
    {
      get
      {
        DateTime? startDateTime = GetStartDateTime();

        return startDateTime.HasValue ? startDateTime.Value.ToShortDateString() : "?";
      }
    }

    public string StartTime
    {
      get
      {
        DateTime? startDateTime = GetStartDateTime();

        return startDateTime.HasValue ? startDateTime.Value.ToShortTimeString() : "?";
      }
    }

    public string Status
    {
      get { return ProjectConfigurationBuild.Status.ToString(); }
    }
  }
}
