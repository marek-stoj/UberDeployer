namespace UberDeployer.WinApp.Forms
{
  partial class MainForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.txtFilter = new System.Windows.Forms.TextBox();
      this.dgv_projectInfos = new UberDeployer.WinApp.CustomControls.MyDataGridView();
      this.ProjectNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.ProjectTypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.btn_openWebApp = new System.Windows.Forms.Button();
      this.btn_openProjectTargetFolder = new System.Windows.Forms.Button();
      this.btn_showProjectInfo = new System.Windows.Forms.Button();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.btn_showDependencies = new System.Windows.Forms.Button();
      this.txtFilterConfigs = new System.Windows.Forms.TextBox();
      this.dgv_projectConfigurations = new UberDeployer.WinApp.CustomControls.MyDataGridView();
      this.ProjectConfigurationNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.ProjectConfigurationWebLinkColumn = new System.Windows.Forms.DataGridViewLinkColumn();
      this.grp_projectConfigurationBuilds = new System.Windows.Forms.GroupBox();
      this.txtFilterBuilds = new System.Windows.Forms.TextBox();
      this.dgv_projectConfigurationBuilds = new UberDeployer.WinApp.CustomControls.MyDataGridView();
      this.ProjectConfigurationBuildStartDateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.ProjectConfigurationBuildStartTimeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.ProjectConfigurationBuildNumberColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.ProjectConfigurationBuildStatusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.ProjectConfigurationBuildWebLinkColumn = new System.Windows.Forms.DataGridViewLinkColumn();
      this.txt_log = new System.Windows.Forms.RichTextBox();
      this.grpLog = new System.Windows.Forms.GroupBox();
      this.label1 = new System.Windows.Forms.Label();
      this.cb_messageTypeThreshold = new System.Windows.Forms.ComboBox();
      this.btn_clearLog = new System.Windows.Forms.Button();
      this.panel2 = new System.Windows.Forms.Panel();
      this.btn_deploy = new System.Windows.Forms.Button();
      this.cbx_targetEnvironment = new System.Windows.Forms.ComboBox();
      this.label5 = new System.Windows.Forms.Label();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmi_reloadProjects = new System.Windows.Forms.ToolStripMenuItem();
      this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.deploymentAuditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.dependenciesVisualizerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.dbVersionsDiffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.btn_showEnvironmentInfo = new System.Windows.Forms.Button();
      this.pic_indeterminateProgress = new System.Windows.Forms.PictureBox();
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgv_projectInfos)).BeginInit();
      this.groupBox3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgv_projectConfigurations)).BeginInit();
      this.grp_projectConfigurationBuilds.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgv_projectConfigurationBuilds)).BeginInit();
      this.grpLog.SuspendLayout();
      this.panel2.SuspendLayout();
      this.menuStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pic_indeterminateProgress)).BeginInit();
      this.SuspendLayout();
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.txtFilter);
      this.groupBox2.Controls.Add(this.dgv_projectInfos);
      this.groupBox2.Controls.Add(this.btn_openWebApp);
      this.groupBox2.Controls.Add(this.btn_openProjectTargetFolder);
      this.groupBox2.Controls.Add(this.btn_showProjectInfo);
      this.groupBox2.Location = new System.Drawing.Point(12, 27);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Padding = new System.Windows.Forms.Padding(10);
      this.groupBox2.Size = new System.Drawing.Size(281, 305);
      this.groupBox2.TabIndex = 1;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Deployable projects";
      // 
      // txtFilter
      // 
      this.txtFilter.Location = new System.Drawing.Point(10, 23);
      this.txtFilter.Name = "txtFilter";
      this.txtFilter.Size = new System.Drawing.Size(262, 20);
      this.txtFilter.TabIndex = 0;
      this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
      // 
      // dgv_projectInfos
      // 
      this.dgv_projectInfos.AllowUserToAddRows = false;
      this.dgv_projectInfos.AllowUserToDeleteRows = false;
      this.dgv_projectInfos.AllowUserToResizeRows = false;
      this.dgv_projectInfos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.dgv_projectInfos.BackgroundColor = System.Drawing.SystemColors.Window;
      this.dgv_projectInfos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgv_projectInfos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ProjectNameColumn,
            this.ProjectTypeColumn});
      this.dgv_projectInfos.Location = new System.Drawing.Point(10, 50);
      this.dgv_projectInfos.MultiSelect = false;
      this.dgv_projectInfos.Name = "dgv_projectInfos";
      this.dgv_projectInfos.ReadOnly = true;
      this.dgv_projectInfos.RowHeadersVisible = false;
      this.dgv_projectInfos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dgv_projectInfos.Size = new System.Drawing.Size(262, 218);
      this.dgv_projectInfos.TabIndex = 1;
      this.dgv_projectInfos.SelectionChanged += new System.EventHandler(this.dgv_projectInfos_SelectionChanged);
      // 
      // ProjectNameColumn
      // 
      this.ProjectNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.ProjectNameColumn.DataPropertyName = "Name";
      this.ProjectNameColumn.HeaderText = "Name";
      this.ProjectNameColumn.Name = "ProjectNameColumn";
      this.ProjectNameColumn.ReadOnly = true;
      // 
      // ProjectTypeColumn
      // 
      this.ProjectTypeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.ProjectTypeColumn.DataPropertyName = "Type";
      this.ProjectTypeColumn.HeaderText = "Type";
      this.ProjectTypeColumn.Name = "ProjectTypeColumn";
      this.ProjectTypeColumn.ReadOnly = true;
      this.ProjectTypeColumn.Width = 56;
      // 
      // btn_openWebApp
      // 
      this.btn_openWebApp.Enabled = false;
      this.btn_openWebApp.Location = new System.Drawing.Point(187, 274);
      this.btn_openWebApp.Name = "btn_openWebApp";
      this.btn_openWebApp.Size = new System.Drawing.Size(85, 23);
      this.btn_openWebApp.TabIndex = 2;
      this.btn_openWebApp.Text = "Target URL(s)";
      this.btn_openWebApp.UseVisualStyleBackColor = true;
      this.btn_openWebApp.Click += new System.EventHandler(this.btn_openWebApp_Click);
      // 
      // btn_openProjectTargetFolder
      // 
      this.btn_openProjectTargetFolder.Enabled = false;
      this.btn_openProjectTargetFolder.Location = new System.Drawing.Point(78, 274);
      this.btn_openProjectTargetFolder.Name = "btn_openProjectTargetFolder";
      this.btn_openProjectTargetFolder.Size = new System.Drawing.Size(103, 23);
      this.btn_openProjectTargetFolder.TabIndex = 1;
      this.btn_openProjectTargetFolder.Text = "Target folder(s)";
      this.btn_openProjectTargetFolder.UseVisualStyleBackColor = true;
      this.btn_openProjectTargetFolder.Click += new System.EventHandler(this.btn_openProjectTargetFolder_Click);
      // 
      // btn_showProjectInfo
      // 
      this.btn_showProjectInfo.Enabled = false;
      this.btn_showProjectInfo.Location = new System.Drawing.Point(10, 274);
      this.btn_showProjectInfo.Name = "btn_showProjectInfo";
      this.btn_showProjectInfo.Size = new System.Drawing.Size(62, 23);
      this.btn_showProjectInfo.TabIndex = 0;
      this.btn_showProjectInfo.Text = "Info";
      this.btn_showProjectInfo.UseVisualStyleBackColor = true;
      this.btn_showProjectInfo.Click += new System.EventHandler(this.btn_showProjectInfo_Click);
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.btn_showDependencies);
      this.groupBox3.Controls.Add(this.txtFilterConfigs);
      this.groupBox3.Controls.Add(this.dgv_projectConfigurations);
      this.groupBox3.Location = new System.Drawing.Point(299, 27);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Padding = new System.Windows.Forms.Padding(10);
      this.groupBox3.Size = new System.Drawing.Size(195, 305);
      this.groupBox3.TabIndex = 2;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Project configurations";
      // 
      // btn_showDependencies
      // 
      this.btn_showDependencies.Enabled = false;
      this.btn_showDependencies.Location = new System.Drawing.Point(10, 274);
      this.btn_showDependencies.Name = "btn_showDependencies";
      this.btn_showDependencies.Size = new System.Drawing.Size(119, 23);
      this.btn_showDependencies.TabIndex = 0;
      this.btn_showDependencies.Text = "Show dependencies";
      this.btn_showDependencies.UseVisualStyleBackColor = true;
      this.btn_showDependencies.Click += new System.EventHandler(this.btn_showDependencies_Click);
      // 
      // txtFilterConfigs
      // 
      this.txtFilterConfigs.Location = new System.Drawing.Point(10, 23);
      this.txtFilterConfigs.Name = "txtFilterConfigs";
      this.txtFilterConfigs.Size = new System.Drawing.Size(175, 20);
      this.txtFilterConfigs.TabIndex = 0;
      this.txtFilterConfigs.TextChanged += new System.EventHandler(this.txtFilterConfigs_TextChanged);
      // 
      // dgv_projectConfigurations
      // 
      this.dgv_projectConfigurations.AllowUserToAddRows = false;
      this.dgv_projectConfigurations.AllowUserToDeleteRows = false;
      this.dgv_projectConfigurations.AllowUserToResizeRows = false;
      this.dgv_projectConfigurations.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.dgv_projectConfigurations.BackgroundColor = System.Drawing.SystemColors.Window;
      this.dgv_projectConfigurations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgv_projectConfigurations.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ProjectConfigurationNameColumn,
            this.ProjectConfigurationWebLinkColumn});
      this.dgv_projectConfigurations.Location = new System.Drawing.Point(10, 50);
      this.dgv_projectConfigurations.MultiSelect = false;
      this.dgv_projectConfigurations.Name = "dgv_projectConfigurations";
      this.dgv_projectConfigurations.ReadOnly = true;
      this.dgv_projectConfigurations.RowHeadersVisible = false;
      this.dgv_projectConfigurations.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dgv_projectConfigurations.Size = new System.Drawing.Size(175, 218);
      this.dgv_projectConfigurations.TabIndex = 1;
      this.dgv_projectConfigurations.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_projectConfigurations_CellContentClick);
      this.dgv_projectConfigurations.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_projectConfigurations_CellDoubleClick);
      this.dgv_projectConfigurations.SelectionChanged += new System.EventHandler(this.dgv_projectConfigurations_SelectionChanged);
      // 
      // ProjectConfigurationNameColumn
      // 
      this.ProjectConfigurationNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.ProjectConfigurationNameColumn.DataPropertyName = "Name";
      this.ProjectConfigurationNameColumn.HeaderText = "Name";
      this.ProjectConfigurationNameColumn.Name = "ProjectConfigurationNameColumn";
      this.ProjectConfigurationNameColumn.ReadOnly = true;
      // 
      // ProjectConfigurationWebLinkColumn
      // 
      this.ProjectConfigurationWebLinkColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.ProjectConfigurationWebLinkColumn.HeaderText = "Web";
      this.ProjectConfigurationWebLinkColumn.Name = "ProjectConfigurationWebLinkColumn";
      this.ProjectConfigurationWebLinkColumn.ReadOnly = true;
      this.ProjectConfigurationWebLinkColumn.Text = "Link";
      this.ProjectConfigurationWebLinkColumn.UseColumnTextForLinkValue = true;
      this.ProjectConfigurationWebLinkColumn.VisitedLinkColor = System.Drawing.Color.Blue;
      this.ProjectConfigurationWebLinkColumn.Width = 36;
      // 
      // grp_projectConfigurationBuilds
      // 
      this.grp_projectConfigurationBuilds.Controls.Add(this.txtFilterBuilds);
      this.grp_projectConfigurationBuilds.Controls.Add(this.dgv_projectConfigurationBuilds);
      this.grp_projectConfigurationBuilds.Location = new System.Drawing.Point(500, 27);
      this.grp_projectConfigurationBuilds.Name = "grp_projectConfigurationBuilds";
      this.grp_projectConfigurationBuilds.Padding = new System.Windows.Forms.Padding(10);
      this.grp_projectConfigurationBuilds.Size = new System.Drawing.Size(339, 305);
      this.grp_projectConfigurationBuilds.TabIndex = 3;
      this.grp_projectConfigurationBuilds.TabStop = false;
      this.grp_projectConfigurationBuilds.Text = "Project configuration builds";
      // 
      // txtFilterBuilds
      // 
      this.txtFilterBuilds.Location = new System.Drawing.Point(10, 23);
      this.txtFilterBuilds.Name = "txtFilterBuilds";
      this.txtFilterBuilds.Size = new System.Drawing.Size(316, 20);
      this.txtFilterBuilds.TabIndex = 0;
      this.txtFilterBuilds.TextChanged += new System.EventHandler(this.txtFilterBuilds_TextChanged);
      // 
      // dgv_projectConfigurationBuilds
      // 
      this.dgv_projectConfigurationBuilds.AllowUserToAddRows = false;
      this.dgv_projectConfigurationBuilds.AllowUserToDeleteRows = false;
      this.dgv_projectConfigurationBuilds.AllowUserToResizeRows = false;
      this.dgv_projectConfigurationBuilds.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.dgv_projectConfigurationBuilds.BackgroundColor = System.Drawing.SystemColors.Window;
      this.dgv_projectConfigurationBuilds.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgv_projectConfigurationBuilds.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ProjectConfigurationBuildStartDateColumn,
            this.ProjectConfigurationBuildStartTimeColumn,
            this.ProjectConfigurationBuildNumberColumn,
            this.ProjectConfigurationBuildStatusColumn,
            this.ProjectConfigurationBuildWebLinkColumn});
      this.dgv_projectConfigurationBuilds.Location = new System.Drawing.Point(10, 50);
      this.dgv_projectConfigurationBuilds.MultiSelect = false;
      this.dgv_projectConfigurationBuilds.Name = "dgv_projectConfigurationBuilds";
      this.dgv_projectConfigurationBuilds.ReadOnly = true;
      this.dgv_projectConfigurationBuilds.RowHeadersVisible = false;
      this.dgv_projectConfigurationBuilds.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dgv_projectConfigurationBuilds.Size = new System.Drawing.Size(316, 247);
      this.dgv_projectConfigurationBuilds.TabIndex = 1;
      this.dgv_projectConfigurationBuilds.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_projectConfigurationBuilds_CellContentClick);
      this.dgv_projectConfigurationBuilds.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_projectConfigurationBuilds_CellDoubleClick);
      // 
      // ProjectConfigurationBuildStartDateColumn
      // 
      this.ProjectConfigurationBuildStartDateColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.ProjectConfigurationBuildStartDateColumn.DataPropertyName = "StartDate";
      this.ProjectConfigurationBuildStartDateColumn.HeaderText = "Date";
      this.ProjectConfigurationBuildStartDateColumn.Name = "ProjectConfigurationBuildStartDateColumn";
      this.ProjectConfigurationBuildStartDateColumn.ReadOnly = true;
      this.ProjectConfigurationBuildStartDateColumn.Width = 55;
      // 
      // ProjectConfigurationBuildStartTimeColumn
      // 
      this.ProjectConfigurationBuildStartTimeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.ProjectConfigurationBuildStartTimeColumn.DataPropertyName = "StartTime";
      this.ProjectConfigurationBuildStartTimeColumn.HeaderText = "Time";
      this.ProjectConfigurationBuildStartTimeColumn.Name = "ProjectConfigurationBuildStartTimeColumn";
      this.ProjectConfigurationBuildStartTimeColumn.ReadOnly = true;
      this.ProjectConfigurationBuildStartTimeColumn.Width = 55;
      // 
      // ProjectConfigurationBuildNumberColumn
      // 
      this.ProjectConfigurationBuildNumberColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.ProjectConfigurationBuildNumberColumn.DataPropertyName = "Number";
      this.ProjectConfigurationBuildNumberColumn.HeaderText = "Number";
      this.ProjectConfigurationBuildNumberColumn.Name = "ProjectConfigurationBuildNumberColumn";
      this.ProjectConfigurationBuildNumberColumn.ReadOnly = true;
      // 
      // ProjectConfigurationBuildStatusColumn
      // 
      this.ProjectConfigurationBuildStatusColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.ProjectConfigurationBuildStatusColumn.DataPropertyName = "Status";
      this.ProjectConfigurationBuildStatusColumn.HeaderText = "Status";
      this.ProjectConfigurationBuildStatusColumn.Name = "ProjectConfigurationBuildStatusColumn";
      this.ProjectConfigurationBuildStatusColumn.ReadOnly = true;
      this.ProjectConfigurationBuildStatusColumn.Width = 62;
      // 
      // ProjectConfigurationBuildWebLinkColumn
      // 
      this.ProjectConfigurationBuildWebLinkColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.ProjectConfigurationBuildWebLinkColumn.HeaderText = "Web";
      this.ProjectConfigurationBuildWebLinkColumn.Name = "ProjectConfigurationBuildWebLinkColumn";
      this.ProjectConfigurationBuildWebLinkColumn.ReadOnly = true;
      this.ProjectConfigurationBuildWebLinkColumn.Text = "Link";
      this.ProjectConfigurationBuildWebLinkColumn.UseColumnTextForLinkValue = true;
      this.ProjectConfigurationBuildWebLinkColumn.VisitedLinkColor = System.Drawing.Color.Blue;
      this.ProjectConfigurationBuildWebLinkColumn.Width = 36;
      // 
      // txt_log
      // 
      this.txt_log.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.txt_log.DetectUrls = false;
      this.txt_log.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txt_log.Location = new System.Drawing.Point(0, 0);
      this.txt_log.Name = "txt_log";
      this.txt_log.ReadOnly = true;
      this.txt_log.Size = new System.Drawing.Size(813, 231);
      this.txt_log.TabIndex = 0;
      this.txt_log.Text = "";
      this.txt_log.WordWrap = false;
      // 
      // grpLog
      // 
      this.grpLog.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.grpLog.Controls.Add(this.label1);
      this.grpLog.Controls.Add(this.cb_messageTypeThreshold);
      this.grpLog.Controls.Add(this.btn_clearLog);
      this.grpLog.Controls.Add(this.panel2);
      this.grpLog.Location = new System.Drawing.Point(12, 378);
      this.grpLog.Name = "grpLog";
      this.grpLog.Size = new System.Drawing.Size(827, 287);
      this.grpLog.TabIndex = 8;
      this.grpLog.TabStop = false;
      this.grpLog.Text = "Log";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(90, 24);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(57, 13);
      this.label1.TabIndex = 10;
      this.label1.Text = "Threshold:";
      // 
      // cb_messageTypeThreshold
      // 
      this.cb_messageTypeThreshold.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cb_messageTypeThreshold.FormattingEnabled = true;
      this.cb_messageTypeThreshold.Location = new System.Drawing.Point(153, 20);
      this.cb_messageTypeThreshold.Name = "cb_messageTypeThreshold";
      this.cb_messageTypeThreshold.Size = new System.Drawing.Size(87, 21);
      this.cb_messageTypeThreshold.TabIndex = 9;
      // 
      // btn_clearLog
      // 
      this.btn_clearLog.Location = new System.Drawing.Point(6, 19);
      this.btn_clearLog.Name = "btn_clearLog";
      this.btn_clearLog.Size = new System.Drawing.Size(65, 23);
      this.btn_clearLog.TabIndex = 0;
      this.btn_clearLog.Text = "Clear lo&g";
      this.btn_clearLog.UseVisualStyleBackColor = true;
      this.btn_clearLog.Click += new System.EventHandler(this.btn_clearLog_Click);
      // 
      // panel2
      // 
      this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.panel2.Controls.Add(this.txt_log);
      this.panel2.Location = new System.Drawing.Point(6, 48);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(815, 233);
      this.panel2.TabIndex = 8;
      // 
      // btn_deploy
      // 
      this.btn_deploy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.btn_deploy.Location = new System.Drawing.Point(725, 338);
      this.btn_deploy.Name = "btn_deploy";
      this.btn_deploy.Size = new System.Drawing.Size(114, 34);
      this.btn_deploy.TabIndex = 4;
      this.btn_deploy.Text = "Deploy";
      this.btn_deploy.UseVisualStyleBackColor = true;
      this.btn_deploy.Click += new System.EventHandler(this.btn_deploy_Click);
      // 
      // cbx_targetEnvironment
      // 
      this.cbx_targetEnvironment.DisplayMember = "DisplayText";
      this.cbx_targetEnvironment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbx_targetEnvironment.FormattingEnabled = true;
      this.cbx_targetEnvironment.Location = new System.Drawing.Point(568, 346);
      this.cbx_targetEnvironment.Name = "cbx_targetEnvironment";
      this.cbx_targetEnvironment.Size = new System.Drawing.Size(121, 21);
      this.cbx_targetEnvironment.TabIndex = 6;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(493, 349);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(69, 13);
      this.label5.TabIndex = 5;
      this.label5.Text = "Environment:";
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolsToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(851, 24);
      this.menuStrip1.TabIndex = 0;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configurationToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "&File";
      // 
      // configurationToolStripMenuItem
      // 
      this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
      this.configurationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.C)));
      this.configurationToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
      this.configurationToolStripMenuItem.Text = "&Configuration";
      this.configurationToolStripMenuItem.Click += new System.EventHandler(this.configurationToolStripMenuItem_Click);
      // 
      // toolStripMenuItem1
      // 
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(210, 6);
      // 
      // exitToolStripMenuItem
      // 
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
      this.exitToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
      this.exitToolStripMenuItem.Text = "E&xit";
      this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
      // 
      // viewToolStripMenuItem
      // 
      this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_reloadProjects});
      this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
      this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
      this.viewToolStripMenuItem.Text = "&View";
      // 
      // tsmi_reloadProjects
      // 
      this.tsmi_reloadProjects.Name = "tsmi_reloadProjects";
      this.tsmi_reloadProjects.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
      this.tsmi_reloadProjects.Size = new System.Drawing.Size(196, 22);
      this.tsmi_reloadProjects.Text = "Reload &projects";
      this.tsmi_reloadProjects.Click += new System.EventHandler(this.reloadProjectsToolStripMenuItem_Click);
      // 
      // toolsToolStripMenuItem
      // 
      this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deploymentAuditToolStripMenuItem,
            this.dependenciesVisualizerToolStripMenuItem,
            this.dbVersionsDiffToolStripMenuItem});
      this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
      this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
      this.toolsToolStripMenuItem.Text = "&Tools";
      // 
      // deploymentAuditToolStripMenuItem
      // 
      this.deploymentAuditToolStripMenuItem.Name = "deploymentAuditToolStripMenuItem";
      this.deploymentAuditToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
      this.deploymentAuditToolStripMenuItem.Text = "Deployment &audit";
      this.deploymentAuditToolStripMenuItem.Click += new System.EventHandler(this.deploymentAuditToolStripMenuItem_Click);
      // 
      // dependenciesVisualizerToolStripMenuItem
      // 
      this.dependenciesVisualizerToolStripMenuItem.Enabled = false;
      this.dependenciesVisualizerToolStripMenuItem.Name = "dependenciesVisualizerToolStripMenuItem";
      this.dependenciesVisualizerToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
      this.dependenciesVisualizerToolStripMenuItem.Text = "Dependencies &visualizer";
      this.dependenciesVisualizerToolStripMenuItem.Click += new System.EventHandler(this.dependenciesVisualizerToolStripMenuItem_Click);
      // 
      // dbVersionsDiffToolStripMenuItem
      // 
      this.dbVersionsDiffToolStripMenuItem.Enabled = false;
      this.dbVersionsDiffToolStripMenuItem.Name = "dbVersionsDiffToolStripMenuItem";
      this.dbVersionsDiffToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
      this.dbVersionsDiffToolStripMenuItem.Text = "Database versions diff";
      this.dbVersionsDiffToolStripMenuItem.Click += new System.EventHandler(this.dbVersionsDiffToolStripMenuItem_Click);
      // 
      // btn_showEnvironmentInfo
      // 
      this.btn_showEnvironmentInfo.Location = new System.Drawing.Point(695, 344);
      this.btn_showEnvironmentInfo.Name = "btn_showEnvironmentInfo";
      this.btn_showEnvironmentInfo.Size = new System.Drawing.Size(24, 23);
      this.btn_showEnvironmentInfo.TabIndex = 7;
      this.btn_showEnvironmentInfo.Text = "I";
      this.btn_showEnvironmentInfo.UseVisualStyleBackColor = true;
      this.btn_showEnvironmentInfo.Click += new System.EventHandler(this.btn_showEnvironmentInfo_Click);
      // 
      // pic_indeterminateProgress
      // 
      this.pic_indeterminateProgress.Image = global::UberDeployer.WinApp.Properties.Resources.indeterminate_progress;
      this.pic_indeterminateProgress.Location = new System.Drawing.Point(823, 5);
      this.pic_indeterminateProgress.Name = "pic_indeterminateProgress";
      this.pic_indeterminateProgress.Size = new System.Drawing.Size(16, 16);
      this.pic_indeterminateProgress.TabIndex = 9;
      this.pic_indeterminateProgress.TabStop = false;
      this.pic_indeterminateProgress.Visible = false;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(851, 677);
      this.Controls.Add(this.pic_indeterminateProgress);
      this.Controls.Add(this.btn_showEnvironmentInfo);
      this.Controls.Add(this.menuStrip1);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.cbx_targetEnvironment);
      this.Controls.Add(this.btn_deploy);
      this.Controls.Add(this.grpLog);
      this.Controls.Add(this.grp_projectConfigurationBuilds);
      this.Controls.Add(this.groupBox3);
      this.Controls.Add(this.groupBox2);
      this.DoubleBuffered = true;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.KeyPreview = true;
      this.MainMenuStrip = this.menuStrip1;
      this.MaximizeBox = false;
      this.Name = "MainForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "ÜberDeployer";
      this.Load += new System.EventHandler(this.MainForm_Load);
      this.Shown += new System.EventHandler(this.MainForm_Shown);
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgv_projectInfos)).EndInit();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgv_projectConfigurations)).EndInit();
      this.grp_projectConfigurationBuilds.ResumeLayout(false);
      this.grp_projectConfigurationBuilds.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgv_projectConfigurationBuilds)).EndInit();
      this.grpLog.ResumeLayout(false);
      this.grpLog.PerformLayout();
      this.panel2.ResumeLayout(false);
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pic_indeterminateProgress)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.GroupBox grp_projectConfigurationBuilds;
    private System.Windows.Forms.RichTextBox txt_log;
    private System.Windows.Forms.GroupBox grpLog;
    private System.Windows.Forms.Button btn_deploy;
    private System.Windows.Forms.PictureBox pic_indeterminateProgress;
    private System.Windows.Forms.ComboBox cbx_targetEnvironment;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Button btn_clearLog;
    private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem tsmi_reloadProjects;
    private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    private CustomControls.MyDataGridView dgv_projectConfigurations;
    private CustomControls.MyDataGridView dgv_projectConfigurationBuilds;
    private System.Windows.Forms.DataGridViewTextBoxColumn ProjectConfigurationNameColumn;
    private System.Windows.Forms.DataGridViewLinkColumn ProjectConfigurationWebLinkColumn;
    private System.Windows.Forms.Button btn_openWebApp;
    private System.Windows.Forms.Button btn_openProjectTargetFolder;
    private System.Windows.Forms.Button btn_showProjectInfo;
    private System.Windows.Forms.Button btn_showEnvironmentInfo;
    private System.Windows.Forms.TextBox txtFilterConfigs;
    private System.Windows.Forms.TextBox txtFilterBuilds;
    private System.Windows.Forms.Button btn_showDependencies;
    private System.Windows.Forms.TextBox txtFilter;
    private CustomControls.MyDataGridView dgv_projectInfos;
    private System.Windows.Forms.DataGridViewTextBoxColumn ProjectNameColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn ProjectTypeColumn;
    private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem deploymentAuditToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem dependenciesVisualizerToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem dbVersionsDiffToolStripMenuItem;
    private System.Windows.Forms.DataGridViewTextBoxColumn ProjectConfigurationBuildStartDateColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn ProjectConfigurationBuildStartTimeColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn ProjectConfigurationBuildNumberColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn ProjectConfigurationBuildStatusColumn;
    private System.Windows.Forms.DataGridViewLinkColumn ProjectConfigurationBuildWebLinkColumn;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox cb_messageTypeThreshold;
  }
}

