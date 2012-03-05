namespace UberDeployer.Core.TeamCity.Models
{
  public enum BuildStatus
  {
    /// <summary>
    /// Build and tests (or other tasks) succeeded.
    /// </summary>
    Success,

    /// <summary>
    /// Build succeeded but tests (or other tasks) didn't pass.
    /// </summary>
    Failure,

    /// <summary>
    /// Build failed.
    /// </summary>
    Error,
  }
}
