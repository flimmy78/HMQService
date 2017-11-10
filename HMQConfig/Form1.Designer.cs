using System.Windows.Forms;

namespace HMQConfig
{
    partial class Form1
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
            this.comboDBInstance = new System.Windows.Forms.ComboBox();
            this.textDBPassword = new System.Windows.Forms.TextBox();
            this.textDBUsername = new System.Windows.Forms.TextBox();
            this.textDBIP = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDBLogin = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_SelectFile = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboDBInstance);
            this.groupBox1.Controls.Add(this.textDBPassword);
            this.groupBox1.Controls.Add(this.textDBUsername);
            this.groupBox1.Controls.Add(this.textDBIP);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnDBLogin);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(388, 193);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "数据库配置";
            // 
            // comboDBInstance
            // 
            this.comboDBInstance.FormattingEnabled = true;
            this.comboDBInstance.Location = new System.Drawing.Point(97, 156);
            this.comboDBInstance.Name = "comboDBInstance";
            this.comboDBInstance.Size = new System.Drawing.Size(285, 20);
            this.comboDBInstance.TabIndex = 4;
            this.comboDBInstance.SelectedIndexChanged += new System.EventHandler(this.comboDBInstance_SelectedIndexChanged);
            // 
            // textDBPassword
            // 
            this.textDBPassword.Location = new System.Drawing.Point(97, 80);
            this.textDBPassword.Name = "textDBPassword";
            this.textDBPassword.PasswordChar = '*';
            this.textDBPassword.Size = new System.Drawing.Size(285, 21);
            this.textDBPassword.TabIndex = 2;
            // 
            // textDBUsername
            // 
            this.textDBUsername.Location = new System.Drawing.Point(97, 53);
            this.textDBUsername.Name = "textDBUsername";
            this.textDBUsername.Size = new System.Drawing.Size(285, 21);
            this.textDBUsername.TabIndex = 1;
            // 
            // textDBIP
            // 
            this.textDBIP.Location = new System.Drawing.Point(97, 24);
            this.textDBIP.Name = "textDBIP";
            this.textDBIP.Size = new System.Drawing.Size(285, 21);
            this.textDBIP.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 159);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "数据库实例：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "密码：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "用户名：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "数据库IP地址：";
            // 
            // btnDBLogin
            // 
            this.btnDBLogin.Location = new System.Drawing.Point(6, 116);
            this.btnDBLogin.Name = "btnDBLogin";
            this.btnDBLogin.Size = new System.Drawing.Size(89, 23);
            this.btnDBLogin.TabIndex = 3;
            this.btnDBLogin.Text = "测试登录";
            this.btnDBLogin.UseVisualStyleBackColor = true;
            this.btnDBLogin.Click += new System.EventHandler(this.btnDBLogin_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_SelectFile);
            this.groupBox2.Location = new System.Drawing.Point(12, 224);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(388, 70);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "考车配置";
            // 
            // btn_SelectFile
            // 
            this.btn_SelectFile.Location = new System.Drawing.Point(6, 29);
            this.btn_SelectFile.Name = "btn_SelectFile";
            this.btn_SelectFile.Size = new System.Drawing.Size(75, 23);
            this.btn_SelectFile.TabIndex = 0;
            this.btn_SelectFile.Text = "导入Excel";
            this.btn_SelectFile.UseVisualStyleBackColor = true;
            this.btn_SelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 410);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "合码器配置";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnDBLogin;
        private System.Windows.Forms.TextBox textDBPassword;
        private System.Windows.Forms.TextBox textDBUsername;
        private System.Windows.Forms.TextBox textDBIP;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboDBInstance;
        private GroupBox groupBox2;
        private Button btn_SelectFile;
    }
}

