using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace TcpClient
{
    public partial class Form1 : Form
    {
        private string sendString;

        public Form1()
        {
            InitializeComponent();

            textBox_zkzmbh.Text = "650100197452";
            textBox_xmbh.Text = "301";
            textBox_cj.Text = "90";
            textBox_kfbh.Text = "41";
            textBox_kch.Text = "1";

            sendString = string.Empty;

            Thread sendThread = new Thread(new ThreadStart(SendThreadProc));
            sendThread.Start();
        }

        private void SendThreadProc()
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6708));

            while (true)
            {
                if (!string.IsNullOrEmpty(sendString))
                {
                    clientSocket.Send(Encoding.ASCII.GetBytes(sendString));

                    byte[] data = new byte[512];
                    int size = clientSocket.Receive(data);
                    string strRecv = Encoding.ASCII.GetString(data, 0, size);

                    if (sendString.CompareTo(strRecv) == 0)
                    {
                        sendString = string.Empty;
                    }
                    else
                    {
                        MessageBox.Show("接收到的返回数据不一致");
                        sendString = string.Empty;
                    }
                }

                Thread.Sleep(1000);
            }

        }

        private void btn_17C51_Click(object sender, EventArgs e)
        {
            string zkzmbh = textBox_zkzmbh.Text;
            string cj = textBox_cj.Text;
            string kch = textBox_kch.Text;
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //XMD*1*1*&17C51-1-4*17C51**510104432929*2017-11-03 11:17:11*90*##
            sendString = string.Format(@"XMD*1*1*&17C51-{0}-1*17C51**{1}*{2}*{3}*##", kch, zkzmbh, time, cj);
        }

        private void btn_17C52_Click(object sender, EventArgs e)
        {
            string zkzmbh = textBox_zkzmbh.Text;
            string cj = textBox_cj.Text;
            string kch = textBox_kch.Text;
            string xmbh = textBox_xmbh.Text;
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //XMD*1*2*&17C52-1-6*17C52*201*510104432929*2017-11-03 11:21:35*90*##
            sendString = string.Format(@"XMD*1*2*&17C52-{0}-1*17C52*{1}*{2}*{3}*{4}*##", kch, xmbh, zkzmbh, time, cj);
        }

        private void btn_17C53_Click(object sender, EventArgs e)
        {
            string zkzmbh = textBox_zkzmbh.Text;
            string cj = textBox_cj.Text;
            string kch = textBox_kch.Text;
            string xmbh = textBox_xmbh.Text;
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //XMD*1*3*&17C53-1-7*17C53*201@1*510104432929*2017-11-03 11:26:35*90*##
            sendString = string.Format(@"XMD*1*3*&17C53-{0}-1*17C53*{1}@1*{2}*{3}*{4}*##", kch, xmbh, zkzmbh, time, cj);
        }

        private void btn_17C55_Click(object sender, EventArgs e)
        {
            string zkzmbh = textBox_zkzmbh.Text;
            string cj = textBox_cj.Text;
            string kch = textBox_kch.Text;
            string xmbh = textBox_xmbh.Text;
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //XMD*1*5*&17C55-1-8*17C55*201*510104432929*2017-11-03 11:28:42*90*##
            sendString = string.Format(@"XMD*1*5*&17C55-{0}-1*17C55*{1}*{2}*{3}*{4}*##", kch, xmbh, zkzmbh, time, cj);
        }

        private void btn_17C56_Click(object sender, EventArgs e)
        {
            string zkzmbh = textBox_zkzmbh.Text;
            string cj = textBox_cj.Text;
            string kch = textBox_kch.Text;
            string xmbh = textBox_xmbh.Text;
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //XMD*1*6*&17C56-1-10*17C56*1*510104432929*2017-11-03 11:30:36*100*##
            sendString = string.Format(@"XMD*1*6*&17C56-{0}-1*17C56*{1}*{2}*{3}*{4}*##", kch, kch, zkzmbh, time, cj);
        }
    }
}
