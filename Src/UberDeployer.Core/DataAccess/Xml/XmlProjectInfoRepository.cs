using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UberDeployer.Core.Domain;

namespace UberDeployer.Core.DataAccess.Xml
{
  public class XmlProjectInfoRepository : IProjectInfoRepository
  {
    #region Nested types

    [XmlInclude(typeof(NtServiceProjectInfoXml))]
    [XmlInclude(typeof(WebAppProjectInfoXml))]
    [XmlInclude(typeof(SchedulerAppProjectInfoXml))]
    [XmlInclude(typeof(TerminalAppProjectInfoXml))]
    public class ProjectInfosXml
    {
      public List<ProjectInfoXml> ProjectInfos { get; set; }
    }

    public abstract class ProjectInfoXml
    {
      public string Name { get; set; }

      public string Type { get; set; }

      public string ArtifactsRepositoryName { get; set; }

      public string ArtifactsRepositoryDirName { get; set; }
    }

    public class NtServiceProjectInfoXml : ProjectInfoXml
    {
      public string NtServiceName { get; set; }

      public string NtServiceDirName { get; set; }

      public string NtServiceDisplayName { get; set; }

      public string NtServiceExeName { get; set; }

      public string NtServiceUserId { get; set; }
    }

    public class WebAppProjectInfoXml : ProjectInfoXml
    {
      public string IisSiteName { get; set; }
      
      public string WebAppName { get; set; }

      public string WebAppDirName { get; set; }

      public IisAppPoolInfoXml AppPool { get; set; }
    }

    public class IisAppPoolInfoXml
    {
      public string Name { get; set; }
      
      public IisAppPoolVersion Version { get; set; }
      
      public IisAppPoolMode Mode { get; set; }
    }

    public class SchedulerAppProjectInfoXml : ProjectInfoXml
    {
      public string SchedulerAppName { get; set; }

      public string SchedulerAppDirName { get; set; }

      public string SchedulerAppExeName { get; set; }

      public string SchedulerAppUserId { get; set; }

      public int ScheduledHour { get; set; }

      public int ScheduledMinute { get; set; }

      /// <summary>
      /// 0 - no limit.
      /// </summary>
      public int ExecutionTimeLimitInMinutes { get; set; }
    }

    public class TerminalAppProjectInfoXml : ProjectInfoXml
    {
      public string TerminalAppName { get; set; }

      public string TerminalAppDirName { get; set; }
      
      public string TerminalAppExeName { get; set; }
    }

    #endregion

    private readonly string _xmlFilePath;

    private ProjectInfosXml _projectInfosXml;
    private Dictionary<string, ProjectInfo> _projectInfosByName;

    #region Constructor(s)

    public XmlProjectInfoRepository(string xmlFilePath)
    {
      if (string.IsNullOrEmpty(xmlFilePath))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "xmlFilePath");
      }

      _xmlFilePath = xmlFilePath;
    }

    #endregion

    #region IProjectInfoRepository Members

    public IEnumerable<ProjectInfo> GetAll()
    {
      LoadXmlIfNeeded();

      return
        _projectInfosByName.Values
          .OrderBy(pi => pi.Name);
    }

    public ProjectInfo GetByName(string name)
    {
      if (string.IsNullOrEmpty(name))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "name");
      }

      LoadXmlIfNeeded();

      ProjectInfo projectInfo;

      if (!_projectInfosByName.TryGetValue(name, out projectInfo))
      {
        return null;
      }

      return projectInfo;
    }

    #endregion

    #region Private helper methods

    private static ProjectInfo CreateProjectInfo(ProjectInfoXml projectInfoXml)
    {
      var ntServiceProjectInfoXml = projectInfoXml as NtServiceProjectInfoXml;

      if (ntServiceProjectInfoXml != null)
      {
        return
          new NtServiceProjectInfo(
            ntServiceProjectInfoXml.Name,
            ntServiceProjectInfoXml.ArtifactsRepositoryName,
            ntServiceProjectInfoXml.ArtifactsRepositoryDirName,
            ntServiceProjectInfoXml.NtServiceName,
            ntServiceProjectInfoXml.NtServiceDirName,
            ntServiceProjectInfoXml.NtServiceDisplayName,
            ntServiceProjectInfoXml.NtServiceExeName,
            ntServiceProjectInfoXml.NtServiceUserId);
      }

      var webAppProjectInfoXml = projectInfoXml as WebAppProjectInfoXml;

      if (webAppProjectInfoXml != null)
      {
        return
          new WebAppProjectInfo(
            webAppProjectInfoXml.Name,
            webAppProjectInfoXml.ArtifactsRepositoryName,
            webAppProjectInfoXml.ArtifactsRepositoryDirName,
            webAppProjectInfoXml.IisSiteName,
            webAppProjectInfoXml.WebAppName,
            webAppProjectInfoXml.WebAppDirName,
            new IisAppPoolInfo(
              webAppProjectInfoXml.AppPool.Name,
              webAppProjectInfoXml.AppPool.Version,
              webAppProjectInfoXml.AppPool.Mode));
      }

      var schedulerAppProjectInfoXml = projectInfoXml as SchedulerAppProjectInfoXml;

      if (schedulerAppProjectInfoXml != null)
      {
        return
          new SchedulerAppProjectInfo(
            schedulerAppProjectInfoXml.Name,
            schedulerAppProjectInfoXml.ArtifactsRepositoryName,
            schedulerAppProjectInfoXml.ArtifactsRepositoryDirName,
            schedulerAppProjectInfoXml.SchedulerAppName,
            schedulerAppProjectInfoXml.SchedulerAppDirName,
            schedulerAppProjectInfoXml.SchedulerAppExeName,
            schedulerAppProjectInfoXml.SchedulerAppUserId,
            schedulerAppProjectInfoXml.ScheduledHour,
            schedulerAppProjectInfoXml.ScheduledMinute,
            schedulerAppProjectInfoXml.ExecutionTimeLimitInMinutes);
      }

      var terminalAppProjectInfoXml = projectInfoXml as TerminalAppProjectInfoXml;

      if (terminalAppProjectInfoXml != null)
      {
        return
          new TerminalAppProjectInfo(
            terminalAppProjectInfoXml.Name,
            terminalAppProjectInfoXml.ArtifactsRepositoryName,
            terminalAppProjectInfoXml.ArtifactsRepositoryDirName,
            terminalAppProjectInfoXml.TerminalAppName,
            terminalAppProjectInfoXml.TerminalAppDirName,
            terminalAppProjectInfoXml.TerminalAppExeName);
      }

      throw new NotSupportedException(string.Format("Project type '{0}' is not supported.", projectInfoXml.GetType()));
    }

    private void LoadXmlIfNeeded()
    {
      if (_projectInfosXml != null)
      {
        return;
      }

      var xmlSerializer = new XmlSerializer(typeof(ProjectInfosXml));

      using (var fs = File.OpenRead(_xmlFilePath))
      {
        _projectInfosXml = (ProjectInfosXml)xmlSerializer.Deserialize(fs);
      }

      _projectInfosByName =
        _projectInfosXml.ProjectInfos
          .Select(CreateProjectInfo)
          .ToDictionary(pi => pi.Name);
    }

    #endregion
  }
}
