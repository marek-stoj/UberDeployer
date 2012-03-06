using System;
using System.Windows.Forms;

namespace UberDeployer.WinApp
{
  public static class GuiUtils
  {
    #region Public methods

    public static void Invoke(Control control, MethodInvoker method)
    {
      if (control == null)
      {
        throw new ArgumentNullException("control");
      }

      if (method == null)
      {
        throw new ArgumentNullException("method");
      }

      if (control.InvokeRequired)
      {
        control.Invoke(method, null);
      }
      else
      {
        method();
      }
    }

    public static void BeginInvoke(Control control, MethodInvoker method)
    {
      if (control == null)
      {
        throw new ArgumentNullException("control");
      }

      if (method == null)
      {
        throw new ArgumentNullException("method");
      }

      if (control.InvokeRequired)
      {
        control.BeginInvoke(method, null);
      }
      else
      {
        method();
      }
    }

    #endregion
  }
}
