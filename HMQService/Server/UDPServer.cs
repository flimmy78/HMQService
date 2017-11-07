using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using HMQService.Common;
using HMQService.Model;
using HMQService.Decode;
using System.Runtime.InteropServices;

namespace HMQService.Server
{
    public class UDPServer
    {
        private static IPAddress DEFAULT_SERVER = IPAddress.Parse(BaseDefine.LISTENING_ADDRESS);
        private static IPEndPoint DEFAULT_IP_END_POINT = new IPEndPoint(DEFAULT_SERVER, BaseDefine.LISTENING_PORT_UDP);

        private Socket m_server = null;
        private Thread m_serverThread = null;
        private bool m_stopServer = false;
        private Dictionary<int, CarManager> m_dicCars = new Dictionary<int, CarManager>();
        private Dictionary<int, ExamProcedure> m_dicExamProcedures = new Dictionary<int, ExamProcedure>();

        /// <summary>
        /// Constructors.
        /// </summary>
        public UDPServer(Dictionary<int, CarManager> dicCars, Dictionary<int, ExamProcedure> dicEP)
        {
            m_dicCars = dicCars;
            m_dicExamProcedures = dicEP;
            Init(DEFAULT_IP_END_POINT);
        }

        ~UDPServer()
        {
            StopServer();
        }

        private void Init(IPEndPoint ipNport)
        {
            try
            {
                m_server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                m_server.Bind(ipNport);
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("Init catch an error : {0}", e.Message);

                m_server = null;
            }
        }

        public void StartServer()
        {
            if (m_server != null)
            {
                m_serverThread = new Thread(new ThreadStart(ServerThreadProc));
                m_serverThread.Start();
            }
        }

        public void StopServer()
        {
            if (m_server != null)
            {
                m_stopServer = true;

                // Wait for one second for the the thread to stop.
                m_serverThread.Join(1000);

                // If still alive; Get rid of the thread.
                if (m_serverThread.IsAlive)
                {
                    m_serverThread.Abort();
                }
                m_serverThread = null;

                // Free Server Object.
                m_server.Close();
            }
        }

        private void ServerThreadProc()
        {
            int nRet = 0;
            Byte[] recvData = new Byte[1024];

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint remoteEP = (EndPoint)(sender);

            while (!m_stopServer)
            {
                try
                {
                    nRet = m_server.ReceiveFrom(recvData, ref remoteEP);
                    Log.GetLogger().DebugFormat("receive data from {0}", remoteEP.ToString());

                    Thread gpsThread = new Thread(new ParameterizedThreadStart(DealGpsDataProc));
                    gpsThread.Start(recvData);
                }
                catch (Exception e)
                {
                    Log.GetLogger().ErrorFormat("ServerThreadProc catch an error : {0}", e.Message);

                    m_stopServer = true;
                }
            }
        }

        private void DealGpsDataProc(object obj)
        {
            try
            {
                Byte[] data = (Byte[])obj;

                //1-4字节为包类型，默认传的是1，这里没有用到
                int type = System.BitConverter.ToInt32(data.Skip(0).Take(4).ToArray(), 0);

                //5-8字节为考车号
                int kch = System.BitConverter.ToInt32(data.Skip(4).Take(4).ToArray(), 0);
                if (kch <= 0)
                {
                    Log.GetLogger().ErrorFormat("udp 数据解析得到的考车号为 {0}", kch);
                    return;
                }
                if (!m_dicCars.ContainsKey(kch) || !m_dicExamProcedures.ContainsKey(kch))
                {
                    Log.GetLogger().ErrorFormat("不存在考车 {0}，请检查配置", kch);
                    return;
                }
                ExamProcedure examProcedure = m_dicExamProcedures[kch];

                //接下来的数据结构可以参考 GPSData 类
                //8个字节存放double类型的经度
                //8个字节存放double类型的纬度
                //4个字节存放float类型的方向角
                //4个字节存放float类型的速度
                //4个字节存放float类型的里程
                double longitude = System.BitConverter.ToDouble(data.Skip(8).Take(8).ToArray(), 0);
                double latitude = System.BitConverter.ToDouble(data.Skip(16).Take(8).ToArray(), 0);
                float directionAngle = System.BitConverter.ToSingle(data.Skip(24).Take(4).ToArray(), 0);
                float speed = System.BitConverter.ToSingle(data.Skip(28).Take(4).ToArray(), 0);
                float mileage = System.BitConverter.ToSingle(data.Skip(32).Take(4).ToArray(), 0);
                Log.GetLogger().DebugFormat("longitude={0}, latitude={1}, angle={2}, speed={3}, mileage={4}",
                    longitude, latitude, directionAngle, speed, mileage);

                GPSData gpsData = new GPSData(longitude, latitude, directionAngle, speed, mileage);

                if (!examProcedure.HandleGPS(gpsData))
                {
                    Log.GetLogger().ErrorFormat("examProcedure.HandleGPS failed, kch={0}", kch);
                }
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
            }

            Log.GetLogger().DebugFormat("DealGpsDataProc end");
        }

    }
}
