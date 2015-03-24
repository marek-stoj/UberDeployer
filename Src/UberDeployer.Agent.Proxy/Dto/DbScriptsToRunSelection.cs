namespace UberDeployer.Agent.Proxy.Dto
{
  public class DbScriptsToRunSelection
  {
    public string[] SelectedScripts { get; set; }

    public DatabaseScriptToRunSelectionType DatabaseScriptToRunSelectionType { get; set; }
  }

  public enum DatabaseScriptToRunSelectionType
  {
    LastVersion,
    Multiselect
  }
}
