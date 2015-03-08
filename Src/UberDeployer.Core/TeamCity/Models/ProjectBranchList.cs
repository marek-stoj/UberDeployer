using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace UberDeployer.Core.TeamCity.Models
{
  public class ProjectBranchList
  {
    public ProjectBranchList()
    {
      Branches = new List<ProjectBranch>();
    }

    [JsonProperty("branch")]
    public List<ProjectBranch> Branches { get; set; }

    public int Count
    {
      get
      {
        return Branches.Count;
      }
    }

    public IEnumerable<string> GetBranches()
    {
      return Branches.Where(x => x.IsDefault == false).Select(x => x.Name);
    }

    public bool HasBranches()
    {
      return Branches.Any(x => x.IsDefault == false);
    }
  }
}