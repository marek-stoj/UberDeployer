using System.Collections.Generic;
using System.Linq;
using ProjectDepsVisualizer.Domain;
using ProjectDepsVisualizer.UI;

namespace ProjectDepsVisualizer.Core
{
  public class ProjectDependenciesModel
  {
    private Dictionary<ProjectDesignator, ProjectInfo> _projectInfos = new Dictionary<ProjectDesignator, ProjectInfo>();

    public ProjectInfo RootProjectInfo { get; set; }

    public Dictionary<ProjectDesignator, ProjectInfo> ProjectInfos
    {
      get { return _projectInfos; }
      set { _projectInfos = value; }
    }

    public VersionsIntegrityCheckResult VerifyVersionsIntegrity()
    {
      bool areVersionsIntegral = true;

      foreach (ProjectInfo projectInfo1 in _projectInfos.Values)
      {
        foreach (ProjectDependency projectDependency1 in projectInfo1.ProjectDependencies)
        {
          bool existsNonIntegralVersionDependency =
            _projectInfos.Values
              .Any(
                projectInfo2 =>
                (projectInfo2.ProjectName != projectInfo1.ProjectName || projectInfo2.ProjectConfiguration != projectInfo1.ProjectConfiguration) &&
                projectInfo2.ProjectDependencies
                  .Any(
                    projectDependency2 =>
                    projectDependency2.ProjectName == projectDependency1.ProjectName &&
                    projectDependency2.ProjectVersion != projectDependency1.ProjectVersion));

          if (existsNonIntegralVersionDependency)
          {
            areVersionsIntegral = false;
            break;
          }
        }
      }

      return
        new VersionsIntegrityCheckResult(areVersionsIntegral);
    }
  }
}
