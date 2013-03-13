using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Domain.InputParameters;

namespace UberDeployer.Core.Tests.Generators
{
  public static class DeploymentInfoGenerator
  {
    public static DeploymentInfo GetNtServiceDeploymentInfo(bool areEnvironmentSpecific = false)
    {
      return GetDeploymentInfo(
        ProjectInfoGenerator.GetNtServiceProjectInfo(areEnvironmentSpecific),
        new NtServiceInputParams());
    }

    public static DeploymentInfo GetWebServiceDeploymentInfo()
    {
      return GetDeploymentInfo(
        ProjectInfoGenerator.GetWebServiceProjectInfo(),
        new WebServiceInputParams());
    }

    public static DeploymentInfo GetWebAppDeploymentInfo()
    {
      return GetDeploymentInfo(
        ProjectInfoGenerator.GetWebAppProjectInfo(),
        new WebAppInputParams());
    }

    public static DeploymentInfo GetDbDeploymentInfo(bool areEnvironmentSpecific = false)
    {
      return GetDeploymentInfo(
        ProjectInfoGenerator.GetDbProjectInfo(areEnvironmentSpecific),
        new DbInputParams());
    }

    public static DeploymentInfo GetTerminalAppDeploymentInfo()
    {
      return GetDeploymentInfo(
        ProjectInfoGenerator.GetTerminalAppProjectInfo(),
        new TerminalAppInputParams());
    }

    public static DeploymentInfo GetSchedulerAppDeploymentInfo()
    {
      return GetDeploymentInfo(
        ProjectInfoGenerator.GetSchedulerAppProjectInfo(),
        new SchedulerAppInputParams());
    }

    private static DeploymentInfo GetDeploymentInfo(ProjectInfo projectInfo, InputParams inputParams)
    {
      return new DeploymentInfo(
        "project_name",
        "project_configuration_name",
        "project_configuration_build_id",
        "target_environment_name",
        projectInfo,
        inputParams);
    }
  }
}
