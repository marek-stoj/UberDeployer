using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using ProjectDepsVisualizer.Domain;
using System.Xml.XPath;

namespace ProjectDepsVisualizer.Core
{
  public class BuildFileAnalyzer
  {
    public List<ProjectDependency> GetProjectDependencies(string buildFileContents)
    {
      if (buildFileContents == null) throw new ArgumentNullException("buildFileContents");

      var projectDependencies = new List<ProjectDependency>();
      try
      {
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
      }
      catch (Exception exc)
      {
        StreamWriter sw = new StreamWriter("c:\\f.build");
        sw.Write(buildFileContents);
        sw.Close();
      }

      return projectDependencies;
    }
  }
}
