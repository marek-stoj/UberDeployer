using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ProjectDepsVisualizer.Domain;

namespace ProjectDepsVisualizer.Core.Exporters
{
  public class DgmlProjectDependenciesModelExporter : IProjectDependenciesModelExporter
  {
    private static readonly XNamespace _DgmlNamespace = "http://schemas.microsoft.com/vs/2009/dgml";

    #region IProjectDependenciesModelExporter members

    public void Export(ProjectDependenciesModel projectDependenciesModel, string filePath)
    {
      if (projectDependenciesModel == null) throw new ArgumentNullException("projectDependenciesModel");
      if (filePath == null) throw new ArgumentNullException("filePath");

      var rootElement = new XElement(_DgmlNamespace + "DirectedGraph");
      var linksElement = new XElement(_DgmlNamespace + "Links");

      rootElement.Add(linksElement);

      var xDocument = new XDocument(rootElement);
      var visitedMap = new Dictionary<ProjectDesignator, bool>();

      ExportAux(linksElement, projectDependenciesModel, projectDependenciesModel.RootProjectInfo, visitedMap);

      using (var xtw = new XmlTextWriter(filePath, Encoding.UTF8))
      {
        xtw.Formatting = Formatting.Indented;

        xDocument.WriteTo(xtw);
      }
    }

    public string ExportFormatName
    {
      get { return "DGML"; }
    }

    public string ExportedFileExtension
    {
      get { return "dgml"; }
    }

    #endregion

    #region Private helper methods

    private static void ExportAux(XElement linksElement, ProjectDependenciesModel projectDependenciesModel, ProjectInfo projectInfo, Dictionary<ProjectDesignator, bool> visitedMap)
    {
      if (linksElement == null) throw new ArgumentNullException("linksElement");
      if (projectDependenciesModel == null) throw new ArgumentNullException("projectDependenciesModel");
      if (projectInfo == null) throw new ArgumentNullException("projectInfo");
      if (visitedMap == null) throw new ArgumentNullException("visitedMap");

      ProjectDesignator projectDesignator = ProjectDesignator.FromProjectInfo(projectInfo);

      visitedMap[projectDesignator] = true;

      foreach (ProjectDependency projectDependency in projectInfo.ProjectDependencies)
      {
        ProjectDesignator dependentProjectDesignator = ProjectDesignator.FromProjectDependency(projectDependency);
        ProjectInfo dependentProjectInfo = projectDependenciesModel.ProjectInfos[dependentProjectDesignator];

        linksElement.Add(
          new XElement(
            _DgmlNamespace + "Link",
            new XAttribute("Source", CreateProjectDisplayString(projectInfo)),
            new XAttribute("Target", CreateProjectDisplayString(dependentProjectInfo)),
            new XAttribute("Label", projectDependency.ProjectVersion)));

        if (visitedMap.ContainsKey(dependentProjectDesignator))
        {
          // already processed - skip it
          continue;
        }

        ExportAux(linksElement, projectDependenciesModel, dependentProjectInfo, visitedMap);
      }
    }

    private static object CreateProjectDisplayString(ProjectInfo projectInfo)
    {
      if (projectInfo == null) throw new ArgumentNullException("projectInfo");

      return string.Format("{0} - {1} - {2}", projectInfo.ProjectName, projectInfo.ProjectConfiguration, projectInfo.ProjectVersion);
    }

    #endregion
  }
}
