namespace ProjectDepsVisualizer.UI
{
  partial class ProjectDependenciesModelForm
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
      this.wpfElementHost_dependenciesGraph = new System.Windows.Forms.Integration.ElementHost();
      this.panel1 = new System.Windows.Forms.Panel();
      this.btn_export = new System.Windows.Forms.Button();
      this.cb_exportFormat = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.btn_close = new System.Windows.Forms.Button();
      this.panel2 = new System.Windows.Forms.Panel();
      this.saveFileDialog_export = new System.Windows.Forms.SaveFileDialog();
      this.btn_verifyVersionsIntegrity = new System.Windows.Forms.Button();
      this.panel1.SuspendLayout();
      this.panel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // wpfElementHost_dependenciesGraph
      // 
      this.wpfElementHost_dependenciesGraph.Dock = System.Windows.Forms.DockStyle.Fill;
      this.wpfElementHost_dependenciesGraph.Location = new System.Drawing.Point(0, 0);
      this.wpfElementHost_dependenciesGraph.Name = "wpfElementHost_dependenciesGraph";
      this.wpfElementHost_dependenciesGraph.Size = new System.Drawing.Size(1008, 692);
      this.wpfElementHost_dependenciesGraph.TabIndex = 0;
      this.wpfElementHost_dependenciesGraph.Text = "elementHost1";
      this.wpfElementHost_dependenciesGraph.Child = null;
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btn_verifyVersionsIntegrity);
      this.panel1.Controls.Add(this.btn_export);
      this.panel1.Controls.Add(this.cb_exportFormat);
      this.panel1.Controls.Add(this.label1);
      this.panel1.Controls.Add(this.btn_close);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 692);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(1008, 38);
      this.panel1.TabIndex = 1;
      // 
      // btn_export
      // 
      this.btn_export.Location = new System.Drawing.Point(217, 7);
      this.btn_export.Name = "btn_export";
      this.btn_export.Size = new System.Drawing.Size(75, 23);
      this.btn_export.TabIndex = 3;
      this.btn_export.Text = "Export...";
      this.btn_export.UseVisualStyleBackColor = true;
      this.btn_export.Click += new System.EventHandler(this.btn_export_Click);
      // 
      // cb_exportFormat
      // 
      this.cb_exportFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cb_exportFormat.FormattingEnabled = true;
      this.cb_exportFormat.Location = new System.Drawing.Point(90, 9);
      this.cb_exportFormat.Name = "cb_exportFormat";
      this.cb_exportFormat.Size = new System.Drawing.Size(121, 21);
      this.cb_exportFormat.TabIndex = 2;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 12);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(72, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Export format:";
      // 
      // btn_close
      // 
      this.btn_close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btn_close.Location = new System.Drawing.Point(921, 6);
      this.btn_close.Name = "btn_close";
      this.btn_close.Size = new System.Drawing.Size(75, 23);
      this.btn_close.TabIndex = 0;
      this.btn_close.Text = "Close";
      this.btn_close.UseVisualStyleBackColor = true;
      this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.wpfElementHost_dependenciesGraph);
      this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel2.Location = new System.Drawing.Point(0, 0);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(1008, 692);
      this.panel2.TabIndex = 2;
      // 
      // btn_verifyVersions
      // 
      this.btn_verifyVersionsIntegrity.Location = new System.Drawing.Point(338, 7);
      this.btn_verifyVersionsIntegrity.Name = "btn_verifyVersionsIntegrity";
      this.btn_verifyVersionsIntegrity.Size = new System.Drawing.Size(115, 23);
      this.btn_verifyVersionsIntegrity.TabIndex = 4;
      this.btn_verifyVersionsIntegrity.Text = "Verify versions";
      this.btn_verifyVersionsIntegrity.UseVisualStyleBackColor = true;
      this.btn_verifyVersionsIntegrity.Click += new System.EventHandler(this.btn_verifyVersionsIntegrity_Click);
      // 
      // ProjectDependenciesModelForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1008, 730);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.panel1);
      this.Name = "ProjectDependenciesModelForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "ProjectDependenciesGraphForm";
      this.Load += new System.EventHandler(this.ProjectDependenciesGraphForm_Load);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.panel2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Integration.ElementHost wpfElementHost_dependenciesGraph;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btn_close;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Button btn_export;
    private System.Windows.Forms.ComboBox cb_exportFormat;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.SaveFileDialog saveFileDialog_export;
    private System.Windows.Forms.Button btn_verifyVersionsIntegrity;
  }
}