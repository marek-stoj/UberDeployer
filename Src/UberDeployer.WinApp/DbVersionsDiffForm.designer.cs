namespace UberDeployer.WinApp
{
  partial class DbVersionsDiffForm
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
      this.btn_close = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.dgv_environments = new UberDeployer.WinApp.CustomControls.MyDataGridView();
      this.EnvironmentNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.DatabaseServerColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.pic_indeterminateProgress = new System.Windows.Forms.PictureBox();
      this.btn_diff = new System.Windows.Forms.Button();
      this.dgv_databasesInEnvironments = new System.Windows.Forms.DataGridView();
      this.cb_differencesOnly = new System.Windows.Forms.CheckBox();
      this.gb_diffResults = new System.Windows.Forms.GroupBox();
      this.dgv_databasesVersions = new System.Windows.Forms.DataGridView();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgv_environments)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pic_indeterminateProgress)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.dgv_databasesInEnvironments)).BeginInit();
      this.gb_diffResults.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgv_databasesVersions)).BeginInit();
      this.SuspendLayout();
      // 
      // btn_close
      // 
      this.btn_close.Location = new System.Drawing.Point(631, 471);
      this.btn_close.Name = "btn_close";
      this.btn_close.Size = new System.Drawing.Size(75, 23);
      this.btn_close.TabIndex = 12;
      this.btn_close.Text = "Close";
      this.btn_close.UseVisualStyleBackColor = true;
      this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.dgv_environments);
      this.groupBox1.Location = new System.Drawing.Point(12, 35);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Padding = new System.Windows.Forms.Padding(10);
      this.groupBox1.Size = new System.Drawing.Size(252, 453);
      this.groupBox1.TabIndex = 13;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Environments";
      // 
      // dgv_environments
      // 
      this.dgv_environments.AllowUserToAddRows = false;
      this.dgv_environments.AllowUserToDeleteRows = false;
      this.dgv_environments.AllowUserToResizeRows = false;
      this.dgv_environments.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.dgv_environments.BackgroundColor = System.Drawing.SystemColors.Window;
      this.dgv_environments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgv_environments.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.EnvironmentNameColumn,
            this.DatabaseServerColumn});
      this.dgv_environments.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dgv_environments.Location = new System.Drawing.Point(10, 23);
      this.dgv_environments.Name = "dgv_environments";
      this.dgv_environments.ReadOnly = true;
      this.dgv_environments.RowHeadersVisible = false;
      this.dgv_environments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dgv_environments.Size = new System.Drawing.Size(232, 420);
      this.dgv_environments.TabIndex = 18;
      this.dgv_environments.SelectionChanged += new System.EventHandler(this.dgv_environments_SelectionChanged);
      // 
      // EnvironmentNameColumn
      // 
      this.EnvironmentNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.EnvironmentNameColumn.DataPropertyName = "Name";
      this.EnvironmentNameColumn.HeaderText = "Name";
      this.EnvironmentNameColumn.Name = "EnvironmentNameColumn";
      this.EnvironmentNameColumn.ReadOnly = true;
      // 
      // DatabaseServerColumn
      // 
      this.DatabaseServerColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.DatabaseServerColumn.DataPropertyName = "Server";
      this.DatabaseServerColumn.HeaderText = "Server";
      this.DatabaseServerColumn.Name = "DatabaseServerColumn";
      this.DatabaseServerColumn.ReadOnly = true;
      this.DatabaseServerColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
      // 
      // pic_indeterminateProgress
      // 
      this.pic_indeterminateProgress.Image = global::UberDeployer.WinApp.Properties.Resources.indeterminate_progress;
      this.pic_indeterminateProgress.Location = new System.Drawing.Point(1046, 4);
      this.pic_indeterminateProgress.Name = "pic_indeterminateProgress";
      this.pic_indeterminateProgress.Size = new System.Drawing.Size(16, 16);
      this.pic_indeterminateProgress.TabIndex = 14;
      this.pic_indeterminateProgress.TabStop = false;
      this.pic_indeterminateProgress.Visible = false;
      // 
      // btn_diff
      // 
      this.btn_diff.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.btn_diff.Location = new System.Drawing.Point(121, 16);
      this.btn_diff.Name = "btn_diff";
      this.btn_diff.Size = new System.Drawing.Size(114, 34);
      this.btn_diff.TabIndex = 15;
      this.btn_diff.Text = "Diff";
      this.btn_diff.UseVisualStyleBackColor = true;
      this.btn_diff.Click += new System.EventHandler(this.btn_diff_Click);
      // 
      // dgv_databasesInEnvironments
      // 
      this.dgv_databasesInEnvironments.AllowUserToAddRows = false;
      this.dgv_databasesInEnvironments.AllowUserToDeleteRows = false;
      this.dgv_databasesInEnvironments.AllowUserToResizeRows = false;
      this.dgv_databasesInEnvironments.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
      this.dgv_databasesInEnvironments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgv_databasesInEnvironments.Location = new System.Drawing.Point(13, 56);
      this.dgv_databasesInEnvironments.MultiSelect = false;
      this.dgv_databasesInEnvironments.Name = "dgv_databasesInEnvironments";
      this.dgv_databasesInEnvironments.ReadOnly = true;
      this.dgv_databasesInEnvironments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dgv_databasesInEnvironments.Size = new System.Drawing.Size(408, 381);
      this.dgv_databasesInEnvironments.TabIndex = 16;
      this.dgv_databasesInEnvironments.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgv_databasesInEnvironments_CellFormatting);
      this.dgv_databasesInEnvironments.SelectionChanged += new System.EventHandler(this.dgv_databasesInEnvironments_SelectionChanged);
      // 
      // cb_differencesOnly
      // 
      this.cb_differencesOnly.AutoSize = true;
      this.cb_differencesOnly.Checked = true;
      this.cb_differencesOnly.CheckState = System.Windows.Forms.CheckState.Checked;
      this.cb_differencesOnly.Location = new System.Drawing.Point(13, 26);
      this.cb_differencesOnly.Name = "cb_differencesOnly";
      this.cb_differencesOnly.Size = new System.Drawing.Size(102, 17);
      this.cb_differencesOnly.TabIndex = 17;
      this.cb_differencesOnly.Text = "Differences only";
      this.cb_differencesOnly.UseVisualStyleBackColor = true;
      // 
      // gb_diffResults
      // 
      this.gb_diffResults.Controls.Add(this.dgv_databasesVersions);
      this.gb_diffResults.Controls.Add(this.cb_differencesOnly);
      this.gb_diffResults.Controls.Add(this.btn_diff);
      this.gb_diffResults.Controls.Add(this.dgv_databasesInEnvironments);
      this.gb_diffResults.Controls.Add(this.btn_close);
      this.gb_diffResults.Location = new System.Drawing.Point(270, 35);
      this.gb_diffResults.Name = "gb_diffResults";
      this.gb_diffResults.Padding = new System.Windows.Forms.Padding(10);
      this.gb_diffResults.Size = new System.Drawing.Size(792, 453);
      this.gb_diffResults.TabIndex = 18;
      this.gb_diffResults.TabStop = false;
      this.gb_diffResults.Text = "Diff results";
      // 
      // dgv_databasesVersions
      // 
      this.dgv_databasesVersions.AllowUserToAddRows = false;
      this.dgv_databasesVersions.AllowUserToDeleteRows = false;
      this.dgv_databasesVersions.AllowUserToResizeRows = false;
      this.dgv_databasesVersions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
      this.dgv_databasesVersions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgv_databasesVersions.Location = new System.Drawing.Point(427, 56);
      this.dgv_databasesVersions.MultiSelect = false;
      this.dgv_databasesVersions.Name = "dgv_databasesVersions";
      this.dgv_databasesVersions.ReadOnly = true;
      this.dgv_databasesVersions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dgv_databasesVersions.Size = new System.Drawing.Size(352, 381);
      this.dgv_databasesVersions.TabIndex = 19;
      this.dgv_databasesVersions.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgv_databasesVersions_CellFormatting);
      // 
      // menuStrip1
      // 
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(1074, 24);
      this.menuStrip1.TabIndex = 19;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // DbVersionsDiffForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1074, 497);
      this.Controls.Add(this.gb_diffResults);
      this.Controls.Add(this.pic_indeterminateProgress);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.menuStrip1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MainMenuStrip = this.menuStrip1;
      this.MaximizeBox = false;
      this.Name = "DbVersionsDiffForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Database versions diff";
      this.Load += new System.EventHandler(this.DbVersionsDiffForm_Load);
      this.groupBox1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dgv_environments)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pic_indeterminateProgress)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.dgv_databasesInEnvironments)).EndInit();
      this.gb_diffResults.ResumeLayout(false);
      this.gb_diffResults.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgv_databasesVersions)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btn_close;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.PictureBox pic_indeterminateProgress;
    private System.Windows.Forms.Button btn_diff;
    private System.Windows.Forms.DataGridView dgv_databasesInEnvironments;
    private System.Windows.Forms.CheckBox cb_differencesOnly;
    private CustomControls.MyDataGridView dgv_environments;
    private System.Windows.Forms.DataGridViewTextBoxColumn EnvironmentNameColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn DatabaseServerColumn;
    private System.Windows.Forms.GroupBox gb_diffResults;
    private System.Windows.Forms.DataGridView dgv_databasesVersions;
    private System.Windows.Forms.MenuStrip menuStrip1;
  }
}