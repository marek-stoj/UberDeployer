using System;
using System.Collections.Generic;

namespace UberDeployer.Core.Deployment.Steps
{
  public interface IScriptsToRunWebSelector
  {
    string[] GetSelectedScriptsToRun(Guid deploymentId, string[] sourceScriptsList);
  }
}