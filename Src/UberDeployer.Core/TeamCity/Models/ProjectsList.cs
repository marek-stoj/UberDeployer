using System.Collections.Generic;
using Newtonsoft.Json;

namespace UberDeployer.Core.TeamCity.Models
{
  public class ProjectsList
  {
    #region Overrides of object

    public override string ToString()
    {
      return string.Format("ProjectsCount: {0}", Projects != null ? Projects.Count : 0);
    }

    #endregion

    #region Properties

    [JsonProperty("project")]
    public List<Project> Projects { get; set; }

    #endregion
  }
}
