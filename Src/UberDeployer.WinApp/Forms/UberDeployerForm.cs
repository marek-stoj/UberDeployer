using System;
using System.Windows.Forms;
using UberDeployer.WinApp.Utils;

namespace UberDeployer.WinApp.Forms
{
  public class UberDeployerForm : Form
  {
    private readonly object _indeterminateProgressMutex = new object();
    private int _indeterminateProgressCounter;

    protected void ToggleIndeterminateProgress(bool visible, Control indeterminateProgressControl)
    {
      lock (_indeterminateProgressMutex)
      {
        bool shouldToggle = false;

        if (visible)
        {
          _indeterminateProgressCounter++;
          shouldToggle = true;
        }
        else
        {
          _indeterminateProgressCounter--;

          if (_indeterminateProgressCounter == 0)
          {
            shouldToggle = true;
          }
        }

        if (shouldToggle)
        {
          GuiUtils.BeginInvoke(this, () => { indeterminateProgressControl.Visible = visible; });
        }
      }
    }

    protected void HandleThreadException(Exception exception)
    {
      GuiUtils.BeginInvoke(this, () => { throw exception; });
    }
  }
}
