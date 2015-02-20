using System;
using System.Collections.Generic;

namespace UberDeployer.Core.Deployment.Steps
{
  public interface IScriptsToRunWebSelector
  {
    string[] GetSelectedScripts(string[] sourceScriptsList, Guid deploymentId);
  }
}