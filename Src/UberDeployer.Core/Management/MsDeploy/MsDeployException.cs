using System;

namespace UberDeployer.Core.Management.MsDeploy
{
  [Serializable]
  public class MsDeployException : Exception
  {
    #region Constructor(s)

    public MsDeployException(Exception invocationException, string consoleError = null)
      : base(CreateExceptioMessageFromInvocationException(invocationException), invocationException)
    {
      ConsoleError = consoleError != null ? consoleError.Trim() : null;
    }

    public MsDeployException(Exception invocationException)
      : this(invocationException, null)
    {
    }

    public MsDeployException(string consoleError)
      : base(CreateExceptionMessageFromConsoleError(consoleError))
    {
      ConsoleError = consoleError.Trim();
    }

    #endregion
    
    #region public proprties
    
    public string ConsoleError { get; set; }
    
    #endregion
    
    #region Private helper methods

    private static string CreateExceptioMessageFromInvocationException(Exception exception)
    {
      return string.Format("MsDeploy error: {0}.", exception.Message);
    }

    private static string CreateExceptionMessageFromConsoleError(string consoleError)
    {
      if (string.IsNullOrEmpty(consoleError))
      {
        return "Unknown error.";
      }

      string formattedConsoleError = ("  " + consoleError.Trim()).Replace("\r\n", "\r\n  ");

      return string.Format("MsDeploy error:\r\n{0}", formattedConsoleError);
    }

    #endregion
  }
}
