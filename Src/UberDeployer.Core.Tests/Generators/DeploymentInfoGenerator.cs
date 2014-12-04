using System;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Domain.Input;

namespace UberDeployer.Core.Tests.Generators
{
  public static class DeploymentInfoGenerator
  {
    public static DeploymentInfo GetNtServiceDeploymentInfo()
    {
      return GetDeploymentInfo(new NtServiceInputParams());
    }

    public static DeploymentInfo GetWebServiceDeploymentInfo()
    {
      return GetDeploymentInfo(new WebServiceInputParams());
    }

    public static DeploymentInfo GetWebAppDeploymentInfo()
    {
      return GetDeploymentInfo(new WebAppInputParams());
    }

    public static DeploymentInfo GetDbDeploymentInfo(bool areEnvironmentSpecific = false)
    {
      return GetDeploymentInfo(new DbInputParams());
    }

    public static DeploymentInfo GetExtensionProjectDeploymentInfo()
    {
      return GetDeploymentInfo(new ExtensionInputParams());
    }

    public static DeploymentInfo GetTerminalAppDeploymentInfo()
    {
      return GetDeploymentInfo(new TerminalAppInputParams());
    }

    public static DeploymentInfo GetSchedulerAppDeploymentInfo()
    {
      return GetDeploymentInfo(new SchedulerAppInputParams());
    }

    private static DeploymentInfo GetDeploymentInfo(InputParams inputParams)
    {
      return
        new DeploymentInfo(
          Guid.NewGuid(),
          false,
          "project_name",
          "project_configuration_name",
          "project_configuration_build_id",
          "target_environment_name",
          inputParams);
    }
  }
}
