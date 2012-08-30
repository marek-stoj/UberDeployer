namespace UberDeployer.WinApp.Forms
{
  partial class DeploymentAuditForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeploymentAuditForm));
      this.pic_indeterminateProgress = new System.Windows.Forms.PictureBox();
      this.grp_deploymentRequests = new System.Windows.Forms.GroupBox();
      this.dgv_deploymentRequests = new UberDeployer.WinApp.CustomControls.MyDataGridView();
      this.btn_close = new System.Windows.Forms.Button();
      this.btn_reloadDeploymentRequests = new System.Windows.Forms.Button();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.DateFinishedColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.RequesterColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.ProjectColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.TargetEnvironmentNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.SuccessfulColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      ((System.ComponentModel.ISupportInitialize)(this.pic_indeterminateProgress)).BeginInit();
      this.grp_deploymentRequests.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgv_deploymentRequests)).BeginInit();
      this.menuStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // pic_indeterminateProgress
      // 
      this.pic_indeterminateProgress.Image = global::UberDeployer.WinApp.Properties.Resources.indeterminate_progress;
      this.pic_indeterminateProgress.Location = new System.Drawing.Point(841, 4);
      this.pic_indeterminateProgress.Name = "pic_indeterminateProgress";
      this.pic_indeterminateProgress.Size = new System.Drawing.Size(16, 16);
      this.pic_indeterminateProgress.TabIndex = 9;
      this.pic_indeterminateProgress.TabStop = false;
      this.pic_indeterminateProgress.Visible = false;
      // 
      // grp_deploymentRequests
      // 
      this.grp_deploymentRequests.Controls.Add(this.dgv_deploymentRequests);
      this.grp_deploymentRequests.Location = new System.Drawing.Point(12, 42);
      this.grp_deploymentRequests.Name = "grp_deploymentRequests";
      this.grp_deploymentRequests.Padding = new System.Windows.Forms.Padding(10);
      this.grp_deploymentRequests.Size = new System.Drawing.Size(838, 473);
      this.grp_deploymentRequests.TabIndex = 10;
      this.grp_deploymentRequests.TabStop = false;
      this.grp_deploymentRequests.Text = "Deployment requests";
      // 
      // dgv_deploymentRequests
      // 
      this.dgv_deploymentRequests.AllowUserToAddRows = false;
      this.dgv_deploymentRequests.AllowUserToDeleteRows = false;
      this.dgv_deploymentRequests.AllowUserToResizeRows = false;
      this.dgv_deploymentRequests.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.dgv_deploymentRequests.BackgroundColor = System.Drawing.SystemColors.Window;
      this.dgv_deploymentRequests.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgv_deploymentRequests.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DateFinishedColumn,
            this.RequesterColumn,
            this.ProjectColumn,
            this.TargetEnvironmentNameColumn,
            this.SuccessfulColumn});
      this.dgv_deploymentRequests.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dgv_deploymentRequests.Location = new System.Drawing.Point(10, 23);
      this.dgv_deploymentRequests.MultiSelect = false;
      this.dgv_deploymentRequests.Name = "dgv_deploymentRequests";
      this.dgv_deploymentRequests.ReadOnly = true;
      this.dgv_deploymentRequests.RowHeadersVisible = false;
      this.dgv_deploymentRequests.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dgv_deploymentRequests.Size = new System.Drawing.Size(818, 440);
      this.dgv_deploymentRequests.TabIndex = 15;
      // 
      // btn_close
      // 
      this.btn_close.Location = new System.Drawing.Point(775, 521);
      this.btn_close.Name = "btn_close";
      this.btn_close.Size = new System.Drawing.Size(75, 23);
      this.btn_close.TabIndex = 11;
      this.btn_close.Text = "Close";
      this.btn_close.UseVisualStyleBackColor = true;
      this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
      // 
      // btn_reloadDeploymentRequests
      // 
      this.btn_reloadDeploymentRequests.Location = new System.Drawing.Point(12, 521);
      this.btn_reloadDeploymentRequests.Name = "btn_reloadDeploymentRequests";
      this.btn_reloadDeploymentRequests.Size = new System.Drawing.Size(75, 23);
      this.btn_reloadDeploymentRequests.TabIndex = 12;
      this.btn_reloadDeploymentRequests.Text = "Refresh";
      this.btn_reloadDeploymentRequests.UseVisualStyleBackColor = true;
      this.btn_reloadDeploymentRequests.Click += new System.EventHandler(this.btn_reloadDeploymentRequests_Click);
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(862, 24);
      this.menuStrip1.TabIndex = 13;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "&File";
      // 
      // exitToolStripMenuItem
      // 
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
      this.exitToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
      this.exitToolStripMenuItem.Text = "E&xit";
      this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
      // 
      // DateFinishedColumn
      // 
      this.DateFinishedColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.DateFinishedColumn.DataPropertyName = "Date";
      this.DateFinishedColumn.HeaderText = "Date";
      this.DateFinishedColumn.Name = "DateFinishedColumn";
      this.DateFinishedColumn.ReadOnly = true;
      this.DateFinishedColumn.Width = 55;
      // 
      // RequesterColumn
      // 
      this.RequesterColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.RequesterColumn.DataPropertyName = "Requester";
      this.RequesterColumn.HeaderText = "Requester";
      this.RequesterColumn.Name = "RequesterColumn";
      this.RequesterColumn.ReadOnly = true;
      this.RequesterColumn.Width = 81;
      // 
      // ProjectColumn
      // 
      this.ProjectColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.ProjectColumn.DataPropertyName = "Project";
      this.ProjectColumn.HeaderText = "Project name";
      this.ProjectColumn.Name = "ProjectColumn";
      this.ProjectColumn.ReadOnly = true;
      // 
      // TargetEnvironmentNameColumn
      // 
      this.TargetEnvironmentNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.TargetEnvironmentNameColumn.DataPropertyName = "Environment";
      this.TargetEnvironmentNameColumn.HeaderText = "Environment";
      this.TargetEnvironmentNameColumn.Name = "TargetEnvironmentNameColumn";
      this.TargetEnvironmentNameColumn.ReadOnly = true;
      this.TargetEnvironmentNameColumn.Width = 91;
      // 
      // SuccessfulColumn
      // 
      this.SuccessfulColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.SuccessfulColumn.DataPropertyName = "Successful";
      this.SuccessfulColumn.HeaderText = "Successful";
      this.SuccessfulColumn.Name = "SuccessfulColumn";
      this.SuccessfulColumn.ReadOnly = true;
      this.SuccessfulColumn.Width = 84;
      // 
      // DeploymentAuditForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(862, 555);
      this.Controls.Add(this.btn_reloadDeploymentRequests);
      this.Controls.Add(this.btn_close);
      this.Controls.Add(this.grp_deploymentRequests);
      this.Controls.Add(this.pic_indeterminateProgress);
      this.Controls.Add(this.menuStrip1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.KeyPreview = true;
      this.MainMenuStrip = this.menuStrip1;
      this.MaximizeBox = false;
      this.Name = "DeploymentAuditForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Deployment audit";
      this.Load += new System.EventHandler(this.DeploymentAuditForm_Load);
      this.Shown += new System.EventHandler(this.DeploymentAuditForm_Shown);
      ((System.ComponentModel.ISupportInitialize)(this.pic_indeterminateProgress)).EndInit();
      this.grp_deploymentRequests.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dgv_deploymentRequests)).EndInit();
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PictureBox pic_indeterminateProgress;
    private System.Windows.Forms.GroupBox grp_deploymentRequests;
    private CustomControls.MyDataGridView dgv_deploymentRequests;
    private System.Windows.Forms.Button btn_close;
    private System.Windows.Forms.Button btn_reloadDeploymentRequests;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    private System.Windows.Forms.DataGridViewTextBoxColumn DateFinishedColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn RequesterColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn ProjectColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn TargetEnvironmentNameColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn SuccessfulColumn;
  }
}

