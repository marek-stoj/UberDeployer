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
    [XmlInclude(typeof(DbProjectInfoXml))]
    [XmlInclude(typeof(UberDeployerAgentProjectInfoXml))]
    public class ProjectInfosXml
    {
      public List<ProjectInfoXml> ProjectInfos { get; set; }
    }

    public abstract class ProjectInfoXml
    {
      private string _allowedEnvironments;
      public string Name { get; set; }

      public string Type { get; set; }

      public string ArtifactsRepositoryName { get; set; }

      public string ArtifactsRepositoryDirName { get; set; }

      public bool ArtifactsAreNotEnvironmentSpecific { get; set; }

      [XmlAttribute("allowedEnvironments")]
      public string AllowedEnvironments
      {
        get { return _allowedEnvironments; }
        set { _allowedEnvironments = value; }
      }
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
      public string AppPoolId { get; set; }
      
      public string WebSiteName { get; set; }
      
      public string WebAppDirName { get; set; }
      
      public string WebAppName { get; set; }
    }

    public class SchedulerAppProjectInfoXml : ProjectInfoXml
    {
      public string SchedulerAppDirName { get; set; }

      public string SchedulerAppExeName { get; set; }

      public List<SchedulerAppTaskXml> SchedulerAppTasks { get; set; }
    }

    public class SchedulerAppTaskXml
    {
      public string Name { get; set; }

      public string ExecutableName { get; set; }

      public string UserId { get; set; }

      public int ScheduledHour { get; set; }

      public int ScheduledMinute { get; set; }

      /// <summary>
      /// 0 - no limit.
      /// </summary>
      public int ExecutionTimeLimitInMinutes { get; set; }

      public RepetitionXml Repetition { get; set; }
    }

    public class RepetitionXml
    {
      public bool Enabled { get; set; }

      public string Interval { get; set; }

      public string Duration { get; set; }

      public bool StopAtDurationEnd { get; set; }
    }

    public class TerminalAppProjectInfoXml : ProjectInfoXml
    {
      public string TerminalAppName { get; set; }

      public string TerminalAppDirName { get; set; }
      
      public string TerminalAppExeName { get; set; }
    }

    public class DbProjectInfoXml : ProjectInfoXml
    {
      public string DbName { get; set; }

      public string DatabaseServerId { get; set; }
    }

    public class UberDeployerAgentProjectInfoXml : NtServiceProjectInfoXml
    {
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

    public ProjectInfo FindByName(string name)
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
      List<string> allowedEnvironmentNames =
        (projectInfoXml.AllowedEnvironments ?? "")
          .Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
          .ToList();

      var uberDeployerAgentProjectInfoXml = projectInfoXml as UberDeployerAgentProjectInfoXml;

      if (uberDeployerAgentProjectInfoXml != null)
      {
        return
          new UberDeployerAgentProjectInfo(
            uberDeployerAgentProjectInfoXml.Name,
            uberDeployerAgentProjectInfoXml.ArtifactsRepositoryName,
            allowedEnvironmentNames,
            uberDeployerAgentProjectInfoXml.ArtifactsRepositoryDirName,
            uberDeployerAgentProjectInfoXml.ArtifactsAreNotEnvironmentSpecific,
            uberDeployerAgentProjectInfoXml.NtServiceName,
            uberDeployerAgentProjectInfoXml.NtServiceDirName,
            uberDeployerAgentProjectInfoXml.NtServiceDisplayName,
            uberDeployerAgentProjectInfoXml.NtServiceExeName,
            uberDeployerAgentProjectInfoXml.NtServiceUserId);
      }

      var ntServiceProjectInfoXml = projectInfoXml as NtServiceProjectInfoXml;

      if (ntServiceProjectInfoXml != null)
      {
        return
          new NtServiceProjectInfo(
            ntServiceProjectInfoXml.Name,
            ntServiceProjectInfoXml.ArtifactsRepositoryName,
            allowedEnvironmentNames,
            ntServiceProjectInfoXml.ArtifactsRepositoryDirName,
            ntServiceProjectInfoXml.ArtifactsAreNotEnvironmentSpecific,
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
            allowedEnvironmentNames,
            webAppProjectInfoXml.ArtifactsRepositoryDirName,
            webAppProjectInfoXml.ArtifactsAreNotEnvironmentSpecific,
            webAppProjectInfoXml.AppPoolId,
            webAppProjectInfoXml.WebSiteName,
            webAppProjectInfoXml.WebAppDirName,
            webAppProjectInfoXml.WebAppName);
      }

      var schedulerAppProjectInfoXml = projectInfoXml as SchedulerAppProjectInfoXml;

      if (schedulerAppProjectInfoXml != null)
      {
        return
          new SchedulerAppProjectInfo(
            schedulerAppProjectInfoXml.Name,
            schedulerAppProjectInfoXml.ArtifactsRepositoryName,
            allowedEnvironmentNames,
            schedulerAppProjectInfoXml.ArtifactsRepositoryDirName,
            schedulerAppProjectInfoXml.ArtifactsAreNotEnvironmentSpecific,
            schedulerAppProjectInfoXml.SchedulerAppDirName,
            schedulerAppProjectInfoXml.SchedulerAppExeName,
            schedulerAppProjectInfoXml.SchedulerAppTasks
              .Select(
                x =>
                  new SchedulerAppTask(
                    x.Name,
                    x.ExecutableName,
                    x.UserId,
                    x.ScheduledHour,
                    x.ScheduledMinute,
                    x.ExecutionTimeLimitInMinutes,
                    x.Repetition.Enabled
                      ? Repetition.CreateEnabled(
                        TimeSpan.Parse(x.Repetition.Interval),
                        TimeSpan.Parse(x.Repetition.Duration),
                        x.Repetition.StopAtDurationEnd)
                      : Repetition.CreatedDisabled())));
      }

      var terminalAppProjectInfoXml = projectInfoXml as TerminalAppProjectInfoXml;

      if (terminalAppProjectInfoXml != null)
      {
        return
          new TerminalAppProjectInfo(
            terminalAppProjectInfoXml.Name,
            terminalAppProjectInfoXml.ArtifactsRepositoryName,
            allowedEnvironmentNames,
            terminalAppProjectInfoXml.ArtifactsRepositoryDirName,
            terminalAppProjectInfoXml.ArtifactsAreNotEnvironmentSpecific,
            terminalAppProjectInfoXml.TerminalAppName,
            terminalAppProjectInfoXml.TerminalAppDirName,
            terminalAppProjectInfoXml.TerminalAppExeName);
      }

      var dbProjectInfoXml = projectInfoXml as DbProjectInfoXml;

      if (dbProjectInfoXml != null)
      {
        return
          new DbProjectInfo(
            dbProjectInfoXml.Name,
            dbProjectInfoXml.ArtifactsRepositoryName,
            allowedEnvironmentNames,
            dbProjectInfoXml.ArtifactsRepositoryDirName,
            dbProjectInfoXml.ArtifactsAreNotEnvironmentSpecific,
            dbProjectInfoXml.DbName,
            dbProjectInfoXml.DatabaseServerId);
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
