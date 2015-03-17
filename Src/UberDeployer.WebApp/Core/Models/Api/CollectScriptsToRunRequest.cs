using System;

namespace UberDeployer.WebApp.Core.Models.Api
{
  public class CollectScriptsToRunRequest
  {
    public Guid? DeploymentId { get; set; }

    public string[] ScriptsToRun { get; set; }
  }
}