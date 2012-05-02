namespace UberDeployer.WinApp.Forms
{
  partial class ConfigurationForm
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
      this.btn_save = new System.Windows.Forms.Button();
      this.btn_cancel = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.label4 = new System.Windows.Forms.Label();
      this.txt_teamCityPassword = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.txt_teamCityUserName = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.txt_teamCityPort = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.txt_teamCityHostName = new System.Windows.Forms.TextBox();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // btn_save
      // 
      this.btn_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btn_save.Location = new System.Drawing.Point(125, 155);
      this.btn_save.Name = "btn_save";
      this.btn_save.Size = new System.Drawing.Size(75, 23);
      this.btn_save.TabIndex = 0;
      this.btn_save.Text = "Save";
      this.btn_save.UseVisualStyleBackColor = true;
      this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
      // 
      // btn_cancel
      // 
      this.btn_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btn_cancel.Location = new System.Drawing.Point(206, 155);
      this.btn_cancel.Name = "btn_cancel";
      this.btn_cancel.Size = new System.Drawing.Size(75, 23);
      this.btn_cancel.TabIndex = 1;
      this.btn_cancel.Text = "Cancel";
      this.btn_cancel.UseVisualStyleBackColor = true;
      this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.label4);
      this.groupBox1.Controls.Add(this.txt_teamCityPassword);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.txt_teamCityUserName);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.txt_teamCityPort);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.txt_teamCityHostName);
      this.groupBox1.Location = new System.Drawing.Point(12, 12);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(267, 131);
      this.groupBox1.TabIndex = 2;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "TeamCity";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(14, 100);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(56, 13);
      this.label4.TabIndex = 7;
      this.label4.Text = "Password:";
      // 
      // txt_teamCityPassword
      // 
      this.txt_teamCityPassword.Location = new System.Drawing.Point(81, 97);
      this.txt_teamCityPassword.Name = "txt_teamCityPassword";
      this.txt_teamCityPassword.PasswordChar = '*';
      this.txt_teamCityPassword.Size = new System.Drawing.Size(166, 20);
      this.txt_teamCityPassword.TabIndex = 6;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(14, 74);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(61, 13);
      this.label3.TabIndex = 5;
      this.label3.Text = "User name:";
      // 
      // txt_teamCityUserName
      // 
      this.txt_teamCityUserName.Location = new System.Drawing.Point(81, 71);
      this.txt_teamCityUserName.Name = "txt_teamCityUserName";
      this.txt_teamCityUserName.Size = new System.Drawing.Size(166, 20);
      this.txt_teamCityUserName.TabIndex = 4;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(14, 48);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(29, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "Port:";
      // 
      // txt_teamCityPort
      // 
      this.txt_teamCityPort.Location = new System.Drawing.Point(81, 45);
      this.txt_teamCityPort.Name = "txt_teamCityPort";
      this.txt_teamCityPort.Size = new System.Drawing.Size(166, 20);
      this.txt_teamCityPort.TabIndex = 2;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(14, 22);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(61, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Host name:";
      // 
      // txt_teamCityHostName
      // 
      this.txt_teamCityHostName.Location = new System.Drawing.Point(81, 19);
      this.txt_teamCityHostName.Name = "txt_teamCityHostName";
      this.txt_teamCityHostName.Size = new System.Drawing.Size(166, 20);
      this.txt_teamCityHostName.TabIndex = 0;
      // 
      // ConfigurationForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(293, 190);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.btn_cancel);
      this.Controls.Add(this.btn_save);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.KeyPreview = true;
      this.MaximizeBox = false;
      this.Name = "ConfigurationForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Configuration";
      this.Load += new System.EventHandler(this.ConfigurationForm_Load);
      this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ConfigurationForm_KeyUp);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btn_save;
    private System.Windows.Forms.Button btn_cancel;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox txt_teamCityPassword;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox txt_teamCityUserName;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txt_teamCityPort;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txt_teamCityHostName;
  }
}