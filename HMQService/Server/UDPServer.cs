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

        /// <summary>
        /// Constructors.
        /// </summary>
        public UDPServer(Dictionary<int, CarManager> dicCars)
        {
            m_dicCars = dicCars;
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
                if (!m_dicCars.ContainsKey(kch))
                {
                    Log.GetLogger().ErrorFormat("不存在考车 {0}，请检查配置", kch);
                    return;
                }

                //GPS 数据，转换为 IntPtr 传给 C++ Dll
                Byte[] gpsData = data.Skip(8).ToArray();
                IntPtr unmanagedPointer = Marshal.AllocHGlobal(gpsData.Length);
                Marshal.Copy(gpsData, 0, unmanagedPointer, gpsData.Length);


                BaseMethod.TF17C54(kch, unmanagedPointer);

                System.Threading.Thread.Sleep(1000);
                Marshal.FreeHGlobal(unmanagedPointer);
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
            }

            Log.GetLogger().DebugFormat("DealGpsDataProc end");
        }

    }
}
