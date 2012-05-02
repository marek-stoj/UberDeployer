namespace UberDeployer.WinApp.Forms
{
  partial class OpenTargetFoldersForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.lst_targetFolders = new System.Windows.Forms.ListBox();
      this.btn_close = new System.Windows.Forms.Button();
      this.btn_open = new System.Windows.Forms.Button();
      this.panel1 = new System.Windows.Forms.Panel();
      this.panel2 = new System.Windows.Forms.Panel();
      this.btn_selectAll = new System.Windows.Forms.Button();
      this.btn_selectNone = new System.Windows.Forms.Button();
      this.groupBox1.SuspendLayout();
      this.panel1.SuspendLayout();
      this.panel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.lst_targetFolders);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Location = new System.Drawing.Point(0, 0);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Padding = new System.Windows.Forms.Padding(10);
      this.groupBox1.Size = new System.Drawing.Size(400, 206);
      this.groupBox1.TabIndex = 3;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Select folder(s) to open";
      // 
      // lst_targetFolders
      // 
      this.lst_targetFolders.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lst_targetFolders.FormattingEnabled = true;
      this.lst_targetFolders.Location = new System.Drawing.Point(10, 23);
      this.lst_targetFolders.Name = "lst_targetFolders";
      this.lst_targetFolders.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
      this.lst_targetFolders.Size = new System.Drawing.Size(380, 173);
      this.lst_targetFolders.TabIndex = 1;
      this.lst_targetFolders.SelectedIndexChanged += new System.EventHandler(this.lst_targetFolders_SelectedIndexChanged);
      // 
      // btn_close
      // 
      this.btn_close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btn_close.Location = new System.Drawing.Point(313, 13);
      this.btn_close.Name = "btn_close";
      this.btn_close.Size = new System.Drawing.Size(75, 23);
      this.btn_close.TabIndex = 5;
      this.btn_close.Text = "Close";
      this.btn_close.UseVisualStyleBackColor = true;
      this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
      // 
      // btn_open
      // 
      this.btn_open.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btn_open.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.btn_open.Location = new System.Drawing.Point(232, 13);
      this.btn_open.Name = "btn_open";
      this.btn_open.Size = new System.Drawing.Size(75, 23);
      this.btn_open.TabIndex = 4;
      this.btn_open.Text = "Open";
      this.btn_open.UseVisualStyleBackColor = true;
      this.btn_open.Click += new System.EventHandler(this.btn_open_Click);
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btn_selectNone);
      this.panel1.Controls.Add(this.btn_selectAll);
      this.panel1.Controls.Add(this.btn_close);
      this.panel1.Controls.Add(this.btn_open);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(10, 216);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(400, 48);
      this.panel1.TabIndex = 6;
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.groupBox1);
      this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel2.Location = new System.Drawing.Point(10, 10);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(400, 206);
      this.panel2.TabIndex = 7;
      // 
      // btn_selectAll
      // 
      this.btn_selectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btn_selectAll.Location = new System.Drawing.Point(10, 13);
      this.btn_selectAll.Name = "btn_selectAll";
      this.btn_selectAll.Size = new System.Drawing.Size(50, 23);
      this.btn_selectAll.TabIndex = 6;
      this.btn_selectAll.Text = "All";
      this.btn_selectAll.UseVisualStyleBackColor = true;
      this.btn_selectAll.Click += new System.EventHandler(this.btn_selectAll_Click);
      // 
      // btn_selectNone
      // 
      this.btn_selectNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btn_selectNone.Location = new System.Drawing.Point(66, 13);
      this.btn_selectNone.Name = "btn_selectNone";
      this.btn_selectNone.Size = new System.Drawing.Size(50, 23);
      this.btn_selectNone.TabIndex = 7;
      this.btn_selectNone.Text = "None";
      this.btn_selectNone.UseVisualStyleBackColor = true;
      this.btn_selectNone.Click += new System.EventHandler(this.btn_selectNone_Click);
      // 
      // OpenTargetFoldersForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(420, 274);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.panel1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.KeyPreview = true;
      this.MaximizeBox = false;
      this.Name = "OpenTargetFoldersForm";
      this.Padding = new System.Windows.Forms.Padding(10);
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Open target folder(s)";
      this.Load += new System.EventHandler(this.OpenTargetFoldersForm_Load);
      this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OpenTargetFoldersForm_KeyUp);
      this.groupBox1.ResumeLayout(false);
      this.panel1.ResumeLayout(false);
      this.panel2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Button btn_close;
    private System.Windows.Forms.Button btn_open;
    private System.Windows.Forms.ListBox lst_targetFolders;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Button btn_selectNone;
    private System.Windows.Forms.Button btn_selectAll;
  }
}