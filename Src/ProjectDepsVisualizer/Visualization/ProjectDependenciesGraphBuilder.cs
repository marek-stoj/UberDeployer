using System;
using System.Collections.Generic;
using ProjectDepsVisualizer.Core;
using ProjectDepsVisualizer.Domain;
using QuickGraph;

namespace ProjectDepsVisualizer.Visualization
{
  public class ProjectDependenciesGraphBuilder
  {
    #region Public methods

    public ProjectDependenciesGraph BuildGraph(ProjectDependenciesModel projectDependenciesModel)
    {
      if (projectDependenciesModel == null) throw new ArgumentNullException("projectDependenciesModel");

      var projectDependenciesGraph = new ProjectDependenciesGraph();

      Dictionary<ProjectDesignator, ProjectInfoVertex> verticesMap =
        GatherVertices(projectDependenciesModel, projectDependenciesGraph);

      GatherEdges(projectDependenciesModel, projectDependenciesGraph, verticesMap);

      return projectDependenciesGraph;
    }

    #endregion

    #region Private helper methods

    private static Dictionary<ProjectDesignator, ProjectInfoVertex> GatherVertices(ProjectDependenciesModel projectDependenciesModel, ProjectDependenciesGraph projectDependenciesGraph)
    {
      if (projectDependenciesModel == null) throw new ArgumentNullException("projectDependenciesModel");
      if (projectDependenciesGraph == null) throw new ArgumentNullException("projectDependenciesGraph");

      var verticesMap = new Dictionary<ProjectDesignator, ProjectInfoVertex>();

      foreach (ProjectDesignator projectDesignator in projectDependenciesModel.ProjectInfos.Keys)
      {
        var projectInfo = projectDependenciesModel.ProjectInfos[projectDesignator];
        var projectInfoVertex = new ProjectInfoVertex(projectInfo);

        projectDependenciesGraph.AddVertex(projectInfoVertex);
        verticesMap.Add(projectDesignator, projectInfoVertex);
      }

      return verticesMap;
    }

    private static void GatherEdges(ProjectDependenciesModel projectDependenciesModel, ProjectDependenciesGraph projectDependenciesGraph, Dictionary<ProjectDesignator, ProjectInfoVertex> verticesMap)
    {
      if (projectDependenciesModel == null) throw new ArgumentNullException("projectDependenciesModel");
      if (projectDependenciesGraph == null) throw new ArgumentNullException("projectDependenciesGraph");
      if (verticesMap == null) throw new ArgumentNullException("verticesMap");

      var visitedMap = new Dictionary<ProjectDesignator, bool>();

      foreach (ProjectDesignator projectDesignator in verticesMap.Keys)
      {
        ProjectInfoVertex vertex = verticesMap[projectDesignator];

        if (visitedMap.ContainsKey(projectDesignator))
        {
          continue;
        }

        GatherEdgesAux(
          vertex.ProjectInfo,
          projectDependenciesGraph,
          verticesMap,
          visitedMap);
      }
    }

    private static void GatherEdgesAux(ProjectInfo projectInfo, ProjectDependenciesGraph projectDependenciesGraph, Dictionary<ProjectDesignator, ProjectInfoVertex> verticesMap, Dictionary<ProjectDesignator, bool> visitedMap)
    {
      // TODO IMM HI: abstract the visitor -> such code is duplicated in DgmlProjectDependenciesModelExporter

      if (projectInfo == null) throw new ArgumentNullException("projectInfo");
      if (projectDependenciesGraph == null) throw new ArgumentNullException("projectDependenciesGraph");
      if (verticesMap == null) throw new ArgumentNullException("verticesMap");

      ProjectDesignator projectDesignator = ProjectDesignator.FromProjectInfo(projectInfo);
      ProjectInfoVertex projectInfoVertex = verticesMap[projectDesignator];

      visitedMap[projectDesignator] = true;

      foreach (ProjectDependency projectDependency in projectInfo.ProjectDependencies)
      {
        ProjectDesignator dependentProjectDesignator = ProjectDesignator.FromProjectDependency(projectDependency);
        ProjectInfoVertex dependentProjectInfoVertex = verticesMap[dependentProjectDesignator];

        projectDependenciesGraph.AddEdge(
          new Edge<ProjectInfoVertex>(
            projectInfoVertex,
            dependentProjectInfoVertex));

        if (visitedMap.ContainsKey(dependentProjectDesignator))
        {
          // already processed - skip it
          continue;
        }
        
        GatherEdgesAux(dependentProjectInfoVertex.ProjectInfo, projectDependenciesGraph, verticesMap, visitedMap);
      }
    }

    #endregion
  }
}
