using System;
using UberDeployer.Core;
using UberDeployer.Core.Domain;
using UberDeployer.Core.TeamCity.Models;

namespace UberDeployer.WinApp
{
  public class ProjectDeploymentInfo
  {
    public ProjectDeploymentInfo(ProjectInfo projectInfo, ProjectConfiguration projectConfiguration, ProjectConfigurationBuild projectConfigurationBuild, string targetEnvironmentName)
    {
      if (projectInfo == null)
      {
        throw new ArgumentNullException("projectInfo");
      }

      if (projectConfiguration == null)
      {
        throw new ArgumentNullException("projectConfiguration");
      }

      if (projectConfigurationBuild == null)
      {
        throw new ArgumentNullException("projectConfigurationBuild");
      }

      if (string.IsNullOrEmpty(targetEnvironmentName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "targetEnvironmentName");
      }

      ProjectInfo = projectInfo;
      ProjectConfiguration = projectConfiguration;
      ProjectConfigurationBuild = projectConfigurationBuild;
      TargetEnvironmentName = targetEnvironmentName;
    }

    public ProjectInfo ProjectInfo { get; private set; }

    public ProjectConfiguration ProjectConfiguration { get; private set; }

    public ProjectConfigurationBuild ProjectConfigurationBuild { get; private set; }

    public string TargetEnvironmentName { get; private set; }
  }
}
