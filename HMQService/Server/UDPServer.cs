using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using HMQService.Common;

namespace HMQService.Server
{
    public class UDPServer
    {
        public static IPAddress DEFAULT_SERVER = IPAddress.Parse("0.0.0.0");
        public static int DEFAULT_PORT = 10086; //监听端口号
        public static IPEndPoint DEFAULT_IP_END_POINT = new IPEndPoint(DEFAULT_SERVER, DEFAULT_PORT);

        private Socket m_server = null;
        private Thread m_serverThread = null;
        private bool m_stopServer = false;

        /// <summary>
        /// Constructors.
        /// </summary>
        public UDPServer()
        {
            Init(DEFAULT_IP_END_POINT);
        }

        public UDPServer(IPAddress serverIP)
        {
            Init(new IPEndPoint(serverIP, DEFAULT_PORT));
        }

        public UDPServer(int port)
        {
            Init(new IPEndPoint(DEFAULT_SERVER, port));
        }

        public UDPServer(IPAddress serverIP, int port)
        {
            Init(new IPEndPoint(serverIP, port));
        }

        public UDPServer(IPEndPoint ipNport)
        {
            Init(ipNport);
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

                    string recvStr = Encoding.ASCII.GetString(recvData);

                    Log.GetLogger().DebugFormat("receive data : {0}", recvStr);
                }
                catch (Exception e)
                {
                    Log.GetLogger().ErrorFormat("ServerThreadProc catch an error : {0}", e.Message);

                    m_stopServer = true;
                }
            }
        }

    }
}
