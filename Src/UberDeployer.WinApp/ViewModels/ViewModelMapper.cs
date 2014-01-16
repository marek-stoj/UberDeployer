using AutoMapper;
using UberDeployer.Agent.Proxy.Dto;
using UberDeployer.WinApp.ViewModels.PropertyGrids;

namespace UberDeployer.WinApp.ViewModels
{
  public static class ViewModelMapper
  {
    static ViewModelMapper()
    {
      Mapper.CreateMap<ProjectInfo, ProjectInfoInPropertyGridViewModel>()
        .Include<NtServiceProjectInfo, NtServiceProjectInfoInPropertyGridViewModel>()
        .Include<WebAppProjectInfo, WebAppProjectInfoInPropertyGridViewModel>()
        .Include<WebServiceProjectInfo, WebServiceProjectInfoInPropertyGridViewModel>()
        .Include<TerminalAppProjectInfo, TerminalAppProjectInfoInPropertyGridViewModel>()
        .Include<SchedulerAppProjectInfo, SchedulerAppProjectInfoInPropertyGridViewModel>()
        .Include<DbProjectInfo, DbProjectInfoInPropertyGridViewModel>()
        .Include<UberDeployerAgentProjectInfo, UberDeployerAgentProjectInfoInPropertyGridViewModel>();

      Mapper.CreateMap<NtServiceProjectInfo, NtServiceProjectInfoInPropertyGridViewModel>();
      Mapper.CreateMap<WebAppProjectInfo, WebAppProjectInfoInPropertyGridViewModel>();
      Mapper.CreateMap<WebServiceProjectInfo, WebServiceProjectInfoInPropertyGridViewModel>();
      Mapper.CreateMap<TerminalAppProjectInfo, TerminalAppProjectInfoInPropertyGridViewModel>();
      Mapper.CreateMap<SchedulerAppProjectInfo, SchedulerAppProjectInfoInPropertyGridViewModel>();
      Mapper.CreateMap<DbProjectInfo, DbProjectInfoInPropertyGridViewModel>();
      Mapper.CreateMap<UberDeployerAgentProjectInfo, UberDeployerAgentProjectInfoInPropertyGridViewModel>();

      Mapper.CreateMap<EnvironmentInfo, EnvironmentInfoInPropertyGridViewModel>();
      Mapper.CreateMap<EnvironmentUser, EnvironmentUserInPropertyGridVieModel>();
      Mapper.CreateMap<ProjectToFailoverClusterGroupMapping, ProjectToFailoverClusterGroupMappingInPropertyGridViewModel>();
    }

    public static TResult Map<TInput, TResult>(TInput input)
    {
      return Mapper.Map<TInput, TResult>(input);
    }
  }
}
