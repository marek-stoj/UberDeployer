using System.Collections.Generic;

namespace UberDeployer.WebApp.Core.Models.Api
{
  public class ProjectViewModel
  {
    public string Name { get; set; }

    public ProjectTypeViewModel Type { get; set; }

    public List<string> AllowedEnvironmentNames { get; set; }
  }
}
