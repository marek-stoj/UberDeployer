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

    public List<string> SchedulerServerTasksMachineNames { get; set; }

    public List<string> SchedulerServerBinariesMachineNames { get; set; }       

    public string NtServicesBaseDirPath { get; set; }

    public string WebAppsBaseDirPath { get; set; }

    public string SchedulerAppsBaseDirPath { get; set; }

    public string TerminalAppsBaseDirPath { get; set; }

    public bool EnableFailoverClusteringForNtServices { get; set; }

    public List<EnvironmentUser> EnvironmentUsers { get; set; }

    public List<IisAppPoolInfo> AppPoolInfos { get; set; }
    
    public List<DatabaseServer> DatabaseServers { get; set; }

    public List<WebAppProjectConfiguration> WebAppProjectConfigurations { get; set; }

    public List<ProjectToFailoverClusterGroupMapping> ProjectToFailoverClusterGroupMappings { get; set; }

    public List<DbProjectConfigurationOverride> DbProjectConfigurationOverrides { get; set; }
  }
}
