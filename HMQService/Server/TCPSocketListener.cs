using System;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.IO;
using HMQService.Common;
using HMQService.Decode;
using System.Collections.Generic;

namespace HMQService.Server
{
	/// <summary>
	/// Summary description for TCPSocketListener.
	/// </summary>
	public class TCPSocketListener
	{	
		public enum STATE{FILE_NAME_READ, DATA_READ, FILE_CLOSED};

		/// <summary>
		/// Variables that are accessed by other classes indirectly.
		/// </summary>
		private Socket m_clientSocket = null;
		private bool m_stopClient=false;
		private Thread m_clientListenerThread=null;
		private bool m_markedForDeletion=false;

		/// <summary>
		/// Working Variables.
		/// </summary>
		private StringBuilder m_oneLineBuf=new StringBuilder();
		private STATE m_processState=STATE.FILE_NAME_READ;
		private long m_totalClientDataSize=0;
		private StreamWriter m_cfgFile=null;
		private DateTime m_lastReceiveDateTime;
		private DateTime m_currentReceiveDateTime;
        private Dictionary<int, CarManager> m_dicCars = new Dictionary<int, CarManager>();
		
		/// <summary>
		/// Client Socket Listener Constructor.
		/// </summary>
		/// <param name="clientSocket"></param>
		public TCPSocketListener(Socket clientSocket, Dictionary<int, CarManager> dicCars)
		{
            m_dicCars = dicCars;
            m_clientSocket = clientSocket;
		}

		/// <summary>
		/// Client SocketListener Destructor.
		/// </summary>
		~TCPSocketListener()
		{
			StopSocketListener();
		}

		/// <summary>
		/// Method that starts SocketListener Thread.
		/// </summary>
		public void StartSocketListener()
		{
			if (m_clientSocket!= null)
			{
				m_clientListenerThread = 
					new Thread(new ThreadStart(SocketListenerThreadStart));

				m_clientListenerThread.Start();
			}
		}

		/// <summary>
		/// Thread method that does the communication to the client. This 
		/// thread tries to receive from client and if client sends any data
		/// then parses it and again wait for the client data to come in a
		/// loop. The recieve is an indefinite time receive.
		/// </summary>
		private void SocketListenerThreadStart()
		{
			int size=0;
			Byte [] byteBuffer = new Byte[1024];
            DataHandler dataHandler = null;

            //m_lastReceiveDateTime = DateTime.Now;
            //m_currentReceiveDateTime = DateTime.Now;

            //Timer t= new Timer(new TimerCallback(CheckClientCommInterval),
            //	null,15000,15000);

            while (!m_stopClient)
			{
				try
				{
                    //���ճ�����Ϣ
					size = m_clientSocket.Receive(byteBuffer);
					m_currentReceiveDateTime=DateTime.Now;

                    //���ݴ���
                    dataHandler = new DataHandler(byteBuffer, size, m_dicCars);
                    dataHandler.StartHandle();

                    //����ȷ����Ϣ������
                    m_clientSocket.Send(byteBuffer);

                    Log.GetLogger().InfoFormat("����ȷ����Ϣ���������");
                }
				catch (Exception e)
				{
                    Log.GetLogger().ErrorFormat("SocketListenerThreadStart catch an error : {0}", e.Message);

                    m_stopClient =true;
					m_markedForDeletion=true;
				}
			}
			//t.Change(Timeout.Infinite, Timeout.Infinite);
			//t=null;
		}

		/// <summary>
		/// Method that stops Client SocketListening Thread.
		/// </summary>
		public void StopSocketListener()
		{
			if (m_clientSocket!= null)
			{
				m_stopClient=true;
				m_clientSocket.Close();

				// Wait for one second for the the thread to stop.
				m_clientListenerThread.Join(1000);
				
				// If still alive; Get rid of the thread.
				if (m_clientListenerThread.IsAlive)
				{
					m_clientListenerThread.Abort();
				}
				m_clientListenerThread=null;
				m_clientSocket=null;
				m_markedForDeletion=true;
			}
		}

		/// <summary>
		/// Method that returns the state of this object i.e. whether this
		/// object is marked for deletion or not.
		/// </summary>
		/// <returns></returns>
		public bool IsMarkedForDeletion()
		{
			return m_markedForDeletion;
		}
	}
}
