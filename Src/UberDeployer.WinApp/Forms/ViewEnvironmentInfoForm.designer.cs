namespace UberDeployer.WinApp.Forms
{
  partial class ViewEnvironmentInfoForm
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
      this.environmentPropertiesPropertyGrid = new System.Windows.Forms.PropertyGrid();
      this.panel1 = new System.Windows.Forms.Panel();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.panel1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      // 
      // btn_close
      // 
      this.btn_close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btn_close.Location = new System.Drawing.Point(582, 3);
      this.btn_close.Name = "btn_close";
      this.btn_close.Size = new System.Drawing.Size(75, 23);
      this.btn_close.TabIndex = 1;
      this.btn_close.Text = "Close";
      this.btn_close.UseVisualStyleBackColor = true;
      this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
      // 
      // environmentPropertiesPropertyGrid
      // 
      this.environmentPropertiesPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.environmentPropertiesPropertyGrid.HelpVisible = false;
      this.environmentPropertiesPropertyGrid.Location = new System.Drawing.Point(5, 18);
      this.environmentPropertiesPropertyGrid.Name = "environmentPropertiesPropertyGrid";
      this.environmentPropertiesPropertyGrid.Size = new System.Drawing.Size(650, 352);
      this.environmentPropertiesPropertyGrid.TabIndex = 2;
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btn_close);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(10, 385);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(660, 29);
      this.panel1.TabIndex = 4;
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.environmentPropertiesPropertyGrid);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox2.Location = new System.Drawing.Point(10, 10);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Padding = new System.Windows.Forms.Padding(5);
      this.groupBox2.Size = new System.Drawing.Size(660, 375);
      this.groupBox2.TabIndex = 5;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Properties";
      // 
      // ViewEnvironmentInfoForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(680, 424);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.panel1);
      this.KeyPreview = true;
      this.Name = "ViewEnvironmentInfoForm";
      this.Padding = new System.Windows.Forms.Padding(10);
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Environment: ";
      this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ViewProjectInfoForm_KeyUp);
      this.panel1.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btn_close;
    private System.Windows.Forms.PropertyGrid environmentPropertiesPropertyGrid;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.GroupBox groupBox2;
  }
}