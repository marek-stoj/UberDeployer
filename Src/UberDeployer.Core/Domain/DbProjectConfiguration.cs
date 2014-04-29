using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Core.Domain
{
  public class DbProjectConfiguration
  {
    public DbProjectConfiguration(string projectName, string databaseServerId)
    {
      Guard.NotNullNorEmpty(projectName, "projectName");
      Guard.NotNullNorEmpty(databaseServerId, "databaseServerId");

      ProjectName = projectName;
      DatabaseServerId = databaseServerId;
    }

    public string ProjectName { get; private set; }

    public string DatabaseServerId { get; private set; }
  }
}
