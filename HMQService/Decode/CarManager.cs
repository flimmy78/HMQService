using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HMQService.Common;
using HMQService.Model;
using System.Runtime.InteropServices;

namespace HMQService.Decode
{
    public class CarManager
    {
        private int m_userId;
        private int m_kch; //考车号
        byte[] m_deChannel; //考车对应的解码通道号

        public CarManager()
        {
            m_deChannel = new byte[4];
            m_userId = -1;
            m_kch = -1;
        }

        ~CarManager()
        {
          
        }

        public void InitCar(int kch, int userid, Byte[] deChan)
        {
            m_userId = userid;
            m_kch = kch;

            m_deChannel[0] = deChan[0];
            m_deChannel[1] = deChan[1];
            m_deChannel[2] = deChan[2];
            m_deChannel[3] = deChan[3];

            Log.GetLogger().InfoFormat("初始化考车{0}, 解码通道={1}, {2}, {3}, {4}", kch, m_deChannel[0], m_deChannel[1], m_deChannel[2],
                m_deChannel[3]);
        }

        public bool StartDynamicDecode(CameraConf camera, int iWnd)
        {
            CHCNetSDK.NET_DVR_PU_STREAM_CFG_V41 struStreamCfgV41 = new CHCNetSDK.NET_DVR_PU_STREAM_CFG_V41();
            CHCNetSDK.NET_DVR_DEC_STREAM_DEV_EX struStreamDev = new CHCNetSDK.NET_DVR_DEC_STREAM_DEV_EX();

            struStreamCfgV41.dwSize = (uint)Marshal.SizeOf(struStreamCfgV41);
            struStreamCfgV41.byStreamMode = 1;    //取流模式
            struStreamDev.struDevChanInfo.byChanType = 0; //通道类型
            struStreamDev.struDevChanInfo.byChannel = 0;    //该参数无效，通道号见dwChannel 
            struStreamDev.struDevChanInfo.byTransProtocol = 0;  //传输协议类型0-TCP，1-UDP	
            struStreamDev.struDevChanInfo.byFactoryType = 0;    //前端设备厂家类型,通过接口获取

            struStreamDev.struDevChanInfo.byAddress = camera.CameraIP;
            struStreamDev.struDevChanInfo.wDVRPort = (ushort)camera.CameraPort;
            struStreamDev.struDevChanInfo.dwChannel = (uint)camera.DwChannel;  //通道号
            struStreamDev.struDevChanInfo.byTransMode = (byte)camera.Mllx;    //传输码流模式
            struStreamDev.struDevChanInfo.sUserName = camera.RasUser;
            struStreamDev.struDevChanInfo.sPassword = camera.RasPassword;

            if (!string.IsNullOrEmpty(camera.MediaIP))  //流媒体 IP 不为空
            {
                struStreamDev.struStreamMediaSvrCfg.byValid = 1;    //启用流媒体
                struStreamDev.struStreamMediaSvrCfg.wDevPort = 554; //端口
                struStreamDev.struStreamMediaSvrCfg.byTransmitType = 0; //TCP
                struStreamDev.struStreamMediaSvrCfg.byAddress = camera.MediaIP;
            }

            uint dwUnionSize = (uint)Marshal.SizeOf(struStreamCfgV41.uDecStreamMode);
            IntPtr ptrStreamUnion = Marshal.AllocHGlobal((Int32)dwUnionSize);
            Marshal.StructureToPtr(struStreamDev, ptrStreamUnion, false);
            struStreamCfgV41.uDecStreamMode = (CHCNetSDK.NET_DVR_DEC_STREAM_MODE)Marshal.PtrToStructure(ptrStreamUnion, typeof(CHCNetSDK.NET_DVR_DEC_STREAM_MODE));
            Marshal.FreeHGlobal(ptrStreamUnion);

            if (!CHCNetSDK.NET_DVR_MatrixStartDynamic_V41(m_userId, m_deChannel[iWnd], ref struStreamCfgV41))
            {
                uint errorCode = CHCNetSDK.NET_DVR_GetLastError();
                Log.GetLogger().ErrorFormat("NET_DVR_MatrixStartDynamic_V41 failed, error code = {0}", errorCode);
                return false;
            }

            Log.GetLogger().InfoFormat("开启动态解码，考车{0}，ecode={1}，子窗口={2}，mediaIP={3}，码流={4}（0主码流，1子码流），{5}_{6}", 
                m_kch, camera.CameraBH, m_deChannel[iWnd], camera.MediaIP, camera.Mllx, camera.CameraIP, camera.DwChannel);
            return true;
        }

        public bool StopDynamicDecode(int iWnd)
        {
            if (!CHCNetSDK.NET_DVR_MatrixStopDynamic(m_userId, m_deChannel[iWnd]))
            {
                uint errorCode = CHCNetSDK.NET_DVR_GetLastError();
                Log.GetLogger().ErrorFormat("NET_DVR_MatrixStopDynamic failed, error code = {0}", errorCode);
                return false;
            }

            Log.GetLogger().InfoFormat("停止动态解码成功，考车{0}，子窗口{1}", m_kch, m_deChannel[iWnd]);
            return true;
        }

        public bool StartPassiveDecode(int iWnd, ref int passiveHanle)
        {
            passiveHanle = -1;

            CHCNetSDK.NET_DVR_MATRIX_PASSIVEMODE passiveMode = new CHCNetSDK.NET_DVR_MATRIX_PASSIVEMODE();
            passiveMode.wTransProtol = 0;   //wTransProtol 	传输协议：0-TCP，1-UDP，2-MCAST 
            passiveMode.wPassivePort = 8000;    //TCP或者UDP端口，TCP时端口默认是8000，不同的解码通道UDP端口号需设置为不同的值
            passiveMode.byStreamType = 2;   //数据类型: 1-实时流, 2-文件流

            passiveHanle = CHCNetSDK.NET_DVR_MatrixStartPassiveDecode(m_userId, m_deChannel[iWnd], ref passiveMode);
            if (passiveHanle < 0)
            {
                uint errorCode = CHCNetSDK.NET_DVR_GetLastError();
                Log.GetLogger().ErrorFormat("启动被动解码失败，NET_DVR_MatrixStartPassiveDecode failed, error code = {0}, 解码通道={1}, 子窗口={2}, 考车={3}",
                    errorCode, m_deChannel[iWnd], iWnd, m_kch);
                return false;
            }

            Log.GetLogger().InfoFormat("开启被动解码，考车{0}，通道号{1}，子窗口{2}，passiveHandle={3}", m_kch, m_deChannel[iWnd],
                iWnd, passiveHanle);
            return true;
        }

    }
}
