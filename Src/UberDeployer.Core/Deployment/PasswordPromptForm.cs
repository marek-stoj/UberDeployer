using System;
using System.Windows.Forms;

namespace UberDeployer.Core.Deployment
{
  // TODO IMM HI: this should not be in the core
  public partial class PasswordPromptForm : Form
  {
    private string _enteredPassword;

    public PasswordPromptForm()
    {
      InitializeComponent();
    }

    private void PasswordPromptForm_Shown(object sender, EventArgs e)
    {
      txt_password.Focus();
    }

    private void PasswordPromptForm_KeyDown(object sender, KeyEventArgs e)
    {
      switch (e.KeyCode)
      {
        case Keys.Escape:
          Cancel();
          break;

        case Keys.Enter:
          Ok();
          break;
      }
    }

    private void btn_ok_Click(object sender, EventArgs e)
    {
      Ok();
    }

    private void btn_cancel_Click(object sender, EventArgs e)
    {
      Cancel();
    }

    private void Ok()
    {
      _enteredPassword = txt_password.Text;

      DialogResult = DialogResult.OK;
      Close();
    }

    private void Cancel()
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }

    public string EnvironmentName
    {
      get { return txt_environmentName.Text; }
      set { txt_environmentName.Text = value ?? ""; }
    }

    public string MachineName
    {
      get { return txt_machineName.Text; }
      set { txt_machineName.Text = value ?? ""; }
    }

    public string UserName
    {
      get { return txt_userName.Text; }
      set { txt_userName.Text = value ?? ""; }
    }

    public string EnteredPassword
    {
      get { return _enteredPassword; }
    }
  }
}
