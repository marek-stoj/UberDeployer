using System.Windows.Forms;

namespace UberDeployer.WinApp.Utils
{
  public static class AppUtils
  {
    public static void NotifyUser(string message, string caption, MessageBoxIcon messageBoxIcon)
    {
      MessageBox.Show(message, caption, MessageBoxButtons.OK, messageBoxIcon);
    }

    public static void NotifyUserInfo(string message)
    {
      NotifyUser(message, "Information", MessageBoxIcon.Information);
    }

    public static void NotifyUserInvalidOperation(string message)
    {
      NotifyUser(message, "Information", MessageBoxIcon.Warning);
    }
  }
}
