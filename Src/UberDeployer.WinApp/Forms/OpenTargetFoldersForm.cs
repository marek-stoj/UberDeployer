using System;
using System.Collections.Generic;
using System.Windows.Forms;
using UberDeployer.WinApp.Utils;

namespace UberDeployer.WinApp.Forms
{
  public partial class OpenTargetFoldersForm : Form
  {
    public OpenTargetFoldersForm(IEnumerable<string> targetFolders)
    {
      InitializeComponent();

      lst_targetFolders.Items.Clear();

      foreach (string targetFolder in targetFolders)
      {
        lst_targetFolders.Items.Add(targetFolder);
      }

      if (lst_targetFolders.Items.Count > 0)
      {
        lst_targetFolders.SelectedIndex = 0;
      }
    }

    public OpenTargetFoldersForm()
      : this(new List<string>())
    {
    }

    private void btn_close_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void btn_open_Click(object sender, EventArgs e)
    {
      OpenSelectedFolders();
      Close();
    }

    private void btn_selectAll_Click(object sender, EventArgs e)
    {
      lst_targetFolders.SelectedIndices.Clear();

      for (int i = 0; i < lst_targetFolders.Items.Count; i++)
      {
        lst_targetFolders.SelectedIndices.Add(i);
      }
    }

    private void btn_selectNone_Click(object sender, EventArgs e)
    {
      lst_targetFolders.SelectedItems.Clear();
    }

    private void lst_targetFolders_SelectedIndexChanged(object sender, EventArgs e)
    {
      btn_open.Enabled = lst_targetFolders.SelectedIndices.Count > 0;
    }

    private void OpenTargetFoldersForm_KeyUp(object sender, KeyEventArgs e)
    {
      switch (e.KeyCode)
      {
        case Keys.Escape:
          Close();
          break;

        case Keys.Enter:
          OpenSelectedFolders();
          Close();
          break;
      }
    }

    private void OpenTargetFoldersForm_Load(object sender, EventArgs e)
    {
      lst_targetFolders.Focus();
    }

    private void OpenSelectedFolders()
    {
      foreach (string targetFolder in lst_targetFolders.SelectedItems)
      {
        SystemUtils.OpenFolder(targetFolder);
      }
    }
  }
}
