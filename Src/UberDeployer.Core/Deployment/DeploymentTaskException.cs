using System;

namespace UberDeployer.Core.Deployment
{
  [Serializable]
  public class DeploymentTaskException : Exception
  {
    #region Constructor(s)

    public DeploymentTaskException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public DeploymentTaskException(string message)
      : this(message, null)
    {
    }

    #endregion
  }
}
