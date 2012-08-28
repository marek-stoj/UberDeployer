using AutoMapper;

namespace UberDeployer.Agent.Service
{
  public static class DtoMapper
  {
    static DtoMapper()
    {
      Mapper.CreateMap<Core.Domain.ProjectInfo, Proxy.Dto.ProjectInfo>()
        .Include<Core.Domain.NtServiceProjectInfo, Proxy.Dto.NtServiceProjectInfo>()
        .Include<Core.Domain.WebAppProjectInfo, Proxy.Dto.WebAppProjectInfo>()
        .Include<Core.Domain.WebServiceProjectInfo, Proxy.Dto.WebServiceProjectInfo>()
        .Include<Core.Domain.TerminalAppProjectInfo, Proxy.Dto.TerminalAppProjectInfo>()
        .Include<Core.Domain.SchedulerAppProjectInfo, Proxy.Dto.SchedulerAppProjectInfo>()
        .Include<Core.Domain.DbProjectInfo, Proxy.Dto.DbProjectInfo>();

      Mapper.CreateMap<Core.Domain.NtServiceProjectInfo, Proxy.Dto.NtServiceProjectInfo>();
      Mapper.CreateMap<Core.Domain.WebAppProjectInfo, Proxy.Dto.WebAppProjectInfo>();
      Mapper.CreateMap<Core.Domain.WebServiceProjectInfo, Proxy.Dto.WebServiceProjectInfo>();
      Mapper.CreateMap<Core.Domain.TerminalAppProjectInfo, Proxy.Dto.TerminalAppProjectInfo>();
      Mapper.CreateMap<Core.Domain.SchedulerAppProjectInfo, Proxy.Dto.SchedulerAppProjectInfo>();
      Mapper.CreateMap<Core.Domain.DbProjectInfo, Proxy.Dto.DbProjectInfo>();

      Mapper.CreateMap<Core.Domain.IisAppPoolInfo, Proxy.Dto.IisAppPoolInfo>();

      Mapper.CreateMap<Core.Domain.EnvironmentInfo, Proxy.Dto.EnvironmentInfo>();

      Mapper.CreateMap<Core.Domain.EnvironmentUser, Proxy.Dto.EnvironmentUser>();

      Mapper.CreateMap<Core.Domain.ProjectToFailoverClusterGroupMapping, Proxy.Dto.ProjectToFailoverClusterGroupMapping>();

      Mapper.CreateMap<Core.TeamCity.Models.ProjectConfiguration, Proxy.Dto.TeamCity.ProjectConfiguration>();

      Mapper.CreateMap<Core.TeamCity.Models.ProjectConfigurationBuild, Proxy.Dto.TeamCity.ProjectConfigurationBuild>();

      Mapper.CreateMap<Core.Deployment.Pipeline.Modules.DeploymentRequest, Proxy.Dto.DeploymentRequest>();

      Mapper.CreateMap<Core.Deployment.DiagnosticMessage, Proxy.Dto.DiagnosticMessage>();
      Mapper.CreateMap<Core.Deployment.DiagnosticMessageType, Proxy.Dto.DiagnosticMessageType>();

      Mapper.AssertConfigurationIsValid();
    }

    public static TResult Map<TInput, TResult>(TInput input)
    {
      return Mapper.Map<TInput, TResult>(input);
    }
  }
}
