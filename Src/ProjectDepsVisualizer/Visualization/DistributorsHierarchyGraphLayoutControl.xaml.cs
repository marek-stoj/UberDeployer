using GraphSharp.Algorithms.Layout.Simple.Hierarchical;

namespace ProjectDepsVisualizer.Visualization
{
  public partial class ProjectDependenciesGraphLayoutControl
  {
    private ProjectDependenciesGraph _graph;

    #region Constructor(s)

    public ProjectDependenciesGraphLayoutControl()
    {
      InitializeComponent();

      graphLayout.LayoutAlgorithmType = "EfficientSugiyama";
      graphLayout.HighlightAlgorithmType = "Simple";

      var layoutParameters = new EfficientSugiyamaLayoutParameters();

      layoutParameters.EdgeRouting = SugiyamaEdgeRoutings.Traditional;
      layoutParameters.MinimizeEdgeLength = false;
      layoutParameters.OptimizeWidth = false;

      graphLayout.LayoutParameters = layoutParameters;
    }

    #endregion

    #region Properties

    public ProjectDependenciesGraph Graph
    {
      get { return _graph; }

      set
      {
        _graph = value;
        graphLayout.Graph = _graph;
      }
    }

    #endregion
  }
}
