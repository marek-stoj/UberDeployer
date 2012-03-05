using System;
using System.Windows.Forms;

namespace UberDeployer.Core.Deployment
{
  // TODO IMM HI: move to WinApp (after using real DI solution)
  public class DialogPromptPasswordCollector : IPasswordCollector
  {
    #region IPasswordCollector members

    public string CollectPasswordForUser(string environmentName, string userName)
    {
      if (string.IsNullOrEmpty(environmentName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "environmentName");
      }

      if (string.IsNullOrEmpty(userName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "userName");
      }

      var passwordPromptForm =
        new PasswordPromptForm
          {
            EnvironmentName = environmentName,
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
