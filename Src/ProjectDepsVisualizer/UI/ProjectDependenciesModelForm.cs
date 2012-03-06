using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ProjectDepsVisualizer.Core;
using ProjectDepsVisualizer.Core.Exporters;
using ProjectDepsVisualizer.Visualization;
using System.Reflection;

namespace ProjectDepsVisualizer.UI
{
  public partial class ProjectDependenciesModelForm : Form
  {
    private static readonly Dictionary<string, IProjectDependenciesModelExporter> _availableExporters;

    private readonly ProjectDependenciesGraphLayoutControl _projectDependenciesGraphLayoutControl;

    private readonly ProjectDependenciesModel _projectDependenciesModel;

    private ProjectDependenciesGraph _projectDependenciesGraph;

    #region Constructor(s)

    static ProjectDependenciesModelForm()
    {
      _availableExporters = new Dictionary<string, IProjectDependenciesModelExporter>();

      ObtainAvailableExporters();
    }

    public ProjectDependenciesModelForm(ProjectDependenciesModel projectDependenciesModel)
    {
      if (projectDependenciesModel == null) throw new ArgumentNullException("projectDependenciesModel");

      _projectDependenciesModel = projectDependenciesModel;

      InitializeComponent();

      _projectDependenciesGraphLayoutControl = new ProjectDependenciesGraphLayoutControl();
      wpfElementHost_dependenciesGraph.Child = _projectDependenciesGraphLayoutControl;

      PopulateExportFormats();
    }

    #endregion

    #region WinForms event handlers

    private void ProjectDependenciesGraphForm_Load(object sender, EventArgs e)
    {
      BuildGraph();
      DisplayGraph();
    }

    private void btn_close_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void btn_export_Click(object sender, EventArgs e)
    {
      ExportGraphToFile();
    }

    private void btn_verifyVersionsIntegrity_Click(object sender, EventArgs e)
    {
      VersionsIntegrityCheckResult versionsIntegrityCheckResult =
        _projectDependenciesModel.VerifyVersionsIntegrity();

      string message;
      MessageBoxIcon icon;

      if (versionsIntegrityCheckResult.AreVersionsIntegral)
      {
        message = "Versions are integral.";
        icon = MessageBoxIcon.Information;
      }
      else
      {
        message = "Versions are not integral.";
        icon = MessageBoxIcon.Warning;
      }

      MessageBox.Show(message, "Information", MessageBoxButtons.OK, icon);
    }

    #endregion

    #region Private helper methods

    private static void ObtainAvailableExporters()
    {
      IEnumerable<Type> exportersTypes =
        Assembly.GetExecutingAssembly().GetTypes()
          .Where(t => typeof(IProjectDependenciesModelExporter).IsAssignableFrom(t));

      foreach (Type exportersType in exportersTypes)
      {
        if (exportersType.IsInterface)
        {
          continue;
        }

        IProjectDependenciesModelExporter exporter =
           (IProjectDependenciesModelExporter)Activator.CreateInstance(exportersType);

        _availableExporters.Add(exporter.ExportFormatName, exporter);
      }
    }

    private static IProjectDependenciesModelExporter GetExporter(string exporterFormatName)
    {
      if (!_availableExporters.ContainsKey(exporterFormatName))
      {
        throw new ArgumentException(string.Format("Exporter with format '{0}' is not available.", exporterFormatName));
      }

      IProjectDependenciesModelExporter exporter = _availableExporters[exporterFormatName];

      return exporter;
    }

    private void PopulateExportFormats()
    {
      cb_exportFormat.Items.Clear();

      foreach (IProjectDependenciesModelExporter exporter in _availableExporters.Values)
      {
        cb_exportFormat.Items.Add(exporter.ExportFormatName);
      }

      if (cb_exportFormat.Items.Count > 0)
      {
        cb_exportFormat.SelectedIndex = 0;
      }
    }

    private void BuildGraph()
    {
      var projectDependenciesGraphBuilder = new ProjectDependenciesGraphBuilder();

      _projectDependenciesGraph =
        projectDependenciesGraphBuilder.BuildGraph(_projectDependenciesModel);
    }

    private void DisplayGraph()
    {
      _projectDependenciesGraphLayoutControl.Graph = _projectDependenciesGraph;
    }

    private void ExportGraphToFile()
    {
      string exporterFormatName = (string)cb_exportFormat.SelectedItem;
      IProjectDependenciesModelExporter exporter = GetExporter(exporterFormatName);
      string exporterFileExtension = exporter.ExportedFileExtension;

      saveFileDialog_export.FileName = _projectDependenciesModel.RootProjectInfo.ProjectName;

      saveFileDialog_export.Filter =
        string.Format("{0} (*.{1})|*.dgml", exporterFormatName, exporterFileExtension);

      if (saveFileDialog_export.ShowDialog() == DialogResult.OK)
      {
        exporter.Export(_projectDependenciesModel, saveFileDialog_export.FileName);

        MessageBox.Show("Done.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    #endregion
  }
}
