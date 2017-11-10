using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BekUtils.Database;
using BekUtils.Util;
using log4net;

namespace HMQConfig
{
    public partial class Form1 : Form
    {
        private IDataProvider m_dbProvider = null;
        private string m_dbAddress;
        private string m_dbUsername;
        private string m_dbPassword;
        private string m_dbInstance;

        public Form1()
        {
            m_dbAddress = string.Empty;
            m_dbUsername = string.Empty;
            m_dbPassword = string.Empty;
            m_dbInstance = string.Empty;

            InitializeComponent();
        }

        private void btnDBLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textDBIP.Text) || string.IsNullOrEmpty(textDBUsername.Text) || string.IsNullOrEmpty(textDBPassword.Text))
            {
                MessageBox.Show("数据库IP、用户名、密码不能为空。");
                goto END;
            }

            m_dbAddress = textDBIP.Text;
            m_dbUsername = textDBUsername.Text;
            m_dbPassword = textDBPassword.Text;

            //禁用控件
            textDBIP.Enabled = false;
            textDBUsername.Enabled = false;
            textDBPassword.Enabled = false;
            comboDBInstance.Enabled = false;

            //清空数据库实例
            comboDBInstance.BeginUpdate();
            comboDBInstance.Items.Clear();
            comboDBInstance.DataSource = null;
            comboDBInstance.Text = "";
            comboDBInstance.EndUpdate();

            List<string> dbNames = new List<string>();
            string connStr = string.Format(BaseDefine.DATABASE_CONN_FORMAT, textDBIP.Text,
                BaseDefine.DATABASE_NAME_MASTER, textDBUsername.Text, textDBPassword.Text);

            try
            {
                //连接数据库
                int dbType = INIOperator.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH_ENV, BaseDefine.CONFIG_SECTION_CONFIG,
                    BaseDefine.CONFIG_KEY_DBADDRESS, 1);
                if (1 == dbType)
                {
                    Log.GetLogger().InfoFormat("数据库类型为：SqlServer");
                    m_dbProvider = DataProvider.CreateDataProvider(DataProvider.DataProviderType.SqlDataProvider, connStr);
                }
                else
                {
                    Log.GetLogger().InfoFormat("数据库类型为：Oracle");
                    m_dbProvider = DataProvider.CreateDataProvider(DataProvider.DataProviderType.OracleDataProvider, connStr);
                }

                //遍历数据库实例名
                DataSet ds = m_dbProvider.RetriveDataSet("select name from master.dbo.sysdatabases;");
                if (null != ds)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string dbName = ds.Tables[0].Rows[i][0].ToString();
                        if (!string.IsNullOrEmpty(dbName))
                        {
                            Log.GetLogger().DebugFormat("find database instance : {0}", dbName);
                            dbNames.Add(dbName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", ex.Message);
            }

            if (0 == dbNames.Count)
            {
                Log.GetLogger().ErrorFormat("未能找到对应的数据库实例，请检查数据库配置。ip={0}, username={1}", textDBIP.Text, textDBUsername.Text);
                MessageBox.Show("未能找到对应的数据库实例，请检查数据库配置。");
                goto END;
            }
            else
            {
                //将数据库配置写入配置文件
                string base64DbAddress = Base64Util.Base64Encode(m_dbAddress);
                string base64DbUserName = Base64Util.Base64Encode(m_dbUsername);
                string base64DbPassword = Base64Util.Base64Encode(m_dbPassword);
                INIOperator.INIWriteValue(BaseDefine.CONFIG_FILE_PATH_DB, BaseDefine.CONFIG_SECTION_CONFIG,
                    BaseDefine.CONFIG_KEY_DBADDRESS, base64DbAddress);
                INIOperator.INIWriteValue(BaseDefine.CONFIG_FILE_PATH_DB, BaseDefine.CONFIG_SECTION_CONFIG,
                    BaseDefine.CONFIG_KEY_USERNAME, base64DbUserName);
                INIOperator.INIWriteValue(BaseDefine.CONFIG_FILE_PATH_DB, BaseDefine.CONFIG_SECTION_CONFIG,
                    BaseDefine.CONFIG_KEY_PASSWORD, base64DbPassword);

                //更新数据库实例下拉框
                comboDBInstance.BeginUpdate();
                foreach (string dbName in dbNames)
                {
                    comboDBInstance.Items.Add(dbName);
                }
                comboDBInstance.EndUpdate();

                MessageBox.Show("数据库连接成功，请选择一个数据库实例。");
            }

            END:
            {
                //恢复控件
                textDBIP.Enabled = true;
                textDBUsername.Enabled = true;
                textDBPassword.Enabled = true;
                comboDBInstance.Enabled = true;
            }
        }

        private void comboDBInstance_SelectedIndexChanged(object sender, EventArgs e)
        {
            Log.GetLogger().DebugFormat("选择数据库实例：{0}", comboDBInstance.Text);

            m_dbInstance = comboDBInstance.Text;

            string base64Instance = Base64Util.Base64Encode(m_dbInstance);
            INIOperator.INIWriteValue(BaseDefine.CONFIG_FILE_PATH_DB, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_INSTANCE, base64Instance);
        }
    }
}
