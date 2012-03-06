using System;
using ProjectDepsVisualizer.Domain;

namespace ProjectDepsVisualizer.Visualization
{
  public class ProjectInfoVertex
  {
    private readonly ProjectInfo _projectInfo;

    #region Constructor(s)

    public ProjectInfoVertex(ProjectInfo projectInfo)
    {
      if (projectInfo == null) throw new ArgumentNullException("projectInfo");

      _projectInfo = projectInfo;
    }

    #endregion

    #region Properties

    public ProjectInfo ProjectInfo { get { return _projectInfo; } }

    public string ProjectName
    {
      get { return _projectInfo.ProjectName; }
    }

    public string ProjectConfiguration
    {
      get { return _projectInfo.ProjectConfiguration; }
    }

    public string ProjectVersion
    {
      get { return _projectInfo.ProjectVersion; }
    }

    #endregion
  }
}
