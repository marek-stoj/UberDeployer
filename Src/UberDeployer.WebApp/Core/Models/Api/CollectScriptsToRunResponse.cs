using System;

namespace UberDeployer.WebApp.Core.Models.Api
{
  public class CollectScriptsToRunResponse
  {
    public Guid? DeploymentId { get; set; }

    public string[] SelectedScripts { get; set; }

    public bool IsMultiselect { get; set; }
  }
}