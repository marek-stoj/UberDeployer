using System;
using System.ComponentModel;
using System.Windows.Forms;
using ProjectDepsVisualizer.Core;
using System.Xml.Linq;
using System.IO;

namespace ProjectDepsVisualizer.UI
{
  public partial class MainForm : Form
  {
    #region Constructor(s)

    public MainForm()
    {
      InitializeComponent();
    }

    #endregion

    #region Public methods

    public void SetProjectConfiguration(string projectName, string projectConfigurationName)
    {
      if (string.IsNullOrEmpty(projectName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "projectName");
      }

      if (string.IsNullOrEmpty(projectConfigurationName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "projectConfigurationName");
      }

      txt_projectName.Text = projectName;
      txt_projectConfiguration.Text = projectConfigurationName;
    }

    #endregion

    #region WinForms event handlers

    private void MainForm_Load(object sender, EventArgs e)
    {
      // do nothing
    }

    private void MainForm_Shown(object sender, EventArgs e)
    {
      txt_projectName.Focus();
    }

    private void btn_close_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void btn_buildDependencyGraph_Click(object sender, EventArgs e)
    {
      var buildGraphBackgroundWorker = new BackgroundWorker();

      buildGraphBackgroundWorker.WorkerSupportsCancellation = false;
      buildGraphBackgroundWorker.DoWork += buildGraphBackgroundWorker_DoWork;
      buildGraphBackgroundWorker.RunWorkerCompleted += buildGraphBackgroundWorker_RunWorkerCompleted;

      ToggleFormEnabled(false);
      buildGraphBackgroundWorker.RunWorkerAsync();
    }

    #endregion

    #region Other event handlers

    private void buildGraphBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      var svnClient =
        new SvnClient(
          txt_svnExeFilePath.Text,
          txt_svnRepositoryBaseUrl.Text,
          txt_svnUserName.Text,
          txt_svnPassword.Text);


      var projectDependenciesModelBuilder = new ProjectDependenciesModelBuilder(svnClient);

      projectDependenciesModelBuilder.LogMessagePosted += projectDependenciesModelBuilder_LogMessagePosted;

      var projectDependenciesModel =
        projectDependenciesModelBuilder
          .BuildModel(
            txt_projectName.Text,
            txt_projectConfiguration.Text);

      GuiUtils.BeginInvoke(
        this,
        () =>
          {
            new ProjectDependenciesModelForm(projectDependenciesModel)
              .Show();
          });
    }

    private void buildWhoDependsOnGraphBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      var svnClient =
        new SvnClient(
          txt_svnExeFilePath.Text,
          txt_svnRepositoryBaseUrl.Text,
          txt_svnUserName.Text,
          txt_svnPassword.Text);


      var projectDependenciesModelBuilder = new ProjectDependenciesModelBuilder(svnClient);

      projectDependenciesModelBuilder.LogMessagePosted += projectDependenciesModelBuilder_LogMessagePosted;

      var projectDependenciesModel =
        projectDependenciesModelBuilder
          .BuildWhoDependsOnModel(
            
            txt_projectName.Text,
            txt_projectConfiguration.Text);

      GuiUtils.BeginInvoke(
        this,
        () =>
        {
          new ProjectDependenciesModelForm(projectDependenciesModel)
            .Show();
        });
    }

    private void buildGraphBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      ToggleFormEnabled(true);
    }

    private void buildWhoDependsGraphBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      ToggleFormEnabled(true);
    }

    private void projectDependenciesModelBuilder_LogMessagePosted(object sender, LogMessageEventArgs e)
    {
      GuiUtils.BeginInvoke(
        this,
        () =>
          {
            txt_log.AppendText(
              string.Format(
                "[{0}] {1}{2}",
                DateTime.Now.ToShortTimeString(),
                e.Message,
                Environment.NewLine));
          });
    }

    #endregion

    #region Private helper methods

    private void ToggleFormEnabled(bool enabled)
    {
      gb_svn.Enabled = enabled;
      gb_project.Enabled = enabled;
      btn_buildDependencyGraph.Enabled = enabled;
    }

    #endregion
  }
}
