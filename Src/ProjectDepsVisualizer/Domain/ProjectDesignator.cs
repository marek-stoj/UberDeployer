using System;

namespace ProjectDepsVisualizer.Domain
{
  public class ProjectDesignator
  {
    #region Constructor(s)

    public ProjectDesignator(string name, string configuration)
    {
      if (name == null) throw new ArgumentNullException("name");
      if (configuration == null) throw new ArgumentNullException("configuration");

      ProjectName = name;
      ProjectConfiguration = configuration;
    }

    #endregion

    #region Factory methods

    public static ProjectDesignator FromProjectInfo(ProjectInfo projectInfo)
    {
      if (projectInfo == null) throw new ArgumentNullException("projectInfo");

      return new ProjectDesignator(projectInfo.ProjectName, projectInfo.ProjectConfiguration);
    }

    public static ProjectDesignator FromProjectDependency(ProjectDependency projectDependency)
    {
      if (projectDependency == null) throw new ArgumentNullException("projectDependency");

      return new ProjectDesignator(projectDependency.ProjectName, projectDependency.ProjectConfiguration);
    }

    #endregion

    #region Object overrides

    public override string ToString()
    {
      return string.Format("{0}, {1}", ProjectName, ProjectConfiguration);
    }

    #endregion

    #region Equality members

    public bool Equals(ProjectDesignator other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return Equals(other.ProjectName, ProjectName) && Equals(other.ProjectConfiguration, ProjectConfiguration);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != typeof(ProjectDesignator)) return false;
      return Equals((ProjectDesignator)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((ProjectName != null ? ProjectName.GetHashCode() : 0) * 397) ^ (ProjectConfiguration != null ? ProjectConfiguration.GetHashCode() : 0);
      }
    }

    #endregion

    #region Properties

    public string ProjectName { get; set; }

    public string ProjectConfiguration { get; set; }

    #endregion
  }
}
