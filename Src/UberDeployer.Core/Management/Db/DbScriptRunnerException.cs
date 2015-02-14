using System;

namespace UberDeployer.Core.Management.Db
{
  public class DbScriptRunnerException : Exception
  {
    public DbScriptRunnerException(string failedScript)
      : this(failedScript, null)
    {
    }

    public DbScriptRunnerException(string failedScript, Exception innerException)
      : base(failedScript, innerException)
    {
    }
  }
}