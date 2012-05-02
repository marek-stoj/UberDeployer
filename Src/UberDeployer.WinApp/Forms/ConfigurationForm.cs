using System;
using System.Windows.Forms;
using UberDeployer.CommonConfiguration;
using UberDeployer.Core.Configuration;

namespace UberDeployer.WinApp.Forms
{
  public partial class ConfigurationForm : Form
  {
    #region Constructor(s)

    public ConfigurationForm()
    {
      InitializeComponent();
    }

    #endregion

    #region WinForms event handlers

    private void ConfigurationForm_Load(object sender, EventArgs e)
    {
      LoadConfiguration();
    }

    private void ConfigurationForm_KeyUp(object sender, KeyEventArgs e)
    {
      switch (e.KeyCode)
      {
        case Keys.Escape:
          Close();
          break;

        case Keys.Enter:
          SaveConfiguration();
          Close();
          break;
      }
    }

    private void btn_cancel_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void btn_save_Click(object sender, EventArgs e)
    {
      SaveConfiguration();
      Close();
    }

    #endregion

    #region Private helper methods

    private void LoadConfiguration()
    {
      IApplicationConfiguration applicationConfiguration =
        ObjectFactory.Instance.CreateApplicationConfiguration();

      txt_teamCityHostName.Text = applicationConfiguration.TeamCityHostName;
      txt_teamCityPort.Text = applicationConfiguration.TeamCityPort != 0 ? applicationConfiguration.TeamCityPort.ToString() : "";
      txt_teamCityUserName.Text = applicationConfiguration.TeamCityUserName;
      txt_teamCityPassword.Text = applicationConfiguration.TeamCityPassword;
    }

    private void SaveConfiguration()
    {
      IApplicationConfiguration applicationConfiguration =
        ObjectFactory.Instance.CreateApplicationConfiguration();

      applicationConfiguration.TeamCityHostName = txt_teamCityHostName.Text;

      int teamCityPort;

      int.TryParse(txt_teamCityPort.Text, out teamCityPort);

      applicationConfiguration.TeamCityPort = teamCityPort;
      applicationConfiguration.TeamCityUserName = txt_teamCityUserName.Text;
      applicationConfiguration.TeamCityPassword = txt_teamCityPassword.Text;

      applicationConfiguration.Save();
    }

    #endregion
  }
}
