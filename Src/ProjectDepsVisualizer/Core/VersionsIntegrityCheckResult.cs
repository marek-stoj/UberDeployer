namespace ProjectDepsVisualizer.Core
{
  public class VersionsIntegrityCheckResult
  {
    public VersionsIntegrityCheckResult(bool areVersionsIntegral)
    {
      AreVersionsIntegral = areVersionsIntegral;
    }

    public bool AreVersionsIntegral { get; private set; }
  }
}
