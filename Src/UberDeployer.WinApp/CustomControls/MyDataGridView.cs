using System.Windows.Forms;

namespace UberDeployer.WinApp.CustomControls
{
  public class MyDataGridView : DataGridView
  {
    private const uint WM_MOUSEWHEEL = 0x20a;

    protected override void WndProc(ref Message m)
    {
      if (m.Msg == WM_MOUSEWHEEL)
      {
        int wheelDelta = ((int)m.WParam) >> 16;
        int newFirstDisplayedScrollingRowIndex = FirstDisplayedScrollingRowIndex - (wheelDelta / 120);

        if (newFirstDisplayedScrollingRowIndex < 0)
        {
          newFirstDisplayedScrollingRowIndex = 0;
        }
        else if (newFirstDisplayedScrollingRowIndex >= Rows.Count)
        {
          newFirstDisplayedScrollingRowIndex = Rows.Count - 1;
        }

        FirstDisplayedScrollingRowIndex = newFirstDisplayedScrollingRowIndex;
      }
      else
      {
        base.WndProc(ref m);
      }
    }
  }
}
