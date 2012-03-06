using System;
using System.Windows.Forms;
using UberDeployer.CommonConfiguration;

namespace UberDeployer.WinApp
{
  internal static class Program
  {
    [STAThread]
    private static void Main()
    {
      Application.ThreadException += Application_ThreadException;
      AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

      Bootstraper.Bootstrap();

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new MainForm());
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      HandlException(e.ExceptionObject as Exception, e.IsTerminating);
    }

    private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
    {
      HandlException(e.Exception, false);
    }

    private static void HandlException(Exception exception, bool isTerminating)
    {
      MessageBoxButtons messageBoxButtons;

      string message =
        string.Format(
          "Error: {0}{1}{1}{2}",
          (exception != null ? exception.Message : "(no exception)."),
          Environment.NewLine,
          (exception != null ? exception.StackTrace : "(no stack trace)"));

      if (isTerminating)
      {
        messageBoxButtons = MessageBoxButtons.OK;
      }
      else
      {
        messageBoxButtons = MessageBoxButtons.YesNo;

        message +=
          string.Format(
            "Error: {0}{0}{1}",
            Environment.NewLine,
            "Do you want the app to continue running?");
      }

      var dialogResult = MessageBox.Show(message, "Unhandled exception", messageBoxButtons, MessageBoxIcon.Error);

      if (isTerminating || dialogResult == DialogResult.No)
      {
        Application.Exit();
      }
    }
  }
}
