using FluentNHibernate.Mapping;
using UberDeployer.Core.Deployment.Pipeline.Modules;

namespace UberDeployer.Core.DataAccess.NHibernate.Mappings
{
  public sealed class DeploymentRequestMap : ClassMap<DeploymentRequest>
  {
    public DeploymentRequestMap()
    {
      Id(dr => dr.Id);

      Map(dr => dr.DateStarted)
        .Not.Nullable();

      Map(dr => dr.DateFinished)
        .Not.Nullable();

      Map(dr => dr.RequesterIdentity)
        .Length(256)
        .Not.Nullable();

      Map(dr => dr.ProjectName)
        .Length(256)
        .Not.Nullable();

      Map(dr => dr.ProjectConfigurationName)
        .Length(256)
        .Not.Nullable();

      Map(dr => dr.ProjectConfigurationBuildId)
        .Length(256)
        .Not.Nullable();

      Map(dr => dr.TargetEnvironmentName)
        .Length(64)
        .Not.Nullable();

      Map(dr => dr.FinishedSuccessfully)
        .Not.Nullable();
    }
  }
}
