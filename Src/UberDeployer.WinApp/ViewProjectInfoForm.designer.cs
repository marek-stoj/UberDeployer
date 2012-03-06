namespace UberDeployer.WinApp
{
  partial class ViewProjectInfoForm
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
      this.projectPropertiesPropertyGrid = new System.Windows.Forms.PropertyGrid();
      this.panel1 = new System.Windows.Forms.Panel();
      this.lbl_projectType = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
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
      // projectPropertiesPropertyGrid
      // 
      this.projectPropertiesPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.projectPropertiesPropertyGrid.HelpVisible = false;
      this.projectPropertiesPropertyGrid.Location = new System.Drawing.Point(5, 18);
      this.projectPropertiesPropertyGrid.Name = "projectPropertiesPropertyGrid";
      this.projectPropertiesPropertyGrid.Size = new System.Drawing.Size(650, 352);
      this.projectPropertiesPropertyGrid.TabIndex = 2;
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.lbl_projectType);
      this.panel1.Controls.Add(this.label1);
      this.panel1.Controls.Add(this.btn_close);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(10, 385);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(660, 29);
      this.panel1.TabIndex = 4;
      // 
      // lbl_projectType
      // 
      this.lbl_projectType.AutoSize = true;
      this.lbl_projectType.Location = new System.Drawing.Point(88, 8);
      this.lbl_projectType.Name = "lbl_projectType";
      this.lbl_projectType.Size = new System.Drawing.Size(13, 13);
      this.lbl_projectType.TabIndex = 3;
      this.lbl_projectType.Text = "?";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.label1.Location = new System.Drawing.Point(3, 8);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(79, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Project type:";
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.projectPropertiesPropertyGrid);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox2.Location = new System.Drawing.Point(10, 10);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Padding = new System.Windows.Forms.Padding(5);
      this.groupBox2.Size = new System.Drawing.Size(660, 375);
      this.groupBox2.TabIndex = 5;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Properties";
      // 
      // ViewProjectInfoForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(680, 424);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.panel1);
      this.KeyPreview = true;
      this.Name = "ViewProjectInfoForm";
      this.Padding = new System.Windows.Forms.Padding(10);
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Project: ";
      this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ViewProjectInfoForm_KeyUp);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btn_close;
    private System.Windows.Forms.PropertyGrid projectPropertiesPropertyGrid;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label lbl_projectType;
  }
}