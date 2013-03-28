using System;
using System.Windows.Forms;
using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Core.Deployment
{
  // TODO IMM HI: move to WinApp (after using real DI solution)
  public class DialogPromptPasswordCollector : IPasswordCollector
  {
    public event EventHandler<DiagnosticMessageEventArgs> DiagnosticMessagePosted;

    #region IPasswordCollector members

    public string CollectPasswordForUser(Guid deploymentId, string environmentName, string machineName, string userName)
    {
      Guard.NotEmpty(deploymentId, "deploymentId");
      Guard.NotNullNorEmpty(environmentName, "environmentName");
      Guard.NotNullNorEmpty(machineName, "machineName");
      Guard.NotNullNorEmpty(userName, "userName");

      var passwordPromptForm =
        new PasswordPromptForm
          {
            EnvironmentName = environmentName,
            MachineName = machineName,
            UserName = userName,
          };

      DialogResult dialogResult =
        passwordPromptForm
          .ShowDialog();

      if (dialogResult == DialogResult.Cancel)
      {
        return null;
      }

      return passwordPromptForm.EnteredPassword;
    }

    #endregion
  }
}
