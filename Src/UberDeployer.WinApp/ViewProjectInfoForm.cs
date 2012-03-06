using System;
using System.Windows.Forms;
using UberDeployer.Core.Domain;

namespace UberDeployer.WinApp
{
  public partial class ViewProjectInfoForm : Form
  {
    #region Constructor(s)

    public ViewProjectInfoForm()
    {
      InitializeComponent();
    }

    #endregion

    #region Public methods

    public void LoadProjectInfo(ProjectInfo projectInfo)
    {
      if (projectInfo == null)
      {
        throw new ArgumentNullException("projectInfo");
      }

      Text = "Project: " + projectInfo.Name;
      lbl_projectType.Text = projectInfo.Type;
      projectPropertiesPropertyGrid.SelectedObject = projectInfo;
    }

    #endregion

    #region WinForms event handlers

    private void ViewProjectInfoForm_KeyUp(object sender, KeyEventArgs e)
    {
      switch (e.KeyCode)
      {
        case Keys.Escape:
          Close();
          break;
      }
    }

    private void btn_close_Click(object sender, EventArgs e)
    {
      Close();
    }

    #endregion
  }
}
