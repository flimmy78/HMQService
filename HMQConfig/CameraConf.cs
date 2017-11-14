using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMQConfig
{
    /// <summary>
    /// 摄像头配置
    /// </summary>
    public struct CameraConf
    {
        private string cameraBH;    //编号
        private string cameraIP;    //设备IP
        private string rasUser; //用户名
        private string rasPassword; //密码
        private string mediaIP; //流媒体IP
        private int cameraPort; //端口号
        private int dwChannel;  //通道号
        private int mllx;   //码流类型  0--主码流，1--子码流
        private string bz;  //备注

        public CameraConf(string cameraBH, string cameraIP, string rasUser, string rasPassword, string mediaIP, int cameraPort,
            int dwChannel, int mllx, string bz)
        {
            this.cameraBH = cameraBH;
            this.cameraIP = cameraIP;
            this.rasUser = rasUser;
            this.rasPassword = rasPassword;
            this.mediaIP = mediaIP;
            this.cameraPort = cameraPort;
            this.dwChannel = dwChannel;
            this.mllx = mllx;
            this.bz = bz;
        }

        public string CameraBH
        {
            get { return cameraBH; }
            set { cameraBH = value; }
        }

        public string CameraIP
        {
            get { return cameraIP; }
            set { cameraIP = value; }
        }

        public string RasUser
        {
            get { return rasUser; }
            set { rasUser = value; }
        }

        public string RasPassword
        {
            get { return rasPassword; }
            set { rasPassword = value; }
        }

        public string MediaIP
        {
            get { return mediaIP; }
            set { mediaIP = value; }
        }

        public int CameraPort
        {
            get { return cameraPort; }
            set { cameraPort = value; }
        }

        public int DwChannel
        {
            get { return dwChannel; }
            set { dwChannel = value; }
        }

        public int Mllx
        {
            get { return mllx; }
            set { mllx = value; }
        }

        public string Bz
        {
            get { return bz; }
            set { bz = value; }
        }
    }
}
