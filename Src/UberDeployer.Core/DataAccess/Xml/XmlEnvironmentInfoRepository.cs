using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.DataAccess.Xml
{
  public class XmlEnvironmentInfoRepository : IEnvironmentInfoRepository
  {
    #region Nested types

    public class EnvironmentInfosXml
    {
      public List<EnvironmentInfoXml> EnvironmentInfos { get; set; }
    }

    public class EnvironmentInfoXml
    {
      public string Name { get; set; }

      public string ConfigurationTemplateName { get; set; }

      public string AppServerMachineName { get; set; }

      public string FailoverClusterMachineName { get; set; }

      public List<string> WebServerMachineNames { get; set; }

      public string TerminalServerMachineName { get; set; }
      
      public string SchedulerServerMachineName { get; set; }

      public string DatabaseServerMachineName { get; set; }

      public string NtServicesBaseDirPath { get; set; }

      public string WebAppsBaseDirPath { get; set; }

      public string SchedulerAppsBaseDirPath { get; set; }

      public string TerminalAppsBaseDirPath { get; set; }

      public bool EnableFailoverClusteringForNtServices { get; set; }

      public List<EnvironmentUserXml> EnvironmentUsers { get; set; }

      public List<AppPoolInfoXml> AppPoolInfos { get; set; }

      public List<WebAppProjectConfigurationXml> WebAppProjectConfigurations { get; set; }

      public List<ProjectToFailoverClusterGroupMappingXml> ProjectToFailoverClusterGroupMappings { get; set; }

      public string TerminalAppsShortcutFolder { get; set; }
    }

    public class EnvironmentUserXml
    {
      public string Id { get; set; }

      public string UserName { get; set; }
    }

    public class AppPoolInfoXml
    {
      public string Name { get; set; }

      public IisAppPoolVersion Version { get; set; }

      public IisAppPoolMode Mode { get; set; }
    }

    public class WebAppProjectConfigurationXml
    {
      [XmlAttribute("projectName")]
      public string ProjectName { get; set; }

      public string AppPoolId { get; set; }

      public string WebSiteName { get; set; }

      public string WebAppName { get; set; }
    }

    public class ProjectToFailoverClusterGroupMappingXml
    {
      public string ProjectName { get; set; }

      public string ClusterGroupName { get; set; }
    }

    #endregion

    private readonly string _xmlFilePath;

    private EnvironmentInfosXml _environmentInfosXml;
    private Dictionary<string, EnvironmentInfo> _environmentInfosByName;

    #region Constructor(s)

    public XmlEnvironmentInfoRepository(string xmlFilePath)
    {
      if (string.IsNullOrEmpty(xmlFilePath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "xmlFilePath");
      }

      _xmlFilePath = xmlFilePath;
    }

    #endregion

    #region IEnvironmentInfoRepository Members

    public IEnumerable<EnvironmentInfo> GetAll()
    {
      LoadXmlIfNeeded();

      return _environmentInfosByName.Values
        .OrderByDescending(ei => ei.Name); // TODO IMM HI: OrderBy instead of OrderByDescending
    }

    public EnvironmentInfo FindByName(string environmentName)
    {
      if (string.IsNullOrEmpty(environmentName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "environmentName");
      }

      LoadXmlIfNeeded();

      EnvironmentInfo environmentInfo;

      if (!_environmentInfosByName.TryGetValue(environmentName, out environmentInfo))
      {
        return null;
      }

      return environmentInfo;
    }

    #endregion

    #region Private helper methods

    private void LoadXmlIfNeeded()
    {
      if (_environmentInfosXml != null)
      {
        return;
      }

      var xmlSerializer = new XmlSerializer(typeof(EnvironmentInfosXml));

      using (var fs = File.OpenRead(_xmlFilePath))
      {
        _environmentInfosXml = (EnvironmentInfosXml)xmlSerializer.Deserialize(fs);
      }

      _environmentInfosByName =
        _environmentInfosXml.EnvironmentInfos
          .Select(
            eiXml =>
            new EnvironmentInfo(
              eiXml.Name,
              eiXml.ConfigurationTemplateName,
              eiXml.AppServerMachineName,
              eiXml.FailoverClusterMachineName,
              eiXml.WebServerMachineNames,
              eiXml.TerminalServerMachineName,
              eiXml.SchedulerServerMachineName,
              eiXml.DatabaseServerMachineName,
              eiXml.NtServicesBaseDirPath,
              eiXml.WebAppsBaseDirPath,
              eiXml.SchedulerAppsBaseDirPath,
              eiXml.TerminalAppsBaseDirPath,
              eiXml.EnableFailoverClusteringForNtServices,
              eiXml.EnvironmentUsers.Select(
                eu =>
                new EnvironmentUser(
                  eu.Id,
                  eu.UserName)),
              eiXml.AppPoolInfos.Select(
                ap =>
                new IisAppPoolInfo(
                  ap.Name,
                  ap.Version,
                  ap.Mode)),
              eiXml.WebAppProjectConfigurations.Select(
                wapc =>
                new WebAppProjectConfiguration(
                  wapc.ProjectName,
                  wapc.AppPoolId,
                  wapc.WebSiteName,
                  wapc.WebAppName)),
              eiXml.ProjectToFailoverClusterGroupMappings.Select(
                ptfcgm =>
                new ProjectToFailoverClusterGroupMapping(
                  ptfcgm.ProjectName,
                  ptfcgm.ClusterGroupName)),
              eiXml.TerminalAppsShortcutFolder
                  ))
          .ToDictionary(ei => ei.Name);
    }

    #endregion
  }
}
