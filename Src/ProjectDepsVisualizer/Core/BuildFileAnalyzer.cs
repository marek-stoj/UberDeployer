using System;
using System.Collections.Generic;
using System.Xml.Linq;
using ProjectDepsVisualizer.Domain;
using System.Xml.XPath;

namespace ProjectDepsVisualizer.Core
{
  public class BuildFileAnalyzer
  {
    public List<ProjectDependency> GetProjectDependencies(string buildFileContents)
    {
      if (buildFileContents == null)
      {
        throw new ArgumentNullException("buildFileContents");
      }

      var projectDependencies = new List<ProjectDependency>();

      XDocument xDocument = XDocument.Parse(buildFileContents);

      foreach (XElement xElement in xDocument.XPathSelectElements("project/target/downloadArtifacts/download/getArtifact"))
      {
        string projectName = xElement.Attribute("projectName").Value;
        string buildConfigurationName = xElement.Attribute("buildConfigurationName").Value;
        string version = xElement.Attribute("version").Value;

        projectDependencies.Add(
          new ProjectDependency
            {
              ProjectName = projectName,
              ProjectConfiguration = buildConfigurationName,
              ProjectVersion = version,
            });
      }

      return projectDependencies;
    }
  }
}
