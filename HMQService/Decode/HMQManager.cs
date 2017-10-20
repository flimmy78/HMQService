using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HMQService.Common;
using HMQService.Database;
using HMQService.Model;
using System.Runtime.InteropServices;
using System.Data;

namespace HMQService.Decode
{
    public class HMQManager
    {
        private Thread m_HMQManagerThread = null;
        private bool m_bInitSDK = false;
        private int m_userId = -1;
        private uint m_iErrorCode = 0;
        private IDataProvider sqlDataProvider = null;
        private Dictionary<string, CameraConf> dicCameras = new Dictionary<string, CameraConf>();   //摄像头信息
        private Dictionary<string, JudgementRule> dicJudgementRule = new Dictionary<string, JudgementRule>();   //扣分规则
        private int[] m_dispalyShow;

        private CHCNetSDK.NET_DVR_DEC_STREAM_DEV_EX m_struStreamDev = new CHCNetSDK.NET_DVR_DEC_STREAM_DEV_EX();
        private CHCNetSDK.NET_DVR_MATRIX_ABILITY_V41 m_struDecAbility = new CHCNetSDK.NET_DVR_MATRIX_ABILITY_V41();

        public HMQManager()
        {
            m_dispalyShow = new int[5];
            
            //SDK初始化
            m_bInitSDK = CHCNetSDK.NET_DVR_Init();
            if (m_bInitSDK == false)
            {
                Log.GetLogger().ErrorFormat("SDK 初始化失败.");
                return;
            }
            else
            {
                //保存SDK日志
                CHCNetSDK.NET_DVR_SetLogToFile(3, "C:\\SdkLog\\", true);
            }
        }

        ~HMQManager()
        {
            StopWork();
        }

        public void StartWork()
        {
            m_HMQManagerThread =
                    new Thread(new ThreadStart(HMQManagerThreadProc));

            m_HMQManagerThread.Start();
        }

        public void StopWork()
        {
            m_HMQManagerThread.Join(1000);

            if (m_HMQManagerThread.IsAlive)
            {
                m_HMQManagerThread.Abort();
            }
            m_HMQManagerThread = null;

            if (null != sqlDataProvider)
            {
                sqlDataProvider.Dispose();
            }
        }

        private void HMQManagerThreadProc()
        {
            Log.GetLogger().InfoFormat("HMQManagerThreadProc begin.");

            //初始化数据库
            if (!InitDatabase())
            {
                return;
            }

            //获取摄像头配置信息
            if (!GetCameraConf())
            {
                return;
            }

            //获取扣分规则
            if (!GetJudgementRule())
            {
                return;
            }

            //初始化设备和画面信息
            if(!InitDevices())
            {
                return;
            }


            //开始动态解码
            if (!StartDynamicDecode())
            {
                return;
            }

            System.Threading.Thread.Sleep(30000);

            //停止动态解码
            if (!StopDynamicDecode())
            {
                return;
            }

            Log.GetLogger().InfoFormat("HMQManagerThreadProc end.");
        }

        private bool LoginDevice()
        {
            string DVRIPAddress = "192.168.0.68";
            Int16 DVRPortNumber = 8000;
            string DVRUserName = "admin";
            string DVRPassword = "hk12345678";

            try
            {
                CHCNetSDK.NET_DVR_DEVICEINFO_V30 m_struDeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();
                m_userId = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref m_struDeviceInfo);
                if (-1 == m_userId)
                {
                    m_iErrorCode = CHCNetSDK.NET_DVR_GetLastError();
                    Log.GetLogger().ErrorFormat("NET_DVR_Login_V30 failed, error code = {0}", m_iErrorCode);
                    return false;
                }
            }
            catch(Exception e)
            {
                Log.GetLogger().InfoFormat("LoginDevice catch an error : {0}", e.Message);
                return false;
            }

            Log.GetLogger().InfoFormat("LoginDevice return {0}", m_userId.ToString());
            return true;
        }

        private bool GetSDKBuildVersion()
        {
            try
            {
                uint version = CHCNetSDK.NET_DVR_GetSDKBuildVersion();
                Log.GetLogger().InfoFormat("sdk build version : {0}", version.ToString());
            }
            catch(Exception e)
            {
                Log.GetLogger().ErrorFormat("GetSDKBuildVersion catch an error : {0}", e.Message);
                return false;
            }

            return true;
        }

        private bool StartDynamicDecode()
        {
            CHCNetSDK.NET_DVR_PU_STREAM_CFG_V41 m_struStreamCfgV41 = new CHCNetSDK.NET_DVR_PU_STREAM_CFG_V41();
            m_struStreamCfgV41.dwSize = (uint)Marshal.SizeOf(m_struStreamCfgV41);

            m_struStreamCfgV41.byStreamMode = 1;    //取流模式
            m_struStreamDev.struDevChanInfo.byChanType = 0; //通道类型
            m_struStreamDev.struDevChanInfo.byChannel = 0;
            m_struStreamDev.struDevChanInfo.byTransProtocol = 0;
            m_struStreamDev.struDevChanInfo.byFactoryType = 0;

            m_struStreamDev.struDevChanInfo.byAddress = "192.168.0.131";
            m_struStreamDev.struDevChanInfo.wDVRPort = 8000;
            m_struStreamDev.struDevChanInfo.dwChannel = 3;  //通道号
            m_struStreamDev.struDevChanInfo.byTransMode = 0;    //传输码流模式
            m_struStreamDev.struDevChanInfo.sUserName = "admin";
            m_struStreamDev.struDevChanInfo.sPassword = "hk12345678";

            m_struStreamDev.struStreamMediaSvrCfg.byValid = 0;
            //m_struStreamDev.struStreamMediaSvrCfg.byValid = 1;
            //m_struStreamDev.struStreamMediaSvrCfg.wDevPort = 554;
            //m_struStreamDev.struStreamMediaSvrCfg.byAddress = "";   //流媒体IP
            //m_struStreamDev.struStreamMediaSvrCfg.byTransmitType = 0;

            uint dwUnionSize = (uint)Marshal.SizeOf(m_struStreamCfgV41.uDecStreamMode);
            IntPtr ptrStreamUnion = Marshal.AllocHGlobal((Int32)dwUnionSize);
            Marshal.StructureToPtr(m_struStreamDev, ptrStreamUnion, false);
            m_struStreamCfgV41.uDecStreamMode = (CHCNetSDK.NET_DVR_DEC_STREAM_MODE)Marshal.PtrToStructure(ptrStreamUnion, typeof(CHCNetSDK.NET_DVR_DEC_STREAM_MODE));
            Marshal.FreeHGlobal(ptrStreamUnion);

            if (!CHCNetSDK.NET_DVR_MatrixStartDynamic_V41(m_userId, 2, ref m_struStreamCfgV41))
            {
                m_iErrorCode = CHCNetSDK.NET_DVR_GetLastError();
                Log.GetLogger().ErrorFormat("NET_DVR_MatrixStartDynamic_V41 failed, error code = {0}", m_iErrorCode);
                return false;
            }

            Log.GetLogger().InfoFormat("NET_DVR_MatrixStartDynamic_V41 success");
            return true;
        }

        private bool StopDynamicDecode()
        {
            if (!CHCNetSDK.NET_DVR_MatrixStopDynamic(m_userId, 2))
            {
                m_iErrorCode = CHCNetSDK.NET_DVR_GetLastError();
                Log.GetLogger().ErrorFormat("NET_DVR_MatrixStopDynamic failed, error code = {0}", m_iErrorCode);
                return false;
            }

            Log.GetLogger().InfoFormat("NET_DVR_MatrixStopDynamic success");
            return true;
        }

        private bool InitDatabase()
        {
            //获取数据库连接串
            string connectString = GetConnectionString();
            if (string.IsNullOrEmpty(connectString))
            {
                Log.GetLogger().ErrorFormat("从配置文件获取数据库连接串失败");
                return false;
            }

            //获取数据库类型，并进行初始化连接
            int nRet = GetDatabaseType();
            if (0 == nRet)
            {
                sqlDataProvider = DataProvider.CreateDataProvider(DataProvider.DataProviderType.OracleDataProvider, connectString);
            }
            else if (1 == nRet)
            {
                sqlDataProvider = DataProvider.CreateDataProvider(DataProvider.DataProviderType.SqlDataProvider, connectString);
            }
            else
            {
                Log.GetLogger().ErrorFormat("数据库类型配置错误，SQLORORACLE = {0}", nRet.ToString());
                return false;
            }

            Log.GetLogger().InfoFormat("数据库连接成功");
            return true;
        }

        /// <summary>
        /// 读取配置文件里的数据库连接字符串
        /// 配置文件config.ini里的数据库连接字符串加过盐，这里解析时需要将每个字符“-1”后，再进行翻转
        /// </summary>
        /// <returns></returns>
        private string GetConnectionString()
        {
            string retConnectionStr = string.Empty;

            try
            {
                string configStr = INIOperator.INIGetStringValue(BaseDefine.CONFIG_FILE_PATH, BaseDefine.CONFIG_SECTION_SQLLINK,
                    BaseDefine.CONFIG_KEY_ServerPZ, string.Empty);
                if (!string.IsNullOrEmpty(configStr))
                {
                    foreach (char c in configStr.ToCharArray())
                    {
                        int nC = (int)c;
                        retConnectionStr = (char)(nC - 1) + retConnectionStr;
                    }
                }
            }
            catch(Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
            }
            Log.GetLogger().InfoFormat("connection string after decode : {0}", retConnectionStr);

            //配置文件里的数据库连接字符串包含"Provider=SQLOLEDB;"，这在C#代码中不适用，这里去除该字段
            if (0 == retConnectionStr.IndexOf("Provider=SQLOLEDB;"))
            {
                retConnectionStr = retConnectionStr.Substring(18);
            }
            Log.GetLogger().InfoFormat("connection string after delete : {0}", retConnectionStr);

            return retConnectionStr;
        }

        /// <summary>
        /// 根据配置文件判断数据库类型
        /// </summary>
        /// <returns>0--oracle，1--sqlserver</returns>
        private int GetDatabaseType()
        {
            int nRet = -1;

            try
            {
                string configStr = INIOperator.INIGetStringValue(BaseDefine.CONFIG_FILE_PATH, BaseDefine.CONFIG_SECTION_CONFIG,
                    BaseDefine.CONFIG_KEY_SQLORACLE, string.Empty);
                if (!string.IsNullOrEmpty(configStr))
                {
                    nRet = int.Parse(configStr);
                }
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
            }

            return nRet;
        }

        /// <summary>
        /// 获取摄像头配置
        /// </summary>
        /// <returns></returns>
        private bool GetCameraConf()
        {
            dicCameras.Clear();

            string sql = string.Format("select {0},{1},{2},{3},{4},{5},{6},{7},{8} from {9} order by {10}",
                BaseDefine.DB_FIELD_BH,
                BaseDefine.DB_FIELD_SBIP,
                BaseDefine.DB_FIELD_YHM,
                BaseDefine.DB_FIELD_MM,
                BaseDefine.DB_FIELD_DKH,
                BaseDefine.DB_FIELD_TDH,
                BaseDefine.DB_FIELD_TRANSMODE,
                BaseDefine.DB_FIELD_MEDIAIP,
                BaseDefine.DB_FIELD_NID,
                BaseDefine.DB_TABLE_TBKVIDEO,
                BaseDefine.DB_FIELD_BH);

            try
            {
                DataSet ds = sqlDataProvider.RetriveDataSet(sql);
                if (null != ds)
                {
                    for(int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string bh = (null == ds.Tables[0].Rows[i][0]) ? string.Empty : ds.Tables[0].Rows[i][0].ToString();
                        string sbip = (null == ds.Tables[0].Rows[i][1]) ? string.Empty : ds.Tables[0].Rows[i][1].ToString();
                        string yhm = (null == ds.Tables[0].Rows[i][2]) ? string.Empty : ds.Tables[0].Rows[i][2].ToString();
                        string mm = (null == ds.Tables[0].Rows[i][3]) ? string.Empty : ds.Tables[0].Rows[i][3].ToString();
                        string dkh = (null == ds.Tables[0].Rows[i][4]) ? string.Empty : ds.Tables[0].Rows[i][4].ToString();
                        string tdh = (null == ds.Tables[0].Rows[i][5]) ? string.Empty : ds.Tables[0].Rows[i][5].ToString();
                        string transmode = (null == ds.Tables[0].Rows[i][6]) ? string.Empty : ds.Tables[0].Rows[i][6].ToString();
                        string mediaip = (null == ds.Tables[0].Rows[i][7]) ? string.Empty : ds.Tables[0].Rows[i][7].ToString();
                        string nid = (null == ds.Tables[0].Rows[i][8]) ? string.Empty : ds.Tables[0].Rows[i][8].ToString();

                        int iDkh = string.IsNullOrEmpty(dkh) ? 8000 : int.Parse(dkh);   //端口号
                        int iTdh = string.IsNullOrEmpty(tdh) ? -1 : int.Parse(tdh); //通道号
                        int iTransmode = string.IsNullOrEmpty(transmode) ? 1 : int.Parse(transmode);   //码流类型

                        CameraConf camera = new CameraConf(bh, sbip, yhm, mm, mediaip, iDkh, iTdh, iTransmode);

                        string key = bh + "_" + nid;
                        if (!dicCameras.ContainsKey(key))
                        {
                            dicCameras.Add(key, camera);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
                return false;
            }

            if (0 == dicCameras.Count)
            {
                Log.GetLogger().ErrorFormat("初始化摄像头信息失败，数据库摄像头表解析结果为空");
                return false;
            }

            Log.GetLogger().InfoFormat("初始化摄像头信息成功");
            return true;
        }

        /// <summary>
        /// 获取扣分规则
        /// </summary>
        /// <returns></returns>
        private bool GetJudgementRule()
        {
            dicJudgementRule.Clear();

            string sql = string.Format("select {0},{1},{2} from {3}",
                BaseDefine.DB_FIELD_CWBH,
                BaseDefine.DB_FIELD_KFLX,
                BaseDefine.DB_FIELD_KCFS,
                BaseDefine.DB_TABLE_ERRORDATA);

            try
            {
                DataSet ds = sqlDataProvider.RetriveDataSet(sql);
                if (null != ds)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string cwbh = (null == ds.Tables[0].Rows[i][0]) ? string.Empty : ds.Tables[0].Rows[i][0].ToString();
                        string kflx = (null == ds.Tables[0].Rows[i][1]) ? string.Empty : ds.Tables[0].Rows[i][1].ToString();
                        string kcfs = (null == ds.Tables[0].Rows[i][2]) ? string.Empty : ds.Tables[0].Rows[i][2].ToString();

                        int iKcfs = string.IsNullOrEmpty(kcfs) ? -1 : int.Parse(kcfs); //扣除分数

                        JudgementRule jRule = new JudgementRule(kflx, iKcfs);

                        if (!string.IsNullOrEmpty(cwbh) && !dicJudgementRule.ContainsKey(cwbh))
                        {
                            dicJudgementRule.Add(cwbh, jRule);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
                return false;
            }

            if (0 == dicJudgementRule.Count)
            {
                Log.GetLogger().ErrorFormat("初始化扣分规则信息失败，数据库扣分规则表解析结果为空");
                return false;
            }

            Log.GetLogger().InfoFormat("初始化扣分规则信息成功");
            return true;
        }

        private bool InitDevices()
        {
            //获取 SDK Build
            if (!GetSDKBuildVersion())
            {
                return false;
            }

            //窗口初始化（看不懂是什么作用，这里先翻译C++代码）
            int kch = 0;
            int count = 4;
            string key = string.Empty;
            m_dispalyShow[4] = BaseMethod.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_VIDEOWND, 1);
            for (int i = 0; i < count; i++)
            {
                key = string.Format("{0}{1}", BaseDefine.CONFIG_KEY_DISPLAY, i + 1);
                m_dispalyShow[i] = BaseMethod.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH, BaseDefine.CONFIG_SECTION_CONFIG,
                    key, i);
                kch += m_dispalyShow[i];
            }
            if (kch != 6) 
            {
                for(int i = 0; i < count; i++)
                {
                    key = string.Format("{0}{1}", BaseDefine.CONFIG_KEY_DISPLAY, i + 1);
                    string value = i.ToString();
                    INIOperator.INIWriteValue(BaseDefine.CONFIG_FILE_PATH, BaseDefine.CONFIG_SECTION_CONFIG, key, value);
                    m_dispalyShow[i] = i;
                }
            }
            Log.GetLogger().InfoFormat("子窗口配置[%d,%d,%d,%d],音频窗口[%d]", m_dispalyShow[0], m_dispalyShow[1],
                m_dispalyShow[2], m_dispalyShow[3], m_dispalyShow[4]);

            //合码器初始化
            int nNum = BaseMethod.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH, BaseDefine.CONFIG_SECTION_JMQ,
                BaseDefine.CONFIG_KEY_NUM, 0);    //合码器数量
            int nEven = BaseMethod.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH, BaseDefine.CONFIG_SECTION_JMQ,
                BaseDefine.CONFIG_KEY_EVEN, 0);    //是否隔行合码
            int nKskm = BaseMethod.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_KSKM, 0);    //考试科目
            int nWnd2 = BaseMethod.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_WND2, 0);    //画面二状态
            if (0 == nNum)
            {
                Log.GetLogger().ErrorFormat("合码器数量为0，请检查配置文件");
                return false;
            }
            Log.GetLogger().InfoFormat("读取到 {0} 台合码器，EVEN = {1}, 科目{2}，画面2={3}", nNum, nEven, nKskm, nWnd2);

            string errorMsg = string.Empty;
            for (int i = 1; i <= nNum; i++)
            {
                string strConf = INIOperator.INIGetStringValue(BaseDefine.CONFIG_FILE_PATH, BaseDefine.CONFIG_SECTION_JMQ,
                    i.ToString(), string.Empty);
                string[] confArray = BaseMethod.SplitString(strConf, BaseDefine.SPLIT_CHAR_COMMA, out errorMsg);
                if (!string.IsNullOrEmpty(errorMsg) || confArray.Length != 4)
                {
                    Log.GetLogger().ErrorFormat("合码器配置错误，请检查配置文件。section = {0}, key = {1}", BaseDefine.CONFIG_SECTION_JMQ, i.ToString());
                    return false;
                }

                string ipAddress = confArray[0];    //合码器地址
                string user = confArray[1];     //用户名
                string password = confArray[2];     //密码
                string port = confArray[3];     //端口
                if (string.IsNullOrEmpty(ipAddress) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(port))
                {
                    Log.GetLogger().ErrorFormat("合码器配置错误，请检查配置文件。section = {0}, key = {1}", BaseDefine.CONFIG_SECTION_JMQ, i.ToString());
                    return false;
                }
                Log.GetLogger().InfoFormat("准备对合码器 {0} 进行初始化，ip={0}, port={1}, user={2}, password={3}", i, port, user, password);

                //初始化合码器
                if (!InitHMQ(ipAddress, user, password, port))
                {
                    return false;
                }

            }

            return true;
        }

        //初始化合码器
        private bool InitHMQ(string ip, string user, string pwd, string port)
        {
            try
            {
                //登录设备
                int nPort = string.IsNullOrEmpty(port) ? 8000 : int.Parse(port);
                CHCNetSDK.NET_DVR_DEVICEINFO_V30 m_struDeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();
                m_userId = CHCNetSDK.NET_DVR_Login_V30(ip, nPort, user, pwd, ref m_struDeviceInfo);
                if (-1 == m_userId)
                {
                    m_iErrorCode = CHCNetSDK.NET_DVR_GetLastError();
                    Log.GetLogger().ErrorFormat("NET_DVR_Login_V30 failed, error code = {0}", m_iErrorCode);
                    return false;
                }

                //获取设备能力集
                int nSize = Marshal.SizeOf(m_struDecAbility);
                IntPtr ptrDecAbility = Marshal.AllocHGlobal(nSize);
                Marshal.StructureToPtr(m_struDecAbility, ptrDecAbility, false);
                if (!CHCNetSDK.NET_DVR_GetDeviceAbility(m_userId, CHCNetSDK.MATRIXDECODER_ABILITY_V41, IntPtr.Zero, 0, ptrDecAbility, (uint)nSize))
                {
                    m_iErrorCode = CHCNetSDK.NET_DVR_GetLastError();
                    Log.GetLogger().ErrorFormat("Get MATRIXDECODER_ABILITY_V41 failed, error code = {0}", m_iErrorCode);
                    return false;
                }
                m_struDecAbility = (CHCNetSDK.NET_DVR_MATRIX_ABILITY_V41)Marshal.PtrToStructure(ptrDecAbility, 
                    typeof(CHCNetSDK.NET_DVR_MATRIX_ABILITY_V41));
                Log.GetLogger().InfoFormat("合码器ip={0}, DecN={1}, BncN={2}", ip, m_struDecAbility.byDecChanNums, 
                    m_struDecAbility.struBncInfo.byChanNums);
                
                ////设置设备自动重启
                //if (!CHCNetSDK.NET_DVR_GetDVRConfig(m_userId, CHCNetSDK., ))
            }
            catch (Exception e)
            {
                Log.GetLogger().InfoFormat("InitHMQ catch an error : {0}", e.Message);
                return false;
            }

            return true;
        }
    }
}
