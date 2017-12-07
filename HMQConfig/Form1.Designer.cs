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
            this.textDBInstance = new System.Windows.Forms.TextBox();
            this.textDBPassword = new System.Windows.Forms.TextBox();
            this.textDBUsername = new System.Windows.Forms.TextBox();
            this.textDBIP = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDBLogin = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnExportTemplate = new System.Windows.Forms.Button();
            this.labelState = new System.Windows.Forms.Label();
            this.btn_SelectFile = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxSleepTime = new System.Windows.Forms.TextBox();
            this.comboBoxXmVideo = new System.Windows.Forms.ComboBox();
            this.comboBoxCarVideo = new System.Windows.Forms.ComboBox();
            this.comboBoxStudentInfo = new System.Windows.Forms.ComboBox();
            this.comboBoxExamInfo = new System.Windows.Forms.ComboBox();
            this.comboBoxAudio = new System.Windows.Forms.ComboBox();
            this.comboBoxWnd2 = new System.Windows.Forms.ComboBox();
            this.comboBoxEven = new System.Windows.Forms.ComboBox();
            this.btnSaveDisplayConf = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnImportMap = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textDBInstance);
            this.groupBox1.Controls.Add(this.textDBPassword);
            this.groupBox1.Controls.Add(this.textDBUsername);
            this.groupBox1.Controls.Add(this.textDBIP);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnDBLogin);
            this.groupBox1.Location = new System.Drawing.Point(12, 221);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(388, 193);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "数据库配置";
            // 
            // textDBInstance
            // 
            this.textDBInstance.Location = new System.Drawing.Point(97, 107);
            this.textDBInstance.Name = "textDBInstance";
            this.textDBInstance.Size = new System.Drawing.Size(285, 21);
            this.textDBInstance.TabIndex = 3;
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
            this.label4.Location = new System.Drawing.Point(4, 110);
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
            this.btnDBLogin.Location = new System.Drawing.Point(6, 145);
            this.btnDBLogin.Name = "btnDBLogin";
            this.btnDBLogin.Size = new System.Drawing.Size(89, 23);
            this.btnDBLogin.TabIndex = 4;
            this.btnDBLogin.Text = "测试登录";
            this.btnDBLogin.UseVisualStyleBackColor = true;
            this.btnDBLogin.Click += new System.EventHandler(this.btnDBLogin_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnExportTemplate);
            this.groupBox2.Controls.Add(this.labelState);
            this.groupBox2.Controls.Add(this.btn_SelectFile);
            this.groupBox2.Location = new System.Drawing.Point(12, 433);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(388, 71);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "考车配置";
            // 
            // btnExportTemplate
            // 
            this.btnExportTemplate.Location = new System.Drawing.Point(8, 29);
            this.btnExportTemplate.Name = "btnExportTemplate";
            this.btnExportTemplate.Size = new System.Drawing.Size(87, 23);
            this.btnExportTemplate.TabIndex = 5;
            this.btnExportTemplate.Text = "导出模板";
            this.btnExportTemplate.UseVisualStyleBackColor = true;
            this.btnExportTemplate.Click += new System.EventHandler(this.btnExportTemplate_Click);
            // 
            // labelState
            // 
            this.labelState.AutoSize = true;
            this.labelState.Location = new System.Drawing.Point(15, 67);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(0, 12);
            this.labelState.TabIndex = 1;
            // 
            // btn_SelectFile
            // 
            this.btn_SelectFile.Location = new System.Drawing.Point(106, 29);
            this.btn_SelectFile.Name = "btn_SelectFile";
            this.btn_SelectFile.Size = new System.Drawing.Size(87, 23);
            this.btn_SelectFile.TabIndex = 6;
            this.btn_SelectFile.Text = "导入配置";
            this.btn_SelectFile.UseVisualStyleBackColor = true;
            this.btn_SelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxSleepTime);
            this.groupBox3.Controls.Add(this.comboBoxXmVideo);
            this.groupBox3.Controls.Add(this.comboBoxCarVideo);
            this.groupBox3.Controls.Add(this.comboBoxStudentInfo);
            this.groupBox3.Controls.Add(this.comboBoxExamInfo);
            this.groupBox3.Controls.Add(this.comboBoxAudio);
            this.groupBox3.Controls.Add(this.comboBoxWnd2);
            this.groupBox3.Controls.Add(this.comboBoxEven);
            this.groupBox3.Controls.Add(this.btnSaveDisplayConf);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(12, 13);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(388, 191);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "显示配置";
            // 
            // textBoxSleepTime
            // 
            this.textBoxSleepTime.Location = new System.Drawing.Point(311, 119);
            this.textBoxSleepTime.Name = "textBoxSleepTime";
            this.textBoxSleepTime.Size = new System.Drawing.Size(48, 21);
            this.textBoxSleepTime.TabIndex = 15;
            // 
            // comboBoxXmVideo
            // 
            this.comboBoxXmVideo.FormattingEnabled = true;
            this.comboBoxXmVideo.Location = new System.Drawing.Point(311, 24);
            this.comboBoxXmVideo.Name = "comboBoxXmVideo";
            this.comboBoxXmVideo.Size = new System.Drawing.Size(71, 20);
            this.comboBoxXmVideo.TabIndex = 9;
            // 
            // comboBoxCarVideo
            // 
            this.comboBoxCarVideo.FormattingEnabled = true;
            this.comboBoxCarVideo.Location = new System.Drawing.Point(106, 24);
            this.comboBoxCarVideo.Name = "comboBoxCarVideo";
            this.comboBoxCarVideo.Size = new System.Drawing.Size(71, 20);
            this.comboBoxCarVideo.TabIndex = 8;
            // 
            // comboBoxStudentInfo
            // 
            this.comboBoxStudentInfo.FormattingEnabled = true;
            this.comboBoxStudentInfo.Location = new System.Drawing.Point(106, 55);
            this.comboBoxStudentInfo.Name = "comboBoxStudentInfo";
            this.comboBoxStudentInfo.Size = new System.Drawing.Size(71, 20);
            this.comboBoxStudentInfo.TabIndex = 10;
            // 
            // comboBoxExamInfo
            // 
            this.comboBoxExamInfo.FormattingEnabled = true;
            this.comboBoxExamInfo.Location = new System.Drawing.Point(311, 55);
            this.comboBoxExamInfo.Name = "comboBoxExamInfo";
            this.comboBoxExamInfo.Size = new System.Drawing.Size(71, 20);
            this.comboBoxExamInfo.TabIndex = 11;
            // 
            // comboBoxAudio
            // 
            this.comboBoxAudio.FormattingEnabled = true;
            this.comboBoxAudio.Location = new System.Drawing.Point(106, 87);
            this.comboBoxAudio.Name = "comboBoxAudio";
            this.comboBoxAudio.Size = new System.Drawing.Size(71, 20);
            this.comboBoxAudio.TabIndex = 12;
            // 
            // comboBoxWnd2
            // 
            this.comboBoxWnd2.FormattingEnabled = true;
            this.comboBoxWnd2.Location = new System.Drawing.Point(106, 119);
            this.comboBoxWnd2.Name = "comboBoxWnd2";
            this.comboBoxWnd2.Size = new System.Drawing.Size(71, 20);
            this.comboBoxWnd2.TabIndex = 14;
            // 
            // comboBoxEven
            // 
            this.comboBoxEven.FormattingEnabled = true;
            this.comboBoxEven.Location = new System.Drawing.Point(311, 87);
            this.comboBoxEven.Name = "comboBoxEven";
            this.comboBoxEven.Size = new System.Drawing.Size(71, 20);
            this.comboBoxEven.TabIndex = 13;
            // 
            // btnSaveDisplayConf
            // 
            this.btnSaveDisplayConf.Location = new System.Drawing.Point(8, 150);
            this.btnSaveDisplayConf.Name = "btnSaveDisplayConf";
            this.btnSaveDisplayConf.Size = new System.Drawing.Size(87, 23);
            this.btnSaveDisplayConf.TabIndex = 16;
            this.btnSaveDisplayConf.Text = "保存配置";
            this.btnSaveDisplayConf.UseVisualStyleBackColor = true;
            this.btnSaveDisplayConf.Click += new System.EventHandler(this.btnSaveDisplayConf_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(206, 58);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 12);
            this.label8.TabIndex = 1;
            this.label8.Text = "实时信息位置：";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(206, 90);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(89, 12);
            this.label10.TabIndex = 1;
            this.label10.Text = "是否隔行解码：";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(365, 122);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(17, 12);
            this.label13.TabIndex = 1;
            this.label13.Text = "ms";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(206, 122);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(89, 12);
            this.label12.TabIndex = 1;
            this.label12.Text = "画面刷新间隔：";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 122);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(89, 12);
            this.label11.TabIndex = 1;
            this.label11.Text = "项目动态切换：";
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
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "考生信息位置：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(206, 27);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 12);
            this.label7.TabIndex = 1;
            this.label7.Text = "车外视频位置：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "车内视频位置：";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnImportMap);
            this.groupBox4.Location = new System.Drawing.Point(12, 515);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(388, 63);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "地图配置";
            // 
            // btnImportMap
            // 
            this.btnImportMap.Location = new System.Drawing.Point(8, 23);
            this.btnImportMap.Name = "btnImportMap";
            this.btnImportMap.Size = new System.Drawing.Size(87, 23);
            this.btnImportMap.TabIndex = 7;
            this.btnImportMap.Text = "导入地图";
            this.btnImportMap.UseVisualStyleBackColor = true;
            this.btnImportMap.Click += new System.EventHandler(this.btnImportMap_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 590);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "四合一配置";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
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
        private GroupBox groupBox2;
        private Button btn_SelectFile;
        private Label labelState;
        private GroupBox groupBox3;
        private Label label8;
        private Label label9;
        private Label label6;
        private Label label7;
        private Label label5;
        private Button btnSaveDisplayConf;
        private ComboBox comboBoxEven;
        private Label label10;
        private ComboBox comboBoxWnd2;
        private Label label11;
        private Button btnExportTemplate;
        private ComboBox comboBoxXmVideo;
        private ComboBox comboBoxCarVideo;
        private ComboBox comboBoxStudentInfo;
        private ComboBox comboBoxExamInfo;
        private ComboBox comboBoxAudio;
        private TextBox textBoxSleepTime;
        private Label label13;
        private Label label12;
        private TextBox textDBInstance;
        private GroupBox groupBox4;
        private Button btnImportMap;
    }
}

