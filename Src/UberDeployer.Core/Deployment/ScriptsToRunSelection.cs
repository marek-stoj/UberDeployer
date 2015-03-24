using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UberDeployer.Core.Deployment
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
