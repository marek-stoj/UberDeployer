using FluentNHibernate.Mapping;
using UberDeployer.Core.Deployment.Pipeline.Modules;

namespace UberDeployer.Core.DataAccess.NHibernate.Mappings
{
  public class DeploymentRequestMap : ClassMap<DeploymentRequest>
  {
    public DeploymentRequestMap()
    {
      Id(dr => dr.Id);

      Map(dr => dr.DateRequested)
        .Not.Nullable();

      Map(dr => dr.RequesterIdentity)
        .Length(256)
        .Not.Nullable();

      Map(dr => dr.ProjectName)
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
