using System;

namespace UberDeployer.Core.Management.Db
{
  public class DbScriptRunnerException : Exception
  {
    public DbScriptRunnerException(string failedScript, Exception innerException)
      : base("Script execution failed", innerException)
    {
      FailedScript = failedScript;
    }

    public string FailedScript { get; private set; }
  }
}
