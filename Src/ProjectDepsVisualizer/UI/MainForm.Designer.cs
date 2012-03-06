namespace ProjectDepsVisualizer.UI
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
      this.txt_svnRepositoryBaseUrl = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.txt_svnExeFilePath = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.txt_svnUserName = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.txt_svnPassword = new System.Windows.Forms.TextBox();
      this.gb_svn = new System.Windows.Forms.GroupBox();
      this.btn_buildDependencyGraph = new System.Windows.Forms.Button();
      this.label5 = new System.Windows.Forms.Label();
      this.txt_projectName = new System.Windows.Forms.TextBox();
      this.txt_projectConfiguration = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      this.gb_project = new System.Windows.Forms.GroupBox();
      this.btn_close = new System.Windows.Forms.Button();
      this.gb_log = new System.Windows.Forms.GroupBox();
      this.txt_log = new System.Windows.Forms.TextBox();
      this.gb_svn.SuspendLayout();
      this.gb_project.SuspendLayout();
      this.gb_log.SuspendLayout();
      this.SuspendLayout();
      // 
      // txt_svnRepositoryBaseUrl
      // 
      this.txt_svnRepositoryBaseUrl.Location = new System.Drawing.Point(167, 45);
      this.txt_svnRepositoryBaseUrl.Name = "txt_svnRepositoryBaseUrl";
      this.txt_svnRepositoryBaseUrl.Size = new System.Drawing.Size(229, 20);
      this.txt_svnRepositoryBaseUrl.TabIndex = 1;
      this.txt_svnRepositoryBaseUrl.Text = "http://svn/svn/";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 48);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(111, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Repository base URL:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(6, 22);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(71, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "EXE file path:";
      // 
      // txt_svnExeFilePath
      // 
      this.txt_svnExeFilePath.Location = new System.Drawing.Point(167, 19);
      this.txt_svnExeFilePath.Name = "txt_svnExeFilePath";
      this.txt_svnExeFilePath.Size = new System.Drawing.Size(229, 20);
      this.txt_svnExeFilePath.TabIndex = 0;
      this.txt_svnExeFilePath.Text = "D:\\Programs\\SVN\\svn.exe";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(6, 74);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(61, 13);
      this.label3.TabIndex = 5;
      this.label3.Text = "User name:";
      // 
      // txt_svnUserName
      // 
      this.txt_svnUserName.Location = new System.Drawing.Point(167, 71);
      this.txt_svnUserName.Name = "txt_svnUserName";
      this.txt_svnUserName.Size = new System.Drawing.Size(229, 20);
      this.txt_svnUserName.TabIndex = 2;
      this.txt_svnUserName.Text = "nant";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(6, 100);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(56, 13);
      this.label4.TabIndex = 7;
      this.label4.Text = "Password:";
      // 
      // txt_svnPassword
      // 
      this.txt_svnPassword.Location = new System.Drawing.Point(167, 97);
      this.txt_svnPassword.Name = "txt_svnPassword";
      this.txt_svnPassword.Size = new System.Drawing.Size(229, 20);
      this.txt_svnPassword.TabIndex = 3;
      this.txt_svnPassword.Text = "builder";
      // 
      // gb_svn
      // 
      this.gb_svn.Controls.Add(this.txt_svnExeFilePath);
      this.gb_svn.Controls.Add(this.label4);
      this.gb_svn.Controls.Add(this.txt_svnRepositoryBaseUrl);
      this.gb_svn.Controls.Add(this.txt_svnPassword);
      this.gb_svn.Controls.Add(this.label1);
      this.gb_svn.Controls.Add(this.label3);
      this.gb_svn.Controls.Add(this.label2);
      this.gb_svn.Controls.Add(this.txt_svnUserName);
      this.gb_svn.Location = new System.Drawing.Point(12, 12);
      this.gb_svn.Name = "gb_svn";
      this.gb_svn.Size = new System.Drawing.Size(408, 128);
      this.gb_svn.TabIndex = 0;
      this.gb_svn.TabStop = false;
      this.gb_svn.Text = "SVN";
      // 
      // btn_buildDependencyGraph
      // 
      this.btn_buildDependencyGraph.Location = new System.Drawing.Point(12, 226);
      this.btn_buildDependencyGraph.Name = "btn_buildDependencyGraph";
      this.btn_buildDependencyGraph.Size = new System.Drawing.Size(161, 23);
      this.btn_buildDependencyGraph.TabIndex = 2;
      this.btn_buildDependencyGraph.Text = "Dependecies of";
      this.btn_buildDependencyGraph.UseVisualStyleBackColor = true;
      this.btn_buildDependencyGraph.Click += new System.EventHandler(this.btn_buildDependencyGraph_Click);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(6, 20);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(38, 13);
      this.label5.TabIndex = 10;
      this.label5.Text = "Name:";
      // 
      // txt_projectName
      // 
      this.txt_projectName.Location = new System.Drawing.Point(167, 17);
      this.txt_projectName.Name = "txt_projectName";
      this.txt_projectName.Size = new System.Drawing.Size(229, 20);
      this.txt_projectName.TabIndex = 0;
      this.txt_projectName.Text = "Nabbi";
      // 
      // txt_projectConfiguration
      // 
      this.txt_projectConfiguration.Location = new System.Drawing.Point(167, 43);
      this.txt_projectConfiguration.Name = "txt_projectConfiguration";
      this.txt_projectConfiguration.Size = new System.Drawing.Size(229, 20);
      this.txt_projectConfiguration.TabIndex = 1;
      this.txt_projectConfiguration.Text = "Trunk";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(6, 46);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(72, 13);
      this.label6.TabIndex = 12;
      this.label6.Text = "Configuration:";
      // 
      // gb_project
      // 
      this.gb_project.Controls.Add(this.label5);
      this.gb_project.Controls.Add(this.txt_projectConfiguration);
      this.gb_project.Controls.Add(this.txt_projectName);
      this.gb_project.Controls.Add(this.label6);
      this.gb_project.Location = new System.Drawing.Point(12, 146);
      this.gb_project.Name = "gb_project";
      this.gb_project.Size = new System.Drawing.Size(408, 74);
      this.gb_project.TabIndex = 1;
      this.gb_project.TabStop = false;
      this.gb_project.Text = "Project";
      // 
      // btn_close
      // 
      this.btn_close.Location = new System.Drawing.Point(346, 226);
      this.btn_close.Name = "btn_close";
      this.btn_close.Size = new System.Drawing.Size(74, 23);
      this.btn_close.TabIndex = 3;
      this.btn_close.Text = "Close";
      this.btn_close.UseVisualStyleBackColor = true;
      this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
      // 
      // gb_log
      // 
      this.gb_log.Controls.Add(this.txt_log);
      this.gb_log.Location = new System.Drawing.Point(426, 12);
      this.gb_log.Name = "gb_log";
      this.gb_log.Padding = new System.Windows.Forms.Padding(10, 5, 10, 10);
      this.gb_log.Size = new System.Drawing.Size(432, 237);
      this.gb_log.TabIndex = 4;
      this.gb_log.TabStop = false;
      this.gb_log.Text = "Log";
      // 
      // txt_log
      // 
      this.txt_log.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txt_log.Location = new System.Drawing.Point(10, 18);
      this.txt_log.Multiline = true;
      this.txt_log.Name = "txt_log";
      this.txt_log.ReadOnly = true;
      this.txt_log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.txt_log.Size = new System.Drawing.Size(412, 209);
      this.txt_log.TabIndex = 0;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(870, 258);
      this.Controls.Add(this.gb_log);
      this.Controls.Add(this.btn_close);
      this.Controls.Add(this.gb_project);
      this.Controls.Add(this.btn_buildDependencyGraph);
      this.Controls.Add(this.gb_svn);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.Name = "MainForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Project Dependencies Visualizer";
      this.Load += new System.EventHandler(this.MainForm_Load);
      this.Shown += new System.EventHandler(this.MainForm_Shown);
      this.gb_svn.ResumeLayout(false);
      this.gb_svn.PerformLayout();
      this.gb_project.ResumeLayout(false);
      this.gb_project.PerformLayout();
      this.gb_log.ResumeLayout(false);
      this.gb_log.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TextBox txt_svnRepositoryBaseUrl;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txt_svnExeFilePath;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox txt_svnUserName;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox txt_svnPassword;
    private System.Windows.Forms.GroupBox gb_svn;
    private System.Windows.Forms.Button btn_buildDependencyGraph;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox txt_projectName;
    private System.Windows.Forms.TextBox txt_projectConfiguration;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.GroupBox gb_project;
    private System.Windows.Forms.Button btn_close;
    private System.Windows.Forms.GroupBox gb_log;
    private System.Windows.Forms.TextBox txt_log;
  }
}

