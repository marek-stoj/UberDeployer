using System;
using System.Collections.Generic;
using ProjectDepsVisualizer.Domain;
using System.Linq;

namespace ProjectDepsVisualizer.Core
{
  public class ProjectDependenciesModelBuilder
  {
    public event EventHandler<LogMessageEventArgs> LogMessagePosted;

    private readonly ISvnClient _svnClient;
    private readonly BuildFileAnalyzer _buildFileAnalyzer;
    private readonly VersionFileAnalyzer _versionFileAnalyzer;

    #region Constructor(s)

    public ProjectDependenciesModelBuilder(ISvnClient svnClient)
    {
      if (svnClient == null) throw new ArgumentNullException("svnClient");

      _svnClient = svnClient;
      
      _buildFileAnalyzer = new BuildFileAnalyzer();
      _versionFileAnalyzer = new VersionFileAnalyzer();
    }

    #endregion

    #region Public methods

    public ProjectDependenciesModel BuildModel(string projectName, string projectConfiguration)
    {
      if (projectName == null) throw new ArgumentNullException("projectName");
      if (projectConfiguration == null) throw new ArgumentNullException("projectConfiguration");

      var projectDependenciesModel = new ProjectDependenciesModel();
      var projectDesignatorQueue = new Queue<ProjectDesignator>();

      projectDesignatorQueue.Enqueue(new ProjectDesignator(projectName, projectConfiguration));

      while (projectDesignatorQueue.Count > 0)
      {
        // get from queue the next project designator to process
        ProjectDesignator projectDesignator =
          projectDesignatorQueue.Dequeue();

        // check if we've already obtained this project's info and if so - skip it
        if (projectDependenciesModel.ProjectInfos.ContainsKey(projectDesignator))
        {
          continue;
        }

        OnLogMessagePosted(
          string.Format(
            "Obtaining project info. Project: {0} (configuration: {1}).",
            projectDesignator.ProjectName,
            projectDesignator.ProjectConfiguration));

        // obtain project info
        ProjectInfo projectInfo =
          ObtainProjectInfo(
            projectDesignator.ProjectName,
            projectDesignator.ProjectConfiguration);

        // remember that we have this project info obtained
        projectDependenciesModel.ProjectInfos.Add(projectDesignator, projectInfo);

        // enqueue all dependent projects so that we'll obtain their infos
        foreach (ProjectDependency projectDependency in projectInfo.ProjectDependencies)
        {
          ProjectDesignator dependentProjectDesignator =
            ProjectDesignator.FromProjectDependency(projectDependency);

          // add this project to the queue if we haven't obtained its info yet
          if (!projectDependenciesModel.ProjectInfos.ContainsKey(dependentProjectDesignator))
          {
            projectDesignatorQueue.Enqueue(dependentProjectDesignator);
          }
        }

        // store the root project info to return from the method (if it hasn't been set yet)
        if (projectDependenciesModel.RootProjectInfo == null)
        {
          projectDependenciesModel.RootProjectInfo = projectInfo;
        }
      }

      return projectDependenciesModel;
    }

    public ProjectDependenciesModel BuildWhoDependsOnModel(string projectName, string projectConfiguration)
    {
      if (projectName == null) throw new ArgumentNullException("projectName");
      if (projectConfiguration == null) throw new ArgumentNullException("projectConfiguration");

      string[] repositories = _svnClient.GetRepositoryNames();

      var projectDependenciesModel = new ProjectDependenciesModel();
      var projectDesignatorQueue = new Queue<ProjectDesignator>();
      foreach (string repository in repositories)
      {
        projectDesignatorQueue.Enqueue(new ProjectDesignator(repository,projectConfiguration));
      }

      projectDesignatorQueue.Enqueue(new ProjectDesignator(projectName, projectConfiguration));

      while (projectDesignatorQueue.Count > 0)
      {
        // get from queue the next project designator to process
        ProjectDesignator projectDesignator =
          projectDesignatorQueue.Dequeue();

        // check if we've already obtained this project's info and if so - skip it
        if (projectDependenciesModel.ProjectInfos.ContainsKey(projectDesignator))
        {
          continue;
        }

        OnLogMessagePosted(
          string.Format(
            "Obtaining project info. Project: {0} (configuration: {1}).",
            projectDesignator.ProjectName,
            projectDesignator.ProjectConfiguration));

        // obtain project info
        ProjectInfo projectInfo =
          ObtainProjectInfo(
            projectDesignator.ProjectName,
            projectDesignator.ProjectConfiguration);

        // remember that we have this project info obtained
        projectDependenciesModel.ProjectInfos.Add(projectDesignator, projectInfo);

        // enqueue all dependent projects so that we'll obtain their infos
        foreach (ProjectDependency projectDependency in projectInfo.ProjectDependencies)
        {
          ProjectDesignator dependentProjectDesignator =
            ProjectDesignator.FromProjectDependency(projectDependency);

          // add this project to the queue if we haven't obtained its info yet
          if (!projectDependenciesModel.ProjectInfos.ContainsKey(dependentProjectDesignator))
          {
            projectDesignatorQueue.Enqueue(dependentProjectDesignator);
          }
        }      
      }

      projectDependenciesModel.RootProjectInfo = projectDependenciesModel.ProjectInfos.FirstOrDefault(f=>f.Key.ProjectName == projectName).Value;
      
      List<ProjectDesignator> removeList = new List<ProjectDesignator>();
      foreach (ProjectDesignator projectDesignator in projectDependenciesModel.ProjectInfos.Keys)
      {
        if (projectDesignator.ProjectName == projectDependenciesModel.RootProjectInfo.ProjectName && 
          projectDesignator.ProjectConfiguration == projectDependenciesModel.RootProjectInfo.ProjectConfiguration)
        {
          continue;
        }
        if (!HasPathTo(projectDependenciesModel.ProjectInfos[projectDesignator], projectDependenciesModel.RootProjectInfo, projectDependenciesModel))
        {
          removeList.Add(projectDesignator);
        }
      }

      foreach (ProjectDesignator projectDesignator in removeList)
      {
        projectDependenciesModel.ProjectInfos.Remove(projectDesignator);
        foreach(ProjectInfo projectInfo in projectDependenciesModel.ProjectInfos.Values)
        {
          projectInfo.ProjectDependencies.RemoveAll(f => f.ProjectConfiguration == projectDesignator.ProjectConfiguration && f.ProjectName == projectDesignator.ProjectName);
        }
      }

      return projectDependenciesModel;
    }

    private bool HasPathTo(ProjectInfo searchedProject, ProjectInfo rootProject, ProjectDependenciesModel model)
    {   
      foreach (ProjectDependency projectDependency in searchedProject.ProjectDependencies)
      {
        if (projectDependency.ProjectName == rootProject.ProjectName && projectDependency.ProjectConfiguration == rootProject.ProjectConfiguration && rootProject.ProjectVersion == projectDependency.ProjectVersion)
          return true;
        return HasPathTo(model.ProjectInfos[new ProjectDesignator(projectDependency.ProjectName,projectDependency.ProjectConfiguration)],rootProject, model);
      }
      return false;
    }

    #endregion

    #region Private helper methods

    private ProjectInfo ObtainProjectInfo(string projectName, string projectConfiguration)
    {
      if (projectName == null) throw new ArgumentNullException("projectName");
      if (projectConfiguration == null) throw new ArgumentNullException("projectConfiguration");

      var projectInfo =
        new ProjectInfo
        {
          ProjectName = projectName,
          ProjectConfiguration = projectConfiguration,
        };

      string projectConfigurationUpper = projectConfiguration.ToUpper();
      string projectRelativePath =
        (projectConfigurationUpper == "TRUNK" || projectConfigurationUpper == "PRODUCTION")
          ? string.Format("{0}/trunk", projectName)
          : string.Format("{0}/branches/{1}", projectName, projectConfiguration);

      ObtainProjectVersion(projectInfo, projectRelativePath);
      ObtainProjectDependencies(projectInfo, projectRelativePath);

      return projectInfo;
    }

    private void ObtainProjectVersion(ProjectInfo projectInfo, string projectRelativePath)
    {
      if (projectInfo == null) throw new ArgumentNullException("projectInfo");
      if (projectRelativePath == null) throw new ArgumentNullException("projectRelativePath");

      string versionFileName = "Version.xml";
      string versionFilePath = string.Format("{0}/{1}", projectRelativePath, versionFileName);
      string versionFileContents = _svnClient.GetFileContents(versionFilePath);

      if (string.IsNullOrEmpty(versionFileContents))
      {
        projectInfo.ProjectVersion = "?";

        return;
      }

      string projectVersion = _versionFileAnalyzer.GetProjectVersion(versionFileContents);

      if (string.IsNullOrEmpty(projectVersion))
      {
        throw new ArgumentException(
          string.Format(
            "Couldn't obtain project version for project {0} (configuration: {1}).",
            projectInfo.ProjectName,
            projectInfo.ProjectConfiguration));
      }

      projectInfo.ProjectVersion = projectVersion;
    }

    private void ObtainProjectDependencies(ProjectInfo projectInfo, string projectRelativePath)
    {
      if (projectInfo == null) throw new ArgumentNullException("projectInfo");
      if (projectRelativePath == null) throw new ArgumentNullException("projectRelativePath");

      string buildFileContents = _svnClient.GetFileContentsByExt(projectRelativePath,".build");
      if (buildFileContents != null)
      {
        foreach (var projectDependency in _buildFileAnalyzer.GetProjectDependencies(buildFileContents))
        {
          projectInfo.ProjectDependencies.Add(projectDependency);
        }
      }
    }

    private void OnLogMessagePosted(string message)
    {
      if (message == null) throw new ArgumentNullException("message");

      var eventHandler = LogMessagePosted;

      if (eventHandler != null)
      {
        eventHandler(this, new LogMessageEventArgs(message));
      }
    }

    #endregion
  }
}
