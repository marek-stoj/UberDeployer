using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.ScheduledTasks;

namespace UberDeployer.Core.Deployment
{
  public static class Converter
  {
    public static RepetitionSpecification CreateRepetitionSpecification(Repetition repetition)
    {
      Guard.NotNull(repetition, "repetition");

      if (!repetition.Enabled)
      {
        return RepetitionSpecification.CreatedDisabled();
      }

      return
        RepetitionSpecification.CreateEnabled(
          repetition.Interval,
          repetition.Duration,
          repetition.StopAtDurationEnd);
    }
  }
}
