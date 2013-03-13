using System.Collections.Generic;

namespace UberDeployer.Agent.Proxy.Dto
{
  public class EnvironmentInfo
  {
    public string Name { get; set; }

    public string ConfigurationTemplateName { get; set; }

    public string AppServerMachineName { get; set; }

    public string FailoverClusterMachineName { get; set; }

    public List<string> WebServerMachineNames { get; set; }

    public string TerminalServerMachineName { get; set; }

    public string DatabaseServerMachineName { get; set; }

    public string NtServicesBaseDirPath { get; set; }

    public string WebAppsBaseDirPath { get; set; }

    public string SchedulerAppsBaseDirPath { get; set; }

    public string TerminalAppsBaseDirPath { get; set; }

    public bool EnableFailoverClusteringForNtServices { get; set; }

    public List<EnvironmentUser> EnvironmentUsers { get; set; }

    public List<IisAppPoolInfo> AppPoolInfos { get; set; }

    public List<ProjectToWebSiteMapping> ProjectToWebSiteMappings { get; set; }

    public List<ProjectToAppPoolMapping> ProjectToAppPoolMappings { get; set; }

    public List<ProjectToFailoverClusterGroupMapping> ProjectToFailoverClusterGroupMappings { get; set; }
  }
}
