using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Core.Domain
{
  public class DbProjectConfigurationOverride
  {
    public DbProjectConfigurationOverride(string projectName, string databaseServerId = null)
    {
      Guard.NotNullNorEmpty(projectName, "projectName");

      ProjectName = projectName;
      DatabaseServerId = databaseServerId;
    }

    public string ProjectName { get; private set; }

    public string DatabaseServerId { get; private set; }
  }
}
