using System;

namespace UberDeployer.Core.Deployment.Pipeline
{
  public class DeploymentContext
  {
    private DateTime _dateStarted;
    private DateTime _dateFinished;

    public DeploymentContext(string requesterIdentity)
    {
      if (string.IsNullOrEmpty(requesterIdentity))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "requesterIdentity");
      }

      RequesterIdentity = requesterIdentity;
    }

    public string RequesterIdentity { get; private set; }

    public DateTime DateStarted
    {
      get { return _dateStarted; }
      
      internal set
      {
        if (value.Kind != DateTimeKind.Utc)
        {
          throw new ArgumentException("Value must be a UTC date.");
        }

        _dateStarted = value;
      }
    }

    public DateTime DateFinished
    {
      get { return _dateFinished; }
      
      internal set
      {
        if (value.Kind != DateTimeKind.Utc)
        {
          throw new ArgumentException("Value must be a UTC date.");
        }

        _dateFinished = value;
      }
    }

    public bool FinishedSuccessfully { get; internal set; }
  }
}
