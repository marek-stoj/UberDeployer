namespace UberDeployer.Core.Deployment
{
  partial class PasswordPromptForm
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
      this.txt_password = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.txt_userName = new System.Windows.Forms.TextBox();
      this.txt_environmentName = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.btn_ok = new System.Windows.Forms.Button();
      this.btn_cancel = new System.Windows.Forms.Button();
      this.txt_machineName = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.txt_machineName);
      this.groupBox1.Controls.Add(this.label4);
      this.groupBox1.Controls.Add(this.txt_password);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.txt_userName);
      this.groupBox1.Controls.Add(this.txt_environmentName);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Location = new System.Drawing.Point(12, 12);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(336, 128);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Enter password";
      // 
      // txt_password
      // 
      this.txt_password.Location = new System.Drawing.Point(96, 97);
      this.txt_password.Name = "txt_password";
      this.txt_password.PasswordChar = '*';
      this.txt_password.Size = new System.Drawing.Size(225, 20);
      this.txt_password.TabIndex = 5;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(13, 100);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(56, 13);
      this.label3.TabIndex = 4;
      this.label3.Text = "Password:";
      // 
      // txt_userName
      // 
      this.txt_userName.Location = new System.Drawing.Point(96, 71);
      this.txt_userName.Name = "txt_userName";
      this.txt_userName.ReadOnly = true;
      this.txt_userName.Size = new System.Drawing.Size(225, 20);
      this.txt_userName.TabIndex = 3;
      // 
      // txt_environmentName
      // 
      this.txt_environmentName.Location = new System.Drawing.Point(96, 19);
      this.txt_environmentName.Name = "txt_environmentName";
      this.txt_environmentName.ReadOnly = true;
      this.txt_environmentName.Size = new System.Drawing.Size(225, 20);
      this.txt_environmentName.TabIndex = 2;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(13, 74);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(61, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "User name:";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(13, 22);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(69, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Environment:";
      // 
      // btn_ok
      // 
      this.btn_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btn_ok.Location = new System.Drawing.Point(192, 150);
      this.btn_ok.Name = "btn_ok";
      this.btn_ok.Size = new System.Drawing.Size(75, 23);
      this.btn_ok.TabIndex = 1;
      this.btn_ok.Text = "OK";
      this.btn_ok.UseVisualStyleBackColor = true;
      this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
      // 
      // btn_cancel
      // 
      this.btn_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btn_cancel.Location = new System.Drawing.Point(273, 150);
      this.btn_cancel.Name = "btn_cancel";
      this.btn_cancel.Size = new System.Drawing.Size(75, 23);
      this.btn_cancel.TabIndex = 2;
      this.btn_cancel.Text = "Cancel";
      this.btn_cancel.UseVisualStyleBackColor = true;
      this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
      // 
      // txt_machineName
      // 
      this.txt_machineName.Location = new System.Drawing.Point(96, 45);
      this.txt_machineName.Name = "txt_machineName";
      this.txt_machineName.ReadOnly = true;
      this.txt_machineName.Size = new System.Drawing.Size(225, 20);
      this.txt_machineName.TabIndex = 7;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(13, 48);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(51, 13);
      this.label4.TabIndex = 6;
      this.label4.Text = "Machine:";
      // 
      // PasswordPromptForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(360, 180);
      this.Controls.Add(this.btn_cancel);
      this.Controls.Add(this.btn_ok);
      this.Controls.Add(this.groupBox1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.KeyPreview = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "PasswordPromptForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Password prompt";
      this.Shown += new System.EventHandler(this.PasswordPromptForm_Shown);
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PasswordPromptForm_KeyDown);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Button btn_ok;
    private System.Windows.Forms.Button btn_cancel;
    private System.Windows.Forms.TextBox txt_password;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox txt_userName;
    private System.Windows.Forms.TextBox txt_environmentName;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txt_machineName;
    private System.Windows.Forms.Label label4;
  }
}