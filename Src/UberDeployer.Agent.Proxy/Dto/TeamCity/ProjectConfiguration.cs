using System.Collections.Generic;
using System.Linq;

namespace UberDeployer.Agent.Proxy.Dto.TeamCity
{
  public class ProjectConfiguration
  {
    public ProjectConfiguration()
    {
      Branches = new List<ProjectConfigurationBranch>();
    }

    public string Id { get; set; }

    public string Name { get; set; }

    public string ProjectName { get; set; }

    public string ProjectId { get; set; }

    public List<ProjectConfigurationBranch> Branches { get; set; }

    public bool HasBranches()
    {
      return Branches.Any(x => !x.IsDefault);
    }

    public IEnumerable<string> GetBranches()
    {
      return Branches.Where(x => x.IsDefault == false).Select(x => x.Name);
    }
  }
}
