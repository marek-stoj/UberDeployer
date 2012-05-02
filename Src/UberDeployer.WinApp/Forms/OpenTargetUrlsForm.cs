using System;
using System.Collections.Generic;
using System.Windows.Forms;
using UberDeployer.WinApp.Utils;

namespace UberDeployer.WinApp.Forms
{
  public partial class OpenTargetUrlsForm : Form
  {
    public OpenTargetUrlsForm(IEnumerable<string> targetUrls)
    {
      InitializeComponent();

      lst_targetUrls.Items.Clear();

      foreach (string targetUrl in targetUrls)
      {
        lst_targetUrls.Items.Add(targetUrl);
      }

      if (lst_targetUrls.Items.Count > 0)
      {
        lst_targetUrls.SelectedIndex = 0;
      }
    }

    public OpenTargetUrlsForm()
      : this(new List<string>())
    {
    }

    private void btn_close_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void btn_open_Click(object sender, EventArgs e)
    {
      OpenSelectedUrls();
      Close();
    }

    private void btn_selectAll_Click(object sender, EventArgs e)
    {
      lst_targetUrls.SelectedIndices.Clear();

      for (int i = 0; i < lst_targetUrls.Items.Count; i++)
      {
        lst_targetUrls.SelectedIndices.Add(i);
      }
    }

    private void btn_selectNone_Click(object sender, EventArgs e)
    {
      lst_targetUrls.SelectedItems.Clear();
    }

    private void lst_targetUrls_SelectedIndexChanged(object sender, EventArgs e)
    {
      btn_open.Enabled = lst_targetUrls.SelectedIndices.Count > 0;
    }

    private void OpenTargetUrlsForm_KeyUp(object sender, KeyEventArgs e)
    {
      switch (e.KeyCode)
      {
        case Keys.Escape:
          Close();
          break;

        case Keys.Enter:
          OpenSelectedUrls();
          Close();
          break;
      }
    }

    private void OpenTargetUrlsForm_Load(object sender, EventArgs e)
    {
      lst_targetUrls.Focus();
    }

    private void OpenSelectedUrls()
    {
      foreach (string targetUrl in lst_targetUrls.SelectedItems)
      {
        SystemUtils.OpenUrl(targetUrl);
      }
    }
  }
}
