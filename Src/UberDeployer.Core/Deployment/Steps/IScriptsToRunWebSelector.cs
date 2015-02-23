using System;
using System.Collections.Generic;

namespace UberDeployer.Core.Deployment.Steps
{
  public interface IScriptsToRunWebSelector
  {
    DbScriptsToRunSelection GetSelectedScriptsToRun(Guid deploymentId, string[] sourceScriptsList);
  }
}