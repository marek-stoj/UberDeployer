using System;
using System.Globalization;

namespace UberDeployer.WebApp.Core.Models.Api
{
  public class ProjectConfigurationBuildViewModel
  {
    private DateTime? GetStartDateTime()
    {
      string startDateStr = StartDateStr;
      DateTime startDateTime;

      if (string.IsNullOrEmpty(startDateStr)
       || !DateTime.TryParseExact(startDateStr, "yyyyMMddTHHmmsszzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDateTime))
      {
        return null;
      }

      return startDateTime;
    }

    public string Id { get; set; }
    
    public string Number { get; set; }

    public string Status { get; set; }

    public string StartDateStr { get; set; }

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
  }
}
