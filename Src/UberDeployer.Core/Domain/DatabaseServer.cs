using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Core.Domain
{
  public class DatabaseServer
  {
    public DatabaseServer(string id, string machineName)
    {
      Guard.NotNullNorEmpty(id, "id");
      Guard.NotNullNorEmpty(machineName, "machineName");

      Id = id;
      MachineName = machineName;
    }

    public string Id { get; private set; }

    public string MachineName { get; private set; }
  }
}
