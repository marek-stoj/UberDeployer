using System;
using AutoMapper;
using UberDeployer.Common.SyntaxSugar;

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
        .Include<Core.Domain.DbProjectInfo, Proxy.Dto.DbProjectInfo>()
        .Include<Core.Domain.UberDeployerAgentProjectInfo, Proxy.Dto.UberDeployerAgentProjectInfo>();

      Mapper.CreateMap<Core.Domain.NtServiceProjectInfo, Proxy.Dto.NtServiceProjectInfo>();
      Mapper.CreateMap<Core.Domain.WebAppProjectInfo, Proxy.Dto.WebAppProjectInfo>();
      Mapper.CreateMap<Core.Domain.WebServiceProjectInfo, Proxy.Dto.WebServiceProjectInfo>();
      Mapper.CreateMap<Core.Domain.TerminalAppProjectInfo, Proxy.Dto.TerminalAppProjectInfo>();
      Mapper.CreateMap<Core.Domain.SchedulerAppProjectInfo, Proxy.Dto.SchedulerAppProjectInfo>();
      Mapper.CreateMap<Core.Domain.DbProjectInfo, Proxy.Dto.DbProjectInfo>();
      Mapper.CreateMap<Core.Domain.UberDeployerAgentProjectInfo, Proxy.Dto.UberDeployerAgentProjectInfo>();

      Mapper.CreateMap<Core.Domain.IisAppPoolInfo, Proxy.Dto.IisAppPoolInfo>();

      Mapper.CreateMap<Core.Domain.DatabaseServer, Proxy.Dto.DatabaseServer>();

      Mapper.CreateMap<Core.Domain.EnvironmentInfo, Proxy.Dto.EnvironmentInfo>();

      Mapper.CreateMap<Core.Domain.EnvironmentUser, Proxy.Dto.EnvironmentUser>();
      
      Mapper.CreateMap<Core.Domain.SchedulerAppTask, Proxy.Dto.SchedulerAppTask>();
      
      Mapper.CreateMap<Core.Domain.Repetition, Proxy.Dto.Repetition>();

      Mapper.CreateMap<Core.Domain.ProjectToFailoverClusterGroupMapping, Proxy.Dto.ProjectToFailoverClusterGroupMapping>();

      Mapper.CreateMap<Core.Domain.WebAppProjectConfigurationOverride, Proxy.Dto.WebAppProjectConfigurationOverride>();
      Mapper.CreateMap<Core.Domain.DbProjectConfigurationOverride, Proxy.Dto.DbProjectConfigurationOverride>();

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

    //TODO MARIO move to other converter?
    public static Core.Domain.DeploymentInfo ConvertDeploymentInfo(Proxy.Dto.DeploymentInfo deploymentInfo, Core.Domain.ProjectInfo projectInfo)
    {
      Core.Domain.Input.InputParams inputParams = ConvertInputParams(deploymentInfo.InputParams);

      return
        new Core.Domain.DeploymentInfo(
          deploymentInfo.DeploymentId,
          deploymentInfo.IsSimulation,
          deploymentInfo.ProjectName,
          deploymentInfo.ProjectConfigurationName,
          deploymentInfo.ProjectConfigurationBuildId,
          deploymentInfo.TargetEnvironmentName,
          inputParams);
    }

    private static Core.Domain.Input.InputParams ConvertInputParams(Proxy.Dto.Input.InputParams inputParams)
    {
      Guard.NotNull(inputParams, "inputParams");

      var dbInputParams = inputParams as Proxy.Dto.Input.DbInputParams;

      if (dbInputParams != null)
      {
        return new Core.Domain.Input.DbInputParams();
      }

      var ntServiceInputParams = inputParams as Proxy.Dto.Input.NtServiceInputParams;

      if (ntServiceInputParams != null)
      {
        return new Core.Domain.Input.NtServiceInputParams();
      }

      var schedulerAppInputParams = inputParams as Proxy.Dto.Input.SchedulerAppInputParams;

      if (schedulerAppInputParams != null)
      {
        return new Core.Domain.Input.SchedulerAppInputParams();
      }

      var terminalAppInputParams = inputParams as Proxy.Dto.Input.TerminalAppInputParams;

      if (terminalAppInputParams != null)
      {
        return new Core.Domain.Input.TerminalAppInputParams();
      }

      var webAppInputParams = inputParams as Proxy.Dto.Input.WebAppInputParams;

      if (webAppInputParams != null)
      {
        return
          new Core.Domain.Input.WebAppInputParams(
            webAppInputParams.OnlyIncludedWebMachines);
      }

      var webServiceInputParams = inputParams as Proxy.Dto.Input.WebServiceInputParams;

      if (webServiceInputParams != null)
      {
        return new Core.Domain.Input.WebServiceInputParams();
      }

      throw new NotSupportedException(string.Format("Unknown input params type: '{0}'.", inputParams.GetType().FullName));
    }
  }
}
