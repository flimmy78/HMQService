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
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;

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

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            //选择 Excel
            string excelFilePath = string.Empty;
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = @"Excel Files (*.xlsx)|*.xlsx";
            if (DialogResult.OK == fd.ShowDialog())
            {
                excelFilePath = fd.FileName;
            }
            else
            {
                return;
            }

            //解析 Excel
            string errorMsg = string.Empty;
            Dictionary<string, HMQConf> dicHmq = new Dictionary<string, HMQConf>();
            bool bRet = ReadFromExcel(excelFilePath, ref dicHmq, out errorMsg);
            if (!bRet)
            {
                Log.GetLogger().ErrorFormat(errorMsg);
                MessageBox.Show(errorMsg);
            }

        }

        private bool ReadFromExcel(string filePath, ref Dictionary<string, HMQConf> dicHmq, out string errorMsg)
        {
            errorMsg = string.Empty;

            try
            {
                //加载 excel
                XSSFWorkbook wk = null;
                using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    wk = new XSSFWorkbook(fs);
                    fs.Close();
                }
                if (null == wk)
                {
                    errorMsg = string.Format("加载文件 {0} 失败，请检查 excel 文件。", filePath);
                    goto END;
                }

                #region 读取考车配置
                string sheetName = BaseDefine.EXCEL_SHEET_NAME_CAR;
                ISheet sheet = wk.GetSheet(sheetName);
                if (null == sheet)
                {
                    errorMsg = string.Format("找不到名称为 {0} 的 Sheet 页，请检查 excel 文件。", sheetName);
                    goto END;
                }
                if (sheet.LastRowNum <= 1)
                {
                    errorMsg = string.Format("Sheet 页 : {0} 的行数为 {1}，请检查 excel 文件。", sheetName, sheet.LastRowNum);
                    goto END;
                }
                for (int i = 1; i <= sheet.LastRowNum; i++)  //跳过第一行
                {
                    IRow row = sheet.GetRow(i);
                    if (null == row)
                    {
                        errorMsg = string.Format("读取 Sheet 页 : {0} 的第 {1} 行时发生错误，请检查 excel 文件。", sheetName, i + 1);
                        goto END;
                    }

                    try
                    {
                        string hmqIp = row.GetCell(0).StringCellValue;  //合码器IP
                        double hmqPort = row.GetCell(1).NumericCellValue;   //合码器端口
                        string hmqUsername = row.GetCell(2).StringCellValue;    //合码器用户名
                        string hmqPassword = row.GetCell(3).StringCellValue;    //合码器密码
                        double hmqTranNo = row.GetCell(4).NumericCellValue; //合码器通道号
                        double carNo = row.GetCell(5).NumericCellValue; //考车号

                        int nPort = (int)hmqPort;
                        int nTranNo = (int)hmqTranNo;
                        int nCarNo = (int)carNo;

                        if (nPort <= 0 || nTranNo <= 0 || nCarNo <= 0 || string.IsNullOrEmpty(hmqIp) || string.IsNullOrEmpty(hmqUsername)
                            || string.IsNullOrEmpty(hmqPassword))
                        {
                            errorMsg = string.Format("Sheet 页 : {0} 的第 {1} 行存在错误数据，请检查 excel 文件。", sheetName, i + 1);
                            goto END;
                        }

                        if (!dicHmq.ContainsKey(hmqIp))
                        {
                            Dictionary<int, int> dicTrans = new Dictionary<int, int>();
                            dicTrans.Add(nTranNo, nCarNo);

                            HMQConf hmqConf = new HMQConf(hmqIp, nPort, hmqUsername, hmqPassword, dicTrans);

                            dicHmq.Add(hmqIp, hmqConf);
                        }
                        else
                        {
                            if (!dicHmq[hmqIp].AddItem(nTranNo, nCarNo))
                            {
                                errorMsg = string.Format("Sheet 页 : {0} 的第 {1} 行错误，存在重复的通道号，请检查 excel 文件。", sheetName, i + 1);
                                goto END;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        errorMsg = string.Format("Sheet 页 : {0} 的第 {1} 行存在错误数据，请检查 excel 文件。", sheetName, i + 1);
                        goto END;
                    }
                }
                #endregion

                #region 读取摄像头配置

                #endregion

            }
            catch (Exception e)
            {
                errorMsg = string.Format("读取文件 {0} 失败，error = {1}", filePath, e.Message);
                goto END;
            }

            END:
            {
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
