using System.Text.RegularExpressions;

namespace UberDeployer.WebApp.Core.Models.Api
{
  public class ProjectConfigurationModel
  {
    private static readonly Regex _branchAndConfigurationNameRegex;

    public string ConfigurationName { get; private set; }

    public string BranchName { get; private set; }

    static ProjectConfigurationModel()
    {
      _branchAndConfigurationNameRegex = new Regex(@"^(?<branchName>\S+)( \[(?<configurationName>\S+)\])", RegexOptions.Compiled);
    }

    public ProjectConfigurationModel(string projectConfiguration)
    {
      ConfigurationName = projectConfiguration;

      var match = _branchAndConfigurationNameRegex.Match(projectConfiguration);

      if (match.Success)
      {
        ConfigurationName = match.Groups["configurationName"].Value;
        BranchName = match.Groups["branchName"].Value;
      }
    }
  }
}