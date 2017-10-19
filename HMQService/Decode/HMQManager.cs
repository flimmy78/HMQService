using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HMQService.Common;
using System.Runtime.InteropServices;

namespace HMQService.Decode
{
    public class HMQManager
    {
        private Thread m_HMQManagerThread = null;
        private bool m_bInitSDK = false;
        private int m_userId = -1;
        private uint m_iErrorCode = 0;

        private CHCNetSDK.NET_DVR_DEC_STREAM_DEV_EX m_struStreamDev = new CHCNetSDK.NET_DVR_DEC_STREAM_DEV_EX();

        public HMQManager()
        {
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
        }

        private void HMQManagerThreadProc()
        {
            Log.GetLogger().InfoFormat("HMQManagerThreadProc begin.");

            //获取 SDK Build
            if (!GetSDKBuildVersion())
            {
                return;
            }

            //登录设备
            if (!LoginDevice())
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
    }
}
