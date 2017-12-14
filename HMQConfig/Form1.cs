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
        private string m_AppPath;
        private string m_confPathDisplay;
        private string m_confPathDB;
        private string m_confPathENV;
        private string m_confPathCar;

        public Form1()
        {
            m_dbAddress = string.Empty;
            m_dbUsername = string.Empty;
            m_dbPassword = string.Empty;
            m_dbInstance = string.Empty;
            m_dbType = -1;

            //初始化 log4net 配置信息
            log4net.Config.XmlConfigurator.Configure();

            //设置服务运行路径
            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
            m_AppPath = System.AppDomain.CurrentDomain.BaseDirectory;
            m_confPathCar = m_AppPath + BaseDefine.CONFIG_FILE_PATH_CAR;
            m_confPathDB = m_AppPath + BaseDefine.CONFIG_FILE_PATH_DB;
            m_confPathDisplay = m_AppPath + BaseDefine.CONFIG_FILE_PATH_DISPLAY;
            m_confPathENV = m_AppPath + BaseDefine.CONFIG_FILE_PATH_ENV;

            //初始化海康SDK
            HikUtils.HikUtils.InitDevice();

            InitializeComponent();

            //读取初始配置
            int nCarVideo = INIOperator.INIGetIntValue(m_confPathDisplay, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_DISPLAY1, 0);
            int nXmVideo = INIOperator.INIGetIntValue(m_confPathDisplay, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_DISPLAY2, 1);
            int nStudentInfo = INIOperator.INIGetIntValue(m_confPathDisplay, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_DISPLAY3, 2);
            int nExamInfo = INIOperator.INIGetIntValue(m_confPathDisplay, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_DISPLAY4, 3);
            int nAudioWnd = INIOperator.INIGetIntValue(m_confPathDisplay, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_VIDEOWND, 1);
            int nEven = INIOperator.INIGetIntValue(m_confPathDisplay, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_EVEN, 0);
            int nWnd2 = INIOperator.INIGetIntValue(m_confPathDisplay, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_WND2, 1);
            int nSleepTime = INIOperator.INIGetIntValue(m_confPathDisplay, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_SLEEP_TIME, 1000);

            //车内视频位置
            comboBoxCarVideo.BeginUpdate();
            comboBoxCarVideo.Items.Add(BaseDefine.STRING_WND_LEFT_TOP);
            comboBoxCarVideo.Items.Add(BaseDefine.STRING_WND_RIGHT_TOP);
            comboBoxCarVideo.Items.Add(BaseDefine.STRING_WND_LEFT_BOTTOM);
            comboBoxCarVideo.Items.Add(BaseDefine.STRING_WND_RIGHT_BOTTOM);
            comboBoxCarVideo.SelectedIndex = nCarVideo;
            comboBoxCarVideo.EndUpdate();
            //车外视频位置
            comboBoxXmVideo.BeginUpdate();
            comboBoxXmVideo.Items.Add(BaseDefine.STRING_WND_LEFT_TOP);
            comboBoxXmVideo.Items.Add(BaseDefine.STRING_WND_RIGHT_TOP);
            comboBoxXmVideo.Items.Add(BaseDefine.STRING_WND_LEFT_BOTTOM);
            comboBoxXmVideo.Items.Add(BaseDefine.STRING_WND_RIGHT_BOTTOM);
            comboBoxXmVideo.SelectedIndex = nXmVideo;
            comboBoxXmVideo.EndUpdate();
            //考生信息画面
            comboBoxStudentInfo.BeginUpdate();
            comboBoxStudentInfo.Items.Add(BaseDefine.STRING_WND_LEFT_TOP);
            comboBoxStudentInfo.Items.Add(BaseDefine.STRING_WND_RIGHT_TOP);
            comboBoxStudentInfo.Items.Add(BaseDefine.STRING_WND_LEFT_BOTTOM);
            comboBoxStudentInfo.Items.Add(BaseDefine.STRING_WND_RIGHT_BOTTOM);
            comboBoxStudentInfo.SelectedIndex = nStudentInfo;
            comboBoxStudentInfo.EndUpdate();
            //考试实时信息画面
            comboBoxExamInfo.BeginUpdate();
            comboBoxExamInfo.Items.Add(BaseDefine.STRING_WND_LEFT_TOP);
            comboBoxExamInfo.Items.Add(BaseDefine.STRING_WND_RIGHT_TOP);
            comboBoxExamInfo.Items.Add(BaseDefine.STRING_WND_LEFT_BOTTOM);
            comboBoxExamInfo.Items.Add(BaseDefine.STRING_WND_RIGHT_BOTTOM);
            comboBoxExamInfo.SelectedIndex = nExamInfo;
            comboBoxExamInfo.EndUpdate();

            //音频窗口位置
            comboBoxAudio.BeginUpdate();
            comboBoxAudio.Items.Add(BaseDefine.STRING_WND_LEFT_TOP);
            comboBoxAudio.Items.Add(BaseDefine.STRING_WND_RIGHT_TOP);
            comboBoxAudio.Items.Add(BaseDefine.STRING_WND_LEFT_BOTTOM);
            comboBoxAudio.Items.Add(BaseDefine.STRING_WND_RIGHT_BOTTOM);
            comboBoxAudio.SelectedIndex = nAudioWnd - 1;    //音频窗口index 从 1 开始计数
            comboBoxAudio.EndUpdate();

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

            //实时信息界面刷新间隔
            textBoxSleepTime.Text = nSleepTime.ToString();

        }

        private int GetWndIndexByDes(string des)
        {
            int nRet = 0;
            if (BaseDefine.STRING_WND_LEFT_TOP == des)
            {
                nRet = 0;
            }
            else if (BaseDefine.STRING_WND_RIGHT_TOP == des)
            {
                nRet = 1;
            }
            else if (BaseDefine.STRING_WND_LEFT_BOTTOM == des)
            {
                nRet = 2;
            }
            else if (BaseDefine.STRING_WND_RIGHT_BOTTOM == des)
            {
                nRet = 3;
            }

            return nRet;
        }

        private string GetWndDesByIndex(int index)
        {
            string retStr = BaseDefine.STRING_WND_LEFT_TOP;
            if (0 == index)
            {
                retStr = BaseDefine.STRING_WND_LEFT_TOP;
            }
            else if (1 == index)
            {
                retStr = BaseDefine.STRING_WND_RIGHT_TOP;
            }

            else if (2 == index)
            {
                retStr = BaseDefine.STRING_WND_LEFT_BOTTOM;
            }
            else if (3 == index)
            {
                retStr = BaseDefine.STRING_WND_RIGHT_BOTTOM;
            }

            return retStr;
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
            m_dbInstance = textDBInstance.Text;

            //禁用控件
            textDBIP.Enabled = false;
            textDBUsername.Enabled = false;
            textDBPassword.Enabled = false;
            textDBInstance.Enabled = false;

            int nCnt = 0;
            try
            {
                //连接数据库
                m_dbType = INIOperator.INIGetIntValue(m_confPathENV, BaseDefine.CONFIG_SECTION_CONFIG,
                    BaseDefine.CONFIG_KEY_SQLORACLE, 1);
                if (1 == m_dbType)
                {
                    Log.GetLogger().InfoFormat("数据库类型为：SqlServer");
                    string connStr = string.Format(BaseDefine.DB_CONN_FORMAT_SQL, m_dbAddress,
                        m_dbInstance, m_dbUsername, m_dbPassword);
                    m_dbProvider = DataProvider.CreateDataProvider(DataProvider.DataProviderType.SqlDataProvider, connStr);
                }
                else
                {
                    Log.GetLogger().InfoFormat("数据库类型为：Oracle");
                    string connStr = string.Format(BaseDefine.DB_CONN_FORMAT_ORACLE,
                         m_dbUsername, m_dbPassword, m_dbAddress, m_dbInstance);
                    m_dbProvider = DataProvider.CreateDataProvider(DataProvider.DataProviderType.OracleDataProvider, connStr);
                }

                //遍历数据库实例名
                string sql = "select 1 from errordata";
                DataSet ds = m_dbProvider.RetriveDataSet(sql);
                if (null != ds)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string strCnt = ds.Tables[0].Rows[i][0].ToString();
                        if (!string.IsNullOrEmpty(strCnt))
                        {
                            int.TryParse(strCnt, out nCnt);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", ex.Message);
            }

            if (nCnt <= 0)
            {
                MessageBox.Show("数据库连接失败，请检查数据库配置。");
                goto END;
            }
            else
            {
                //将数据库配置写入配置文件
                string base64DbAddress = Base64Util.Base64Encode(m_dbAddress);
                string base64DbUserName = Base64Util.Base64Encode(m_dbUsername);
                string base64DbPassword = Base64Util.Base64Encode(m_dbPassword);
                string base64DbInstance = Base64Util.Base64Encode(m_dbInstance);
                INIOperator.INIWriteValue(m_confPathDB, BaseDefine.CONFIG_SECTION_CONFIG,
                    BaseDefine.CONFIG_KEY_DBADDRESS, base64DbAddress);
                INIOperator.INIWriteValue(m_confPathDB, BaseDefine.CONFIG_SECTION_CONFIG,
                    BaseDefine.CONFIG_KEY_USERNAME, base64DbUserName);
                INIOperator.INIWriteValue(m_confPathDB, BaseDefine.CONFIG_SECTION_CONFIG,
                    BaseDefine.CONFIG_KEY_PASSWORD, base64DbPassword);
                INIOperator.INIWriteValue(m_confPathDB, BaseDefine.CONFIG_SECTION_CONFIG,
                    BaseDefine.CONFIG_KEY_INSTANCE, base64DbInstance);

                MessageBox.Show("数据库登录成功");
            }

        END:
            {
                //恢复控件
                textDBIP.Enabled = true;
                textDBUsername.Enabled = true;
                textDBPassword.Enabled = true;
                textDBInstance.Enabled = true;
            }
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
                        if (nCarNo <= 0 || nPort <= 0 || nTranNo <= 0 || string.IsNullOrEmpty(deviceIP) || string.IsNullOrEmpty(username)
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
            catch (Exception e)
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

            if (File.Exists(m_confPathCar))
            {
                File.Delete(m_confPathCar);
            }

            int nCount = dicHmq.Count;
            bool bRet = INIOperator.INIWriteValue(m_confPathCar, BaseDefine.CONFIG_SECTION_JMQ,
                BaseDefine.CONFIG_KEY_NUM, nCount.ToString());
            if (!bRet)
            {
                Log.GetLogger().ErrorFormat("WriteHMQConfToIni failed, section={0}, key={1},value={2}",
                    BaseDefine.CONFIG_SECTION_JMQ, BaseDefine.CONFIG_KEY_NUM, nCount);
            }

            int nIndex = 1;
            foreach (HMQConf hmq in dicHmq.Values)
            {
                string key = nIndex.ToString();
                string value = string.Format("{0},{1},{2},{3}", hmq.Ip, hmq.Username, hmq.Password, hmq.Port);
                bRet = INIOperator.INIWriteValue(m_confPathCar, BaseDefine.CONFIG_SECTION_JMQ, key, value);
                if (!bRet)
                {
                    Log.GetLogger().ErrorFormat("WriteHMQConfToIni failed, section={0}, key={1},value={2}",
                        BaseDefine.CONFIG_SECTION_JMQ, key, value);
                }

                string section = string.Format("{0}{1}", BaseDefine.CONFIG_SECTION_JMQ, nIndex);    //JMQ1、JMQ2
                foreach (int tranNo in hmq.DicTran2Car.Keys)
                {
                    int CarNo = hmq.DicTran2Car[tranNo];

                    key = string.Format("{0}{1}", BaseDefine.CONFIG_KEY_BNC, tranNo);   //BNC1、BNC2
                    bRet = INIOperator.INIWriteValue(m_confPathCar, section, key, CarNo.ToString());
                    if (!bRet)
                    {
                        Log.GetLogger().ErrorFormat("WriteHMQConfToIni failed, section={0}, key={1},value={2}, file={3}",
                            section, key, CarNo, BaseDefine.CONFIG_FILE_PATH_CAR);
                    }
                }

                nIndex++;
            }

            return true;
        }

        private bool WriteCameraConfToDB(Dictionary<string, CameraConf> dicCamera, out string errorMsg)
        {
            errorMsg = string.Empty;
            string connStr = string.Empty;
            IDataProvider sqlProvider = null;

            try
            {
                if (1 == m_dbType)
                {
                    connStr = string.Format(BaseDefine.DB_CONN_FORMAT_SQL, m_dbAddress,
                        m_dbInstance, m_dbUsername, m_dbPassword);
                    sqlProvider = DataProvider.CreateDataProvider(DataProvider.DataProviderType.SqlDataProvider, connStr);
                }
                else
                {
                    connStr = string.Format(BaseDefine.DB_CONN_FORMAT_ORACLE,
                         m_dbUsername, m_dbPassword, m_dbAddress, m_dbInstance);
                    sqlProvider = DataProvider.CreateDataProvider(DataProvider.DataProviderType.OracleDataProvider, connStr);
                }

                if (null == sqlProvider)
                {
                    errorMsg = string.Format("连接数据库失败，connStr={0}", connStr);
                    return false;
                }

                foreach (string key in dicCamera.Keys)
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
                        string sql = string.Format("delete from {0} where {1}='{2}' and {3}='{4}'", BaseDefine.DB_TABLE_TBKVIDEO,
                            BaseDefine.DB_FIELD_BH, bh, BaseDefine.DB_FIELD_NID, nid);
                        int nRet = sqlProvider.ExecuteNonQuery(sql);
                        if (nRet < 0)
                        {
                            Log.GetLogger().ErrorFormat("delete error，nRet = {0}, sql={1}", nRet, sql);
                        }

                        System.Threading.Thread.Sleep(10);

                        //插入新记录
                        sql = string.Format("insert into {0}({1},{2},{3},{4},{5},{6},{7},{8},{9},{10}) values('{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}')",
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
                    catch (Exception e)
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
            int nCount = INIOperator.INIGetIntValue(m_confPathCar, BaseDefine.CONFIG_SECTION_JMQ,
                BaseDefine.CONFIG_KEY_NUM, 0);
            if (0 == nCount)
            {
                Log.GetLogger().InfoFormat("读取到解码设备数量为0");
                return true;
            }

            //获取解码设备类型
            int nType = INIOperator.INIGetIntValue(m_confPathENV, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_HMQ, 1);

            for (int i = 1; i <= nCount; i++)
            {
                string hmqInfo = INIOperator.INIGetStringValue(m_confPathCar, BaseDefine.CONFIG_SECTION_JMQ,
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

                    int kch = INIOperator.INIGetIntValue(m_confPathCar, section, key, 0);
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

        private bool ReadCameraConfFromDB(out Dictionary<string, CameraConf> dicCamera, out string errorMsg)
        {
            dicCamera = new Dictionary<string, CameraConf>();
            errorMsg = string.Empty;

            IDataProvider sqlProvider = null;
            string connStr = string.Empty;
            try
            {
                if (1 == m_dbType)
                {
                    connStr = string.Format(BaseDefine.DB_CONN_FORMAT_SQL, m_dbAddress,
                        m_dbInstance, m_dbUsername, m_dbPassword);
                    sqlProvider = DataProvider.CreateDataProvider(DataProvider.DataProviderType.SqlDataProvider, connStr);
                }
                else
                {
                    connStr = string.Format(BaseDefine.DB_CONN_FORMAT_ORACLE,
                         m_dbUsername, m_dbPassword, m_dbAddress, m_dbInstance);
                    sqlProvider = DataProvider.CreateDataProvider(DataProvider.DataProviderType.OracleDataProvider, connStr);
                }

                if (null == sqlProvider)
                {
                    errorMsg = string.Format("连接数据库失败，connStr={0}", connStr);
                    return false;
                }

                string sql = string.Format("select {0},{1},{2},{3},{4},{5},{6},{7},{8},{9} from {10}",
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
                    BaseDefine.DB_TABLE_TBKVIDEO);
                DataSet ds = sqlProvider.RetriveDataSet(sql);
                if (null != ds)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow row = ds.Tables[0].Rows[i];

                        string bh = GetDataColumnStringValue(row, 0);
                        string sbip = GetDataColumnStringValue(row, 1);
                        int dkh = GetDataColumnIntValue(row, 2);
                        string yhm = GetDataColumnStringValue(row, 3);
                        string mm = GetDataColumnStringValue(row, 4);
                        int tdh = GetDataColumnIntValue(row, 5);
                        string bz = GetDataColumnStringValue(row, 6);
                        string nid = GetDataColumnStringValue(row, 7);
                        string mediaIp = GetDataColumnStringValue(row, 8);
                        int transmode = GetDataColumnIntValue(row, 9);
                        if (string.IsNullOrEmpty(bh) || string.IsNullOrEmpty(sbip) || string.IsNullOrEmpty(yhm) || string.IsNullOrEmpty(mm)
                            || string.IsNullOrEmpty(nid) || 0 == dkh || 0 == tdh)
                        {
                            Log.GetLogger().InfoFormat("数据库存在错误数据，bh={0}, nid={1}", bh, nid);
                            continue;
                        }

                        string key = bh + "_" + nid;
                        if (!dicCamera.ContainsKey(key))
                        {
                            CameraConf camera = new CameraConf(bh, sbip, yhm, mm, mediaIp, dkh, tdh, transmode, bz);
                            dicCamera.Add(key, camera);
                        }

                    }
                }

            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
                return false;
            }

            return true;
        }

        private string GetDataColumnStringValue(DataRow row, int index)
        {
            string retStr = string.Empty;
            if (null == row || index < 0)
            {
                return retStr;
            }

            try
            {
                retStr = row[index].ToString();
            }
            catch (Exception e)
            {
            }

            return retStr;
        }

        private int GetDataColumnIntValue(DataRow row, int index)
        {
            int nRet = 0;

            string strRet = GetDataColumnStringValue(row, index);
            try
            {
                nRet = string.IsNullOrEmpty(strRet) ? 0 : int.Parse(strRet);
            }
            catch (Exception e)
            { }

            return nRet;
        }

        private void btnSaveDisplayConf_Click(object sender, EventArgs e)
        {
            int nCarVideo = GetWndIndexByDes(comboBoxCarVideo.Text);
            int nXmVideo = GetWndIndexByDes(comboBoxXmVideo.Text);
            int nStudentInfo = GetWndIndexByDes(comboBoxStudentInfo.Text);
            int nExamInfo = GetWndIndexByDes(comboBoxExamInfo.Text);
            int nAudio = GetWndIndexByDes(comboBoxAudio.Text) + 1;      //音频窗口Index 从 1 开始
            string strEven = comboBoxEven.Text;
            string strWnd2 = comboBoxWnd2.Text;
            string strSleepTime = textBoxSleepTime.Text;

            INIOperator.INIWriteValue(m_confPathDisplay, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_DISPLAY1, nCarVideo.ToString());
            INIOperator.INIWriteValue(m_confPathDisplay, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_DISPLAY2, nXmVideo.ToString());
            INIOperator.INIWriteValue(m_confPathDisplay, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_DISPLAY3, nStudentInfo.ToString());
            INIOperator.INIWriteValue(m_confPathDisplay, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_DISPLAY4, nExamInfo.ToString());
            INIOperator.INIWriteValue(m_confPathDisplay, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_VIDEOWND, nAudio.ToString());
            INIOperator.INIWriteValue(m_confPathDisplay, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_SLEEP_TIME, strSleepTime);

            //是否隔行解码
            if (BaseDefine.STRING_EVEN_YES == strEven)
            {
                INIOperator.INIWriteValue(m_confPathDisplay, BaseDefine.CONFIG_SECTION_CONFIG,
                    BaseDefine.CONFIG_KEY_EVEN, "1");
            }
            else
            {
                INIOperator.INIWriteValue(m_confPathDisplay, BaseDefine.CONFIG_SECTION_CONFIG,
                    BaseDefine.CONFIG_KEY_EVEN, "0");
            }

            //项目动态切换
            if (BaseDefine.STRING_WND2_YES == strWnd2)
            {
                INIOperator.INIWriteValue(m_confPathDisplay, BaseDefine.CONFIG_SECTION_CONFIG,
                    BaseDefine.CONFIG_KEY_WND2, "1");
            }
            else
            {
                INIOperator.INIWriteValue(m_confPathDisplay, BaseDefine.CONFIG_SECTION_CONFIG,
                    BaseDefine.CONFIG_KEY_WND2, "0");
            }

            Log.GetLogger().InfoFormat("保存配置成功，display1={0}, display2={1}, display3={2}, display4={3}, videownd={4}, even={5}",
                nCarVideo, nXmVideo, nStudentInfo, nExamInfo, nAudio, strEven);
            MessageBox.Show("保存配置成功");
        }

        /// <summary>
        /// 导出模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportTemplate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(m_dbAddress) || string.IsNullOrEmpty(m_dbUsername) ||
                string.IsNullOrEmpty(m_dbPassword) || string.IsNullOrEmpty(m_dbInstance))
            {
                MessageBox.Show("请先配置数据库连接。");
                return;
            }

            labelState.Text = string.Empty;

            //选择目录
            string excelFilePath = string.Empty;
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            folderDlg.Description = @"请选择 Excel 模板存放目录";
            if (DialogResult.OK == folderDlg.ShowDialog())
            {
                excelFilePath = folderDlg.SelectedPath + @"\" + BaseDefine.STRING_EXCEL_TEMPLATE;
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
            Dictionary<string, CameraConf> dicCamera = new Dictionary<string, CameraConf>();
            if (!ReadCameraConfFromDB(out dicCamera, out errorMsg))
            {
                Log.GetLogger().InfoFormat("从数据库读取摄像头配置失败");
            }

            //生成 excel 模板
            if (!ExportExcelTemplate(excelFilePath, dicHmq, dicCamera))
            {
                Log.GetLogger().ErrorFormat("导出excel失败");
                MessageBox.Show("导出excel失败");
            }
            else
            {
                MessageBox.Show("导出excel模板成功");
            }

        }

        private bool ExportExcelTemplate(string filePath, Dictionary<string, HMQConf> dicHmq, Dictionary<string, CameraConf> dicCamera)
        {
            string errorMsg = string.Empty;

            try
            {
                XSSFWorkbook wk = new XSSFWorkbook();

                #region 创建模板(sheet页、第一行)
                if (!CreateExcelTemplate(wk))
                {
                    errorMsg = string.Format("创建sheet页、行失败。filePath={0}", filePath);
                    goto END;
                }
                #endregion

                #region 写入通道配置
                int rowNum = 1;
                string sheetName = BaseDefine.EXCEL_SHEET_NAME_CONF_TRANS;
                ISheet sheetTrans = wk.GetSheet(sheetName);
                if (null == sheetTrans)
                {
                    errorMsg = string.Format("读取名为 {0} 的 sheet 页失败，filePath={1}", sheetName, filePath);
                    goto END;
                }
                foreach (string key in dicHmq.Keys)
                {
                    HMQConf hmqConf = dicHmq[key];

                    string ip = hmqConf.Ip;
                    double port = (double)hmqConf.Port;
                    string user = hmqConf.Username;
                    string password = hmqConf.Password;
                    Dictionary<int, int> dicTrans = hmqConf.DicTran2Car;

                    foreach (int tranNo in dicTrans.Keys)
                    {
                        int kch = dicTrans[tranNo];

                        double dTran = (double)tranNo;
                        double dKch = (double)kch;

                        IRow row = sheetTrans.CreateRow(rowNum++);    //创建新行

                        CreateStringCell(row, 0, ip);
                        CreateNumbericCell(row, 1, port);
                        CreateStringCell(row, 2, user);
                        CreateStringCell(row, 3, password);
                        CreateNumbericCell(row, 4, dTran);
                        CreateNumbericCell(row, 5, dKch);
                    }
                }
                #endregion

                #region 写入摄像头配置
                int rowCar = 1;
                int rowXm = 1;
                string sheetCarName = BaseDefine.EXCEL_SHEET_NAME_CONF_CAMERA_CAR;
                string sheetXmName = BaseDefine.EXCEL_SHEET_NAME_CONF_CAMERA_XM;
                ISheet sheetCar = wk.GetSheet(sheetCarName);
                ISheet sheetXm = wk.GetSheet(sheetXmName);
                if (null == sheetCar)
                {
                    errorMsg = string.Format("读取名为 {0} 的 sheet 页失败，filePath={1}", sheetCarName, filePath);
                    goto END;
                }
                if (null == sheetXm)
                {
                    errorMsg = string.Format("读取名为 {0} 的 sheet 页失败，filePath={1}", sheetXmName, filePath);
                    goto END;
                }
                foreach (string key in dicCamera.Keys)
                {
                    string[] strArray = BaseMethod.SplitString(key, '_', out errorMsg);
                    if (null == strArray || strArray.Length != 2)
                    {
                        errorMsg = string.Format("数据库值存在异常，key={0}", key);
                        goto END;
                    }

                    try
                    {
                        CameraConf camera = dicCamera[key];
                        string bh = strArray[0];
                        string nid = strArray[1];

                        int nNid = string.IsNullOrEmpty(nid) ? 0 : int.Parse(nid);

                        if (bh.Contains("考车"))
                        {
                            IRow row = sheetCar.CreateRow(rowCar++);

                            bh = bh.Substring(2);
                            int nBh = string.IsNullOrEmpty(bh) ? 0 : int.Parse(bh);
                            string mllx = (0 == camera.Mllx) ? BaseDefine.STRING_BITSTREAM_MASTER : BaseDefine.STRING_BITSTREAM_SUB;

                            CreateNumbericCell(row, 0, (double)nBh);
                            CreateStringCell(row, 1, camera.CameraIP);
                            CreateStringCell(row, 2, camera.RasUser);
                            CreateStringCell(row, 3, camera.RasPassword);
                            CreateNumbericCell(row, 4, (double)camera.CameraPort);
                            CreateNumbericCell(row, 5, (double)camera.DwChannel);
                            CreateNumbericCell(row, 6, (double)nNid);
                            CreateStringCell(row, 7, mllx);
                            CreateStringCell(row, 8, camera.MediaIP);
                        }
                        else
                        {
                            IRow row = sheetXm.CreateRow(rowXm++);

                            int nBh = string.IsNullOrEmpty(bh) ? 0 : int.Parse(bh);
                            string mllx = (0 == camera.Mllx) ? BaseDefine.STRING_BITSTREAM_MASTER : BaseDefine.STRING_BITSTREAM_SUB;

                            CreateNumbericCell(row, 0, (double)nBh);
                            CreateStringCell(row, 1, camera.Bz);
                            CreateStringCell(row, 2, camera.CameraIP);
                            CreateStringCell(row, 3, camera.RasUser);
                            CreateStringCell(row, 4, camera.RasPassword);
                            CreateNumbericCell(row, 5, (double)camera.CameraPort);
                            CreateNumbericCell(row, 6, (double)camera.DwChannel);
                            CreateStringCell(row, 7, mllx);
                            CreateStringCell(row, 8, camera.MediaIP);
                        }


                    }
                    catch (Exception e)
                    {
                    }
                }
                #endregion

                using (FileStream fs = File.OpenWrite(filePath))
                {
                    wk.Write(fs);
                    fs.Close();
                }
            }
            catch (Exception e)
            {
                errorMsg = string.Format("catch an error : {0}", e.Message);
            }

        END:
            {
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    Log.GetLogger().ErrorFormat(errorMsg);
                    return false;
                }
            }

            return true;
        }

        private bool CreateExcelTemplate(XSSFWorkbook wk)
        {
            try
            {
                //通道配置
                string sheetName = BaseDefine.EXCEL_SHEET_NAME_CONF_TRANS;
                ISheet sheetTrans = wk.CreateSheet(sheetName);
                if (null != sheetTrans)
                {
                    IRow row = sheetTrans.CreateRow(0);

                    CreateStringCell(row, 0, "合码器/解码器IP");
                    CreateStringCell(row, 1, "合码器/解码器端口");
                    CreateStringCell(row, 2, "合码器/解码器用户名");
                    CreateStringCell(row, 3, "合码器/解码器密码");
                    CreateStringCell(row, 4, "合码器/解码器通道号");
                    CreateStringCell(row, 5, "对应考车号（阿拉伯数字）");
                }

                //车载摄像头
                sheetName = BaseDefine.EXCEL_SHEET_NAME_CONF_CAMERA_CAR;
                ISheet sheetCar = wk.CreateSheet(sheetName);
                if (null != sheetCar)
                {
                    IRow row = sheetCar.CreateRow(0);

                    CreateStringCell(row, 0, "考车号（阿拉伯数字）");
                    CreateStringCell(row, 1, "设备IP");
                    CreateStringCell(row, 2, "用户名");
                    CreateStringCell(row, 3, "密码");
                    CreateStringCell(row, 4, "端口号");
                    CreateStringCell(row, 5, "通道号");
                    CreateStringCell(row, 6, "摄像头编号");
                    CreateStringCell(row, 7, "码流类型（主码流/子码流）");
                    CreateStringCell(row, 8, "流媒体IP（可为空）");
                }

                //项目摄像头
                sheetName = BaseDefine.EXCEL_SHEET_NAME_CONF_CAMERA_XM;
                ISheet sheetXm = wk.CreateSheet(sheetName);
                if (null != sheetXm)
                {
                    IRow row = sheetXm.CreateRow(0);

                    CreateStringCell(row, 0, "项目编号");
                    CreateStringCell(row, 1, "项目名称");
                    CreateStringCell(row, 2, "设备IP");
                    CreateStringCell(row, 3, "用户名");
                    CreateStringCell(row, 4, "密码");
                    CreateStringCell(row, 5, "端口号");
                    CreateStringCell(row, 6, "通道号");
                    CreateStringCell(row, 7, "码流类型（主码流/子码流）");
                    CreateStringCell(row, 8, "流媒体IP（可为空）");
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        private bool CreateStringCell(IRow row, int cellIndex, string value)
        {
            if (null == row || cellIndex < 0)
            {
                return false;
            }

            try
            {
                ICell cell = row.CreateCell(cellIndex);
                cell.SetCellValue(value);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        private bool CreateNumbericCell(IRow row, int cellIndex, double value)
        {
            if (null == row || cellIndex < 0)
            {
                return false;
            }

            try
            {
                ICell cell = row.CreateCell(cellIndex);
                cell.SetCellValue(value);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        private void btnImportMap_Click(object sender, EventArgs e)
        {
            string mapFilePath = SelectMapFile();
            if (string.IsNullOrEmpty(mapFilePath))
            {
                return;
            }

            string fileMapConf = BaseDefine.CONFIG_FILE_PATH_MAP;
            string sectionConfig = BaseDefine.CONFIG_SECTION_CONFIG;
            string keyZoomIn = BaseDefine.CONFIG_KEY_ZOOMIN;
            int nZoomIn = BaseMethod.INIGetIntValue(fileMapConf, sectionConfig, keyZoomIn, BaseDefine.MAP_DEFAULT_ZOOMIN);

            List<PointF[]> listRegions = new List<PointF[]>();
            double maxX = 0;
            double maxY = 0;
            double minX = 0;
            double minY = 0;
            if (!ReadMapPoints(mapFilePath, ref maxX, ref minX, ref maxY, ref minY))
            {
                MessageBox.Show("从原始地图文件读取坐标范围失败");
                return;
            }

            maxX += 30;
            maxY += 30;
            minX -= 30;
            minY -= 30;
            if (!ReadRelativePoints(mapFilePath, maxX, minX, maxY, minY, nZoomIn, ref listRegions))
            {
                MessageBox.Show("从原始地图文件读取点坐标失败");
                return;
            }

            int width = (int)((maxX - minX) * nZoomIn);
            int height = (int)((maxY - minY) * nZoomIn);
            string keySplitWidth = BaseDefine.CONFIG_KEY_SPLITWIDTH;
            int splitWidth = BaseMethod.INIGetIntValue(fileMapConf, sectionConfig, keySplitWidth, BaseDefine.MAP_SPLIT_WIDTH);
            if (splitWidth <= 0)
            {
                splitWidth = BaseDefine.MAP_SPLIT_WIDTH;
            }
            if (!MakeSplitImage(width, height, splitWidth, splitWidth, listRegions))
            {
                MessageBox.Show("生成底图失败");
                return;
            }

            if (!WriteMapConfig(maxX, minX, maxY, minY, nZoomIn, splitWidth))
            {
                MessageBox.Show("写配置文件失败");
                return;
            }

            MessageBox.Show("导入地图成功");
        }

        private string SelectMapFile()
        {
            string retStr = string.Empty;
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = @"Map Files (*.cfg)|*.cfg";
            if (DialogResult.OK == fd.ShowDialog())
            {
                retStr = fd.FileName;
            }

            return retStr;
        }

        private bool ReadMapPoints(string mapFilePath, ref List<PointF[]> regions, ref float maxX, ref float maxY, ref float minX, ref float minY)
        {
            string line = string.Empty;
            string errorMsg = string.Empty;
            List<PointF> points = new List<PointF>();
            PointF firstPoint = new PointF();

            try
            {
                StreamReader file = new StreamReader(mapFilePath);
                while ((line = file.ReadLine()) != null)
                {
                    string[] strArray = BaseMethod.SplitString(line, ',', out errorMsg);
                    if (null == strArray)
                    {
                        continue;
                    }

                    if (3 == strArray.Length)
                    {
                        float x = 0;
                        float y = 0;
                        float.TryParse(strArray[1], out x);
                        float.TryParse(strArray[2], out y);
                        if (x <= 0.000001 || y <= 0.000001)
                        {
                            Log.GetLogger().ErrorFormat("地图文件中存在错误数据，line={0}", line);
                            continue;
                        }

                        if (maxX <= 0.000001)
                        {
                            maxX = x;
                        }
                        if (maxY <= 0.000001)
                        {
                            maxY = y;
                        }
                        if (minX <= 0.000001)
                        {
                            minX = x;
                        }
                        if (minY <= 0.000001)
                        {
                            minY = y;
                        }

                        if (x > maxX)
                        {
                            maxX = x;
                        }
                        if (y > maxY)
                        {
                            maxY = y;
                        }
                        if (x < minX)
                        {
                            minX = x;
                        }
                        if (y < minY)
                        {
                            minY = y;
                        }

                        PointF point = new PointF((float)x, (float)y);
                        if (0 == points.Count)
                        {
                            firstPoint = point;
                        }
                        points.Add(point);
                    }
                    else if (2 == strArray.Length)
                    {
                        if (points.Count > 0)
                        {
                            points.Add(firstPoint);
                            regions.Add(points.ToArray());

                            points.Clear();
                        }
                    }
                }

                if (null != points && points.Count > 0)
                {
                    points.Add(firstPoint);
                    regions.Add(points.ToArray());
                }

                file.Dispose();
                file.Close();
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
            }

            return true;
        }

        //读取地图源文件里面最大最小坐标位置
        private bool ReadMapPoints(string mapFilePath, ref double maxX, ref double minX, ref double maxY, ref double minY)
        {
            string line = string.Empty;
            string errorMsg = string.Empty;

            try
            {
                StreamReader file = new StreamReader(mapFilePath);
                while ((line = file.ReadLine()) != null)
                {
                    string[] strArray = BaseMethod.SplitString(line, ',', out errorMsg);
                    if (null == strArray)
                    {
                        continue;
                    }

                    if (3 == strArray.Length)
                    {
                        double x = 0;
                        double y = 0;
                        double.TryParse(strArray[1], out x);
                        double.TryParse(strArray[2], out y);
                        if (x <= 0.000001 || y <= 0.000001)
                        {
                            Log.GetLogger().ErrorFormat("地图文件中存在错误数据，line={0}", line);
                            continue;
                        }

                        if (maxX <= 0.000001)
                        {
                            maxX = x;
                        }
                        if (maxY <= 0.000001)
                        {
                            maxY = y;
                        }
                        if (minX <= 0.000001)
                        {
                            minX = x;
                        }
                        if (minY <= 0.000001)
                        {
                            minY = y;
                        }

                        if (x > maxX)
                        {
                            maxX = x;
                        }
                        if (y > maxY)
                        {
                            maxY = y;
                        }
                        if (x < minX)
                        {
                            minX = x;
                        }
                        if (y < minY)
                        {
                            minY = y;
                        }
                    }
                }

                file.Dispose();
                file.Close();
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
                return false;
            }

            if (maxX <= 0.000001 || minX <= 0.000001 || maxY <= 0.000001 || minY <= 0.000001)
            {
                Log.GetLogger().ErrorFormat("读取地图边界失败，file = {0}", mapFilePath);
                return false;
            }

            return true;
        }

        //读取相对坐标集合
        private bool ReadRelativePoints(string mapFilePath, double maxX, double minX, double maxY, double minY, int nZoomIn,
            ref List<PointF[]> regions)
        {
            string line = string.Empty;
            string errorMsg = string.Empty;
            List<PointF> points = new List<PointF>();
            PointF firstPoint = new PointF();

            try
            {
                StreamReader file = new StreamReader(mapFilePath);
                while ((line = file.ReadLine()) != null)
                {
                    string[] strArray = BaseMethod.SplitString(line, ',', out errorMsg);
                    if (null == strArray)
                    {
                        continue;
                    }

                    if (3 == strArray.Length)
                    {
                        double x = 0;
                        double y = 0;
                        double.TryParse(strArray[1], out x);
                        double.TryParse(strArray[2], out y);
                        if (x <= 0.000001 || y <= 0.000001)
                        {
                            Log.GetLogger().ErrorFormat("地图文件中存在错误数据，line={0}", line);
                            continue;
                        }

                        float pointX = (float)((x - minX) * nZoomIn);
                        float pointY = (float)((maxY - y) * nZoomIn);
                        PointF point = new PointF(pointX, pointY);
                        if (0 == points.Count)
                        {
                            firstPoint = point;
                        }
                        points.Add(point);
                    }
                    else if (2 == strArray.Length)
                    {
                        if (points.Count > 0)
                        {
                            points.Add(firstPoint);
                            regions.Add(points.ToArray());

                            points.Clear();
                        }
                    }
                }

                if (null != points && points.Count > 0)
                {
                    points.Add(firstPoint);
                    regions.Add(points.ToArray());
                }

                file.Dispose();
                file.Close();
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
            }

            return true;
        }

        private bool MakeSplitImage(int width, int height, int splitWidth, int splitHeight, List<PointF[]> regions)
        {
            int rowCount = height / splitHeight + 1;
            int columnCount = width / splitWidth + 1;

            string path = BaseDefine.MAP_IMAGE_PATH;
            if (!ReCreateDirectory(path))
            {
                return false;
            }

            for (int i = 0; i < rowCount; i ++)
            {
                for (int j = 0; j < columnCount; j ++)
                {
                    string fileName = path + @"\" + i + @"_" + j + @".png";

                    try
                    {
                        Bitmap bm = new Bitmap(splitWidth, splitHeight);
                        Graphics graphics = Graphics.FromImage(bm);
                        Pen pen = new Pen(Color.Gray, 1);
                        graphics.Clear(Color.White);

                        Rectangle rect = new Rectangle(j * splitWidth, i * splitHeight, splitWidth, splitHeight);
                        foreach (PointF[] region in regions)
                        {
                            bool bFirstPoint = true;
                            PointF firstPoint = new PointF();
                            PointF prePoint = new PointF();
                            foreach (PointF point in region)
                            {
                                float relativeX = point.X - j * splitWidth;   //相对X坐标
                                float relativeY = point.Y - i * splitHeight;   //相对Y坐标
                                if (bFirstPoint)
                                {
                                    firstPoint = new PointF(relativeX, relativeY);
                                    prePoint = firstPoint;
                                    bFirstPoint = false;
                                }
                                else
                                {
                                    PointF currentPoint = new PointF(relativeX, relativeY);
                                    graphics.DrawLine(pen, prePoint, currentPoint);
                                    prePoint = currentPoint;
                                }
                            }

                            if (!bFirstPoint)
                            {
                                graphics.DrawLine(pen, prePoint, firstPoint);
                            }
                        }

                        bm.Save(fileName);

                        graphics.Dispose();
                        bm.Dispose();
                        pen.Dispose();
                    }
                    catch(Exception e)
                    {
                        Log.GetLogger().ErrorFormat("生成地图背景时错误，file={0}, error={1}", fileName, e.Message);
                    }
                }
            }

            return true;
        }

        private bool ReCreateDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                if (Directory.Exists(path))
                {
                    Log.GetLogger().ErrorFormat("删除路径 {0} 失败，可能文件正在被使用", path);
                    return false;
                }
                Directory.CreateDirectory(path);
            }
            catch(Exception e)
            {
                Log.GetLogger().ErrorFormat("ReCreateDirectory catch an error : {0}", e.Message);
                return false;
            }

            return true;
        }

        private bool WriteMapConfig(double maxX, double minX, double maxY, double minY, int zoomIn, int splitWidth)
        {
            string fileMapConf = BaseDefine.CONFIG_FILE_PATH_MAP;
            string sectionConfig = BaseDefine.CONFIG_SECTION_CONFIG;
            string keyMaxX = BaseDefine.CONFIG_KEY_MAXX;
            string keyMinX = BaseDefine.CONFIG_KEY_MINX;
            string keyMaxY = BaseDefine.CONFIG_KEY_MAXY;
            string keyMinY = BaseDefine.CONFIG_KEY_MINY;
            string keyZoomIn = BaseDefine.CONFIG_KEY_ZOOMIN;
            string keySplitWidth = BaseDefine.CONFIG_KEY_SPLITWIDTH;
            string keySplitHeight = BaseDefine.CONFIG_KEY_SPLITHEIGHT;
            string keyXC = BaseDefine.CONFIG_KEY_XC;
            string keyYC = BaseDefine.CONFIG_KEY_YC;

            maxX = Math.Round(maxX);
            minX = Math.Round(minX);
            maxY = Math.Round(maxY);
            minY = Math.Round(minY);

            INIOperator.INIWriteValue(fileMapConf, sectionConfig, keyMaxX, ((int)maxX).ToString());
            INIOperator.INIWriteValue(fileMapConf, sectionConfig, keyMinX, ((int)minX).ToString());
            INIOperator.INIWriteValue(fileMapConf, sectionConfig, keyMaxY, ((int)maxY).ToString());
            INIOperator.INIWriteValue(fileMapConf, sectionConfig, keyMinY, ((int)minY).ToString());
            INIOperator.INIWriteValue(fileMapConf, sectionConfig, keyZoomIn, zoomIn.ToString());
            INIOperator.INIWriteValue(fileMapConf, sectionConfig, keySplitWidth, splitWidth.ToString());
            INIOperator.INIWriteValue(fileMapConf, sectionConfig, keyXC, "1");
            INIOperator.INIWriteValue(fileMapConf, sectionConfig, keyYC, "0");

            return true;
        }
    }
}
