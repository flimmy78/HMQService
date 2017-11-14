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
        private int m_dbType;

        public Form1()
        {
            m_dbAddress = string.Empty;
            m_dbUsername = string.Empty;
            m_dbPassword = string.Empty;
            m_dbInstance = string.Empty;
            m_dbType = -1;

            //初始化 log4net 配置信息
            log4net.Config.XmlConfigurator.Configure();

            //初始化海康SDK
            HikUtils.HikUtils.InitDevice();

            InitializeComponent();

            //读取初始配置
            int nDisplay1 = INIOperator.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH_DISPLAY, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_DISPLAY1, 0);
            int nDisplay2 = INIOperator.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH_DISPLAY, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_DISPLAY2, 1);
            int nDisplay3 = INIOperator.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH_DISPLAY, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_DISPLAY3, 2);
            int nDisplay4 = INIOperator.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH_DISPLAY, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_DISPLAY4, 3);
            int nVideoWnd = INIOperator.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH_DISPLAY, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_VIDEOWND, 1);
            int nEven = INIOperator.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH_DISPLAY, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_EVEN, 0);
            int nWnd2 = INIOperator.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH_DISPLAY, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_WND2, 1);

            textBoxDisplay1.Text = nDisplay1.ToString();
            textBoxDisplay2.Text = nDisplay2.ToString();
            textBoxDisplay3.Text = nDisplay3.ToString();
            textBoxDisplay4.Text = nDisplay4.ToString();
            textBoxVideoWnd.Text = nVideoWnd.ToString();

            //是否隔行解码
            comboBoxEven.BeginUpdate();
            comboBoxEven.Items.Add(BaseDefine.STRING_EVEN_NO);
            comboBoxEven.Items.Add(BaseDefine.STRING_EVEN_YES);
            if (1 == nEven)
            {
                comboBoxEven.SelectedIndex = 1;
            }
            else
            {
                comboBoxEven.SelectedIndex = 0;
            }
            comboBoxEven.EndUpdate();

            //画面二是否自动切换项目
            comboBoxWnd2.BeginUpdate();
            comboBoxWnd2.Items.Add(BaseDefine.STRING_WND2_YES);
            comboBoxWnd2.Items.Add(BaseDefine.STRING_WND2_NO);
            if (1 == nWnd2)
            {
                comboBoxWnd2.SelectedIndex = 0;
            }
            else
            {
                comboBoxWnd2.SelectedIndex = 1;
            }
            comboBoxWnd2.EndUpdate();

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
            labelState.Text = string.Empty;

            List<string> dbNames = new List<string>();
            string connStr = string.Format(BaseDefine.DB_CONN_FORMAT, textDBIP.Text,
                BaseDefine.DB_NAME_MASTER, textDBUsername.Text, textDBPassword.Text);

            try
            {
                //连接数据库
                m_dbType = INIOperator.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH_ENV, BaseDefine.CONFIG_SECTION_CONFIG,
                    BaseDefine.CONFIG_KEY_DBADDRESS, 1);
                if (1 == m_dbType)
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
            if (string.IsNullOrEmpty(m_dbAddress) || string.IsNullOrEmpty(m_dbUsername) || 
                string.IsNullOrEmpty(m_dbPassword) || string.IsNullOrEmpty(m_dbInstance))
            {
                MessageBox.Show("请先配置数据库连接。");
                return;
            }

            labelState.Text = string.Empty;

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
            Dictionary<string, CameraConf> dicCamera = new Dictionary<string, CameraConf>();
            bool bRet = ReadFromExcel(excelFilePath, ref dicHmq, ref dicCamera, out errorMsg);
            if (!bRet)
            {
                Log.GetLogger().ErrorFormat(errorMsg);
                MessageBox.Show(errorMsg);
                return;
            }

            //将合码器/解码器通道配置写入本地配置文件
            bRet = WriteHMQConfToIni(dicHmq, out errorMsg);
            if (!bRet)
            {
                Log.GetLogger().ErrorFormat(errorMsg);
                MessageBox.Show(errorMsg);
                return;
            }

            //将摄像头配置写入数据库
            bRet = WriteCameraConfToDB(dicCamera, out errorMsg);
            if (!bRet)
            {
                Log.GetLogger().ErrorFormat(errorMsg);
                MessageBox.Show(errorMsg);
                return;
            }

            labelState.Text = string.Format("成功导入 {0}", excelFilePath);
            Log.GetLogger().InfoFormat("导入Excel配置成功");
            MessageBox.Show("导入Excel配置成功");
        }

        private bool ReadFromExcel(string filePath, ref Dictionary<string, HMQConf> dicHmq, 
            ref Dictionary<string, CameraConf> dicCamera, out string errorMsg)
        {
            errorMsg = string.Empty;
            dicHmq.Clear();
            dicCamera.Clear();

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

                #region 读取通道配置
                string sheetName = BaseDefine.EXCEL_SHEET_NAME_CONF_TRANS;
                ISheet sheet = wk.GetSheet(sheetName);
                if (null == sheet)
                {
                    errorMsg = string.Format("找不到名称为 {0} 的 Sheet 页，请检查 excel 文件。", sheetName);
                    goto END;
                }
                if (sheet.LastRowNum < 1)
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
                        string hmqIp = GetStringCellValue(row, 0);  //合码器IP
                        int nPort = GetIntCellValue(row, 1);   //合码器端口
                        string hmqUsername = GetStringCellValue(row, 2);    //合码器用户名
                        string hmqPassword = GetStringCellValue(row, 3);    //合码器密码
                        int nTranNo = GetIntCellValue(row, 4); //合码器通道号
                        int nCarNo = GetIntCellValue(row, 5); //考车号
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

                #region 读取车载摄像头配置
                sheetName = BaseDefine.EXCEL_SHEET_NAME_CONF_CAMERA_CAR;
                sheet = wk.GetSheet(sheetName);
                if (null == sheet)
                {
                    errorMsg = string.Format("找不到名称为 {0} 的 Sheet 页，请检查 excel 文件。", sheetName);
                    goto END;
                }
                if (sheet.LastRowNum < 1)
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
                        int nCarNo = GetIntCellValue(row, 0); //考车号
                        string deviceIP = GetStringCellValue(row, 1);   //设备IP
                        string username = GetStringCellValue(row, 2);   //用户名
                        string password = GetStringCellValue(row, 3);   //密码
                        int nPort = GetIntCellValue(row, 4);  //端口
                        int nTranNo = GetIntCellValue(row, 5);   //通道号
                        int nCameraNo = GetIntCellValue(row, 6); //摄像头编号
                        string bitStreamType = GetStringCellValue(row, 7);  //码流类型
                        string mediaIP = GetStringCellValue(row, 8);    //流媒体IP
                        if (nCarNo <= 0 || nPort <= 0 || nTranNo <= 0 || nCameraNo<=0 || string.IsNullOrEmpty(deviceIP) || string.IsNullOrEmpty(username)
                            || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(bitStreamType))
                        {
                            errorMsg = string.Format("Sheet 页 : {0} 的第 {1} 行存在错误数据，请检查 excel 文件。", sheetName, i + 1);
                            goto END;
                        }

                        string key = string.Format("考车{0}_{1}", nCarNo.ToString(), nCameraNo.ToString());
                        if (!dicCamera.ContainsKey(key))
                        {
                            int nBitStreamType = 0;
                            if (BaseDefine.STRING_BITSTREAM_MASTER == bitStreamType)
                            {
                                nBitStreamType = 0; //主码流
                            }
                            else
                            {
                                nBitStreamType = 1; //子码流
                            }

                            string bz = "考车" + nCarNo.ToString();
                            CameraConf camera = new CameraConf(key, deviceIP, username, password, mediaIP, nPort, nTranNo, nBitStreamType, bz);

                            dicCamera.Add(key, camera);
                        }
                        else
                        {
                            errorMsg = string.Format("Sheet 页 : {0} 的第 {1} 行错误，存在重复的考车摄像头编号，请检查 excel 文件。", sheetName, i + 1);
                            goto END;
                        }
                    }
                    catch (Exception e)
                    {
                        errorMsg = string.Format("Sheet 页 : {0} 的第 {1} 行存在错误数据，请检查 excel 文件。", sheetName, i + 1);
                        goto END;
                    }
                }
                #endregion

                #region 读取项目摄像头配置
                sheetName = BaseDefine.EXCEL_SHEET_NAME_CONF_CAMERA_XM;
                sheet = wk.GetSheet(sheetName);
                if (null == sheet)
                {
                    errorMsg = string.Format("找不到名称为 {0} 的 Sheet 页，请检查 excel 文件。", sheetName);
                    goto END;
                }
                if (sheet.LastRowNum < 1)
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
                        int nXmNo = GetIntCellValue(row, 0); //项目编号
                        string xmName = GetStringCellValue(row, 1); //项目名称
                        string deviceIP = GetStringCellValue(row, 2);   //设备IP
                        string username = GetStringCellValue(row, 3);   //用户名
                        string password = GetStringCellValue(row, 4);   //密码
                        int nPort = GetIntCellValue(row, 5);  //端口
                        int nTranNo = GetIntCellValue(row, 6);   //通道号
                        string bitStreamType = GetStringCellValue(row, 7);  //码流类型
                        string mediaIP = GetStringCellValue(row, 8);    //流媒体IP
                        if (nXmNo <= 0 || nPort <= 0 || nTranNo <= 0 || string.IsNullOrEmpty(deviceIP) || string.IsNullOrEmpty(username)
                            || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(bitStreamType))
                        {
                            errorMsg = string.Format("Sheet 页 : {0} 的第 {1} 行存在错误数据，请检查 excel 文件。", sheetName, i + 1);
                            goto END;
                        }

                        string key = string.Format("{0}_1", nXmNo.ToString());
                        if (!dicCamera.ContainsKey(key))
                        {
                            int nBitStreamType = 0;
                            if (BaseDefine.STRING_BITSTREAM_MASTER == bitStreamType)
                            {
                                nBitStreamType = 0; //主码流
                            }
                            else
                            {
                                nBitStreamType = 1; //子码流
                            }

                            CameraConf camera = new CameraConf(key, deviceIP, username, password, mediaIP, nPort, nTranNo, nBitStreamType, xmName);

                            dicCamera.Add(key, camera);
                        }
                        else
                        {
                            errorMsg = string.Format("Sheet 页 : {0} 的第 {1} 行错误，存在重复的项目摄像头编号，请检查 excel 文件。", sheetName, i + 1);
                            goto END;
                        }
                    }
                    catch (Exception e)
                    {
                        errorMsg = string.Format("Sheet 页 : {0} 的第 {1} 行存在错误数据，请检查 excel 文件。", sheetName, i + 1);
                        goto END;
                    }
                }
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

        private string GetStringCellValue(IRow row, int index)
        {
            string retStr = string.Empty;
            if (null == row || index < 0)
            {
                return retStr;
            }

            try
            {
                ICell cell = row.GetCell(index);
                if (null == cell)
                {
                    return retStr;
                }

                retStr = cell.StringCellValue;
            }
            catch(Exception e)
            {
            }

            return retStr;
        }

        private int GetIntCellValue(IRow row, int index)
        {
            int nRet = 0;
            if (null == row || index < 0)
            {
                return nRet;
            }

            try
            {
                ICell cell = row.GetCell(index);
                if (null == cell)
                {
                    return nRet;
                }

                double dValue = cell.NumericCellValue;
                nRet = (int)dValue;
            }
            catch (Exception e)
            {
            }

            return nRet;
        }

        private bool WriteHMQConfToIni(Dictionary<string, HMQConf> dicHmq, out string errorMsg)
        {
            errorMsg = string.Empty;

            if (File.Exists(BaseDefine.CONFIG_FILE_PATH_CAR))
            {
                File.Delete(BaseDefine.CONFIG_FILE_PATH_CAR);
            }

            int nCount = dicHmq.Count;
            bool bRet = INIOperator.INIWriteValue(BaseDefine.CONFIG_FILE_PATH_CAR, BaseDefine.CONFIG_SECTION_JMQ,
                BaseDefine.CONFIG_KEY_NUM, nCount.ToString());

            int nIndex = 1;
            foreach(HMQConf hmq in dicHmq.Values)
            {
                string key = nIndex.ToString();
                string value = string.Format("{0},{1},{2},{3}", hmq.Ip, hmq.Username, hmq.Password, hmq.Port);
                bRet = INIOperator.INIWriteValue(BaseDefine.CONFIG_FILE_PATH_CAR, BaseDefine.CONFIG_SECTION_JMQ, key, value);

                string section = string.Format("{0}{1}", BaseDefine.CONFIG_SECTION_JMQ, nIndex);    //JMQ1、JMQ2
                foreach(int tranNo in hmq.DicTran2Car.Keys)
                {
                    int CarNo = hmq.DicTran2Car[tranNo];

                    key = string.Format("{0}{1}", BaseDefine.CONFIG_KEY_BNC, tranNo);   //BNC1、BNC2
                    bRet = INIOperator.INIWriteValue(BaseDefine.CONFIG_FILE_PATH_CAR, section, key, CarNo.ToString());
                }

                nIndex++;
            }

            return true;
        }

        private bool WriteCameraConfToDB(Dictionary<string, CameraConf> dicCamera, out string errorMsg)
        {
            errorMsg = string.Empty;

            IDataProvider sqlProvider = null;
            string connStr = string.Format(BaseDefine.DB_CONN_FORMAT, m_dbAddress,
                m_dbInstance, m_dbUsername, m_dbPassword);

            try
            {
                if (1 == m_dbType)
                {
                    sqlProvider = DataProvider.CreateDataProvider(DataProvider.DataProviderType.SqlDataProvider, connStr);
                }
                else
                {
                    sqlProvider = DataProvider.CreateDataProvider(DataProvider.DataProviderType.OracleDataProvider, connStr);
                }

                if (null == sqlProvider)
                {
                    errorMsg = string.Format("连接数据库失败，connStr={0}", connStr);
                    return false;
                }

                foreach(string key in dicCamera.Keys)
                {
                    try
                    {
                        string[] strArray = BaseMethod.SplitString(key, '_', out errorMsg);
                        if (strArray.Length != 2)
                        {
                            errorMsg = string.Format("摄像头配置存在错误的键值:{0}", key);
                            return false;
                        }

                        string bh = strArray[0];
                        string nid = strArray[1];
                        if (string.IsNullOrEmpty(bh) || string.IsNullOrEmpty(nid))
                        {
                            errorMsg = string.Format("摄像头配置存在错误的键值:{0}", key);
                            return false;
                        }

                        CameraConf camera = dicCamera[key];

                        //先删除旧记录
                        string sql = string.Format("delete from {0} where {1}='{2}' and {3}='{4}';", BaseDefine.DB_TABLE_TBKVIDEO,
                            BaseDefine.DB_FIELD_BH, bh, BaseDefine.DB_FIELD_NID, nid);
                        int nRet = sqlProvider.ExecuteNonQuery(sql);
                        if (nRet < 0)
                        {
                            Log.GetLogger().ErrorFormat("delete error，nRet = {0}, sql={1}", nRet, sql);
                        }

                        System.Threading.Thread.Sleep(10);

                        //插入新记录
                        sql = string.Format("insert into {0}({1},{2},{3},{4},{5},{6},{7},{8},{9},{10}) values('{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}');",
                            BaseDefine.DB_TABLE_TBKVIDEO,
                            BaseDefine.DB_FIELD_BH,
                            BaseDefine.DB_FIELD_SBIP,
                            BaseDefine.DB_FIELD_DKH,
                            BaseDefine.DB_FIELD_YHM,
                            BaseDefine.DB_FIELD_MM,
                            BaseDefine.DB_FIELD_TDH,
                            BaseDefine.DB_FIELD_BZ,
                            BaseDefine.DB_FIELD_NID,
                            BaseDefine.DB_FIELD_MEDIAIP,
                            BaseDefine.DB_FIELD_TRANSMODE,
                            bh, camera.CameraIP, camera.CameraPort, camera.RasUser,
                            camera.RasPassword, camera.DwChannel, camera.Bz, nid, camera.MediaIP, camera.Mllx);
                        nRet = sqlProvider.ExecuteNonQuery(sql);
                        if (nRet != 1)
                        {
                            Log.GetLogger().ErrorFormat("insert error，nRet = {0}, sql={1}", nRet, sql);
                        }

                        //System.Threading.Thread.Sleep(1000);
                    }
                    catch(Exception e)
                    {
                        Log.GetLogger().DebugFormat("execute sql catch an error, {0}", e.Message);
                    }
                    
                }

                sqlProvider.Dispose();
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
                return false;
            }

            return true;
        }

        private bool ReadHMQConfFromIni(out Dictionary<string, HMQConf> dicHmq, out string errorMsg)
        {
            dicHmq = new Dictionary<string, HMQConf>();
            errorMsg = string.Empty;

            //读取解码设备数量 
            int nCount = INIOperator.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH_CAR, BaseDefine.CONFIG_SECTION_JMQ,
                BaseDefine.CONFIG_KEY_NUM, 0);
            if (0 == nCount)
            {
                Log.GetLogger().InfoFormat("读取到解码设备数量为0");
                return true;
            }

            //获取解码设备类型
            int nType = INIOperator.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH_ENV, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_HMQ, 1);

            for (int i = 1; i <= nCount; i++)
            {
                string hmqInfo = INIOperator.INIGetStringValue(BaseDefine.CONFIG_FILE_PATH_CAR, BaseDefine.CONFIG_SECTION_JMQ,
                    i.ToString(), "");
                if (string.IsNullOrEmpty(hmqInfo))
                {
                    errorMsg = string.Format("读取合码器配置存在异常，key={0}", i);
                    return false;
                }
                string[] strArray = BaseMethod.SplitString(hmqInfo, ',', out errorMsg);
                if (!string.IsNullOrEmpty(errorMsg) || strArray.Length != 4)
                {
                    errorMsg = string.Format("读取合码器配置存在异常，key={0}, value={1}", i, hmqInfo);
                    return false;
                }
                string ip = strArray[0];
                string username = strArray[1];
                string password = strArray[2];
                string port = strArray[3];
                if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(port))
                {
                    errorMsg = string.Format("读取合码器配置存在异常，key={0}, value={1}", i, hmqInfo);
                    return false;
                }

                //登录合码器
                int userId = -1;
                int nPort = string.IsNullOrEmpty(port) ? 8000 : int.Parse(port);
                if (!HikUtils.HikUtils.LoginHikDevice(ip, username, password, nPort, out userId))
                {
                    errorMsg = string.Format("合码器登录失败，key={0}, value={1}", i, hmqInfo);
                    continue;
                }

                //获取设备能力集
                HikUtils.CHCNetSDK.NET_DVR_MATRIX_ABILITY_V41 struDecAbility = new HikUtils.CHCNetSDK.NET_DVR_MATRIX_ABILITY_V41();
                if (!HikUtils.HikUtils.GetDeviceAbility(userId, ref struDecAbility))
                {
                    errorMsg = string.Format("获取设备能力集失败，key={0}, value={1}", i, hmqInfo);
                    continue;
                }

                int chanCount = 0;
                if (1 == nType) //合码器
                {
                    chanCount = struDecAbility.struDviInfo.byChanNums;
                }
                else  //解码器
                {
                    chanCount = struDecAbility.struBncInfo.byChanNums;
                }

                Dictionary<int, int> dicTrans = new Dictionary<int, int>();
                string section = string.Format("{0}{1}", BaseDefine.CONFIG_SECTION_JMQ, i); //JMQ1、JMQ2
                for (int j = 1; j <= chanCount; j++)    //通道号
                {
                    string key = string.Format("{0}{1}", BaseDefine.CONFIG_KEY_BNC, j);     //BNC1、BNC2

                    int kch = INIOperator.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH_CAR, section, key, 0);
                    if (kch > 0 && !dicTrans.ContainsKey(j))
                    {
                        dicTrans.Add(j, kch);
                    }
                }

                if (dicTrans.Count > 0 && !dicHmq.ContainsKey(ip))
                {
                    HMQConf hmq = new HMQConf(ip, nPort, username, password, dicTrans);
                    dicHmq.Add(ip, hmq);
                }
            }

            return true;
        }

        private void btnSaveDisplayConf_Click(object sender, EventArgs e)
        {
            string strDisplay1 = textBoxDisplay1.Text;
            string strDisplay2 = textBoxDisplay2.Text;
            string strDisplay3 = textBoxDisplay3.Text;
            string strDisplay4 = textBoxDisplay4.Text;
            string strVideoWnd = textBoxVideoWnd.Text;
            string strEven = comboBoxEven.Text;
            string strWnd2 = comboBoxWnd2.Text;

            INIOperator.INIWriteValue(BaseDefine.CONFIG_FILE_PATH_DISPLAY, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_DISPLAY1, strDisplay1);
            INIOperator.INIWriteValue(BaseDefine.CONFIG_FILE_PATH_DISPLAY, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_DISPLAY2, strDisplay2);
            INIOperator.INIWriteValue(BaseDefine.CONFIG_FILE_PATH_DISPLAY, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_DISPLAY3, strDisplay3);
            INIOperator.INIWriteValue(BaseDefine.CONFIG_FILE_PATH_DISPLAY, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_DISPLAY4, strDisplay4);
            INIOperator.INIWriteValue(BaseDefine.CONFIG_FILE_PATH_DISPLAY, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_VIDEOWND, strVideoWnd);

            //是否隔行解码
            if (BaseDefine.STRING_EVEN_YES == strEven)
            {
                INIOperator.INIWriteValue(BaseDefine.CONFIG_FILE_PATH_DISPLAY, BaseDefine.CONFIG_SECTION_CONFIG,
                    BaseDefine.CONFIG_KEY_EVEN, "1");
            }
            else
            {
                INIOperator.INIWriteValue(BaseDefine.CONFIG_FILE_PATH_DISPLAY, BaseDefine.CONFIG_SECTION_CONFIG,
                    BaseDefine.CONFIG_KEY_EVEN, "0");
            }

            //项目动态切换
            if (BaseDefine.STRING_WND2_YES == strWnd2)
            {
                INIOperator.INIWriteValue(BaseDefine.CONFIG_FILE_PATH_DISPLAY, BaseDefine.CONFIG_SECTION_CONFIG,
                    BaseDefine.CONFIG_KEY_WND2, "1");
            }
            else
            {
                INIOperator.INIWriteValue(BaseDefine.CONFIG_FILE_PATH_DISPLAY, BaseDefine.CONFIG_SECTION_CONFIG,
                    BaseDefine.CONFIG_KEY_WND2, "0");
            }

            Log.GetLogger().InfoFormat("保存配置成功，display1={0}, display2={1}, display3={2}, display4={3}, videownd={4}, even={5}",
                strDisplay1, strDisplay2, strDisplay3, strDisplay4, strVideoWnd, strEven);
            MessageBox.Show("保存配置成功");
        }

        /// <summary>
        /// 导出模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportTemplate_Click(object sender, EventArgs e)
        {
            //if (string.IsNullOrEmpty(m_dbAddress) || string.IsNullOrEmpty(m_dbUsername) ||
            //    string.IsNullOrEmpty(m_dbPassword) || string.IsNullOrEmpty(m_dbInstance))
            //{
            //    MessageBox.Show("请先配置数据库连接。");
            //    return;
            //}

            labelState.Text = string.Empty;

            //选择目录
            string excelFilePath = string.Empty;
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            folderDlg.Description = @"请选择 Excel 模板存放目录";
            if (DialogResult.OK == folderDlg.ShowDialog())
            {
                excelFilePath = folderDlg.SelectedPath + BaseDefine.STRING_EXCEL_TEMPLATE;
            }
            if (File.Exists(excelFilePath))
            {
                File.Delete(excelFilePath);
            }

            //从配置文件读取合码器通道配置
            string errorMsg = string.Empty;
            Dictionary<string, HMQConf> dicHmq = new Dictionary<string, HMQConf>();
            if (!ReadHMQConfFromIni(out dicHmq, out errorMsg))
            {
                Log.GetLogger().InfoFormat("从配置文件读取合码器通道配置失败");
            }

            //从数据库读取摄像头配置
            int kkk = 0;

        }
    }
}
