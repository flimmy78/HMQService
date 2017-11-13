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
            this.labelState = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxDisplay1 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxDisplay2 = new System.Windows.Forms.TextBox();
            this.textBoxDisplay3 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxDisplay4 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxVideoWnd = new System.Windows.Forms.TextBox();
            this.btnSaveDisplayConf = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.comboBoxEven = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
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
            this.groupBox1.Location = new System.Drawing.Point(12, 194);
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
            this.groupBox2.Controls.Add(this.labelState);
            this.groupBox2.Controls.Add(this.btn_SelectFile);
            this.groupBox2.Location = new System.Drawing.Point(12, 406);
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
            // labelState
            // 
            this.labelState.AutoSize = true;
            this.labelState.Location = new System.Drawing.Point(97, 37);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(0, 12);
            this.labelState.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.comboBoxEven);
            this.groupBox3.Controls.Add(this.btnSaveDisplayConf);
            this.groupBox3.Controls.Add(this.textBoxDisplay2);
            this.groupBox3.Controls.Add(this.textBoxDisplay4);
            this.groupBox3.Controls.Add(this.textBoxVideoWnd);
            this.groupBox3.Controls.Add(this.textBoxDisplay3);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.textBoxDisplay1);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(12, 13);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(388, 164);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "显示配置";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "画面一位置：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "画面三位置：";
            // 
            // textBoxDisplay1
            // 
            this.textBoxDisplay1.Location = new System.Drawing.Point(94, 24);
            this.textBoxDisplay1.Name = "textBoxDisplay1";
            this.textBoxDisplay1.Size = new System.Drawing.Size(71, 21);
            this.textBoxDisplay1.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(194, 27);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 12);
            this.label7.TabIndex = 1;
            this.label7.Text = "画面二位置：";
            // 
            // textBoxDisplay2
            // 
            this.textBoxDisplay2.Location = new System.Drawing.Point(281, 24);
            this.textBoxDisplay2.Name = "textBoxDisplay2";
            this.textBoxDisplay2.Size = new System.Drawing.Size(71, 21);
            this.textBoxDisplay2.TabIndex = 7;
            // 
            // textBoxDisplay3
            // 
            this.textBoxDisplay3.Location = new System.Drawing.Point(94, 55);
            this.textBoxDisplay3.Name = "textBoxDisplay3";
            this.textBoxDisplay3.Size = new System.Drawing.Size(71, 21);
            this.textBoxDisplay3.TabIndex = 8;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(194, 58);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 12);
            this.label8.TabIndex = 1;
            this.label8.Text = "画面四位置：";
            // 
            // textBoxDisplay4
            // 
            this.textBoxDisplay4.Location = new System.Drawing.Point(281, 55);
            this.textBoxDisplay4.Name = "textBoxDisplay4";
            this.textBoxDisplay4.Size = new System.Drawing.Size(71, 21);
            this.textBoxDisplay4.TabIndex = 9;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 90);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(89, 12);
            this.label9.TabIndex = 1;
            this.label9.Text = "音频窗口位置：";
            // 
            // textBoxVideoWnd
            // 
            this.textBoxVideoWnd.Location = new System.Drawing.Point(94, 87);
            this.textBoxVideoWnd.Name = "textBoxVideoWnd";
            this.textBoxVideoWnd.Size = new System.Drawing.Size(71, 21);
            this.textBoxVideoWnd.TabIndex = 10;
            // 
            // btnSaveDisplayConf
            // 
            this.btnSaveDisplayConf.Location = new System.Drawing.Point(8, 125);
            this.btnSaveDisplayConf.Name = "btnSaveDisplayConf";
            this.btnSaveDisplayConf.Size = new System.Drawing.Size(87, 23);
            this.btnSaveDisplayConf.TabIndex = 12;
            this.btnSaveDisplayConf.Text = "保存配置";
            this.btnSaveDisplayConf.UseVisualStyleBackColor = true;
            this.btnSaveDisplayConf.Click += new System.EventHandler(this.btnSaveDisplayConf_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(194, 90);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(89, 12);
            this.label10.TabIndex = 1;
            this.label10.Text = "是否隔行解码：";
            // 
            // comboBoxEven
            // 
            this.comboBoxEven.FormattingEnabled = true;
            this.comboBoxEven.Location = new System.Drawing.Point(281, 87);
            this.comboBoxEven.Name = "comboBoxEven";
            this.comboBoxEven.Size = new System.Drawing.Size(71, 20);
            this.comboBoxEven.TabIndex = 11;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 487);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "合码器配置";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
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
        private Label labelState;
        private GroupBox groupBox3;
        private TextBox textBoxDisplay2;
        private TextBox textBoxDisplay4;
        private TextBox textBoxVideoWnd;
        private TextBox textBoxDisplay3;
        private Label label8;
        private TextBox textBoxDisplay1;
        private Label label9;
        private Label label6;
        private Label label7;
        private Label label5;
        private Button btnSaveDisplayConf;
        private ComboBox comboBoxEven;
        private Label label10;
    }
}

