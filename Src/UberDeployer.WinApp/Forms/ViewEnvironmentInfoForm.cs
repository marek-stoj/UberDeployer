using System;
using System.Windows.Forms;
using UberDeployer.Agent.Proxy.Dto;

namespace UberDeployer.WinApp.Forms
{
  public partial class ViewEnvironmentInfoForm : Form
  {
    #region Constructor(s)

    public ViewEnvironmentInfoForm()
    {
      InitializeComponent();
    }

    #endregion

    #region Public methods

    public void LoadEnvironmentInfo(EnvironmentInfo environmentInfo)
    {
      if (environmentInfo == null)
      {
        throw new ArgumentNullException("environmentInfo");
      }

      Text = "Environment: " + environmentInfo.Name;
      environmentPropertiesPropertyGrid.SelectedObject = environmentInfo;
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
