using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using HMQService.Common;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace HMQService.Decode
{
    /// <summary>
    /// 考试过程信息处理
    /// </summary>
    public class ExamProcedure
    {
        private int m_userId;
        private int m_kch;
        private int m_thirdPassiveHandle;
        private int m_fourthPassiveHandle;
        private Graphics gThirdPic;
        private Graphics gFourthPic;
        private Bitmap bmThirdPic;
        private Bitmap bmFourthPic;
        private Font font;
        private Brush brush;
        private Image imgTbk;
        private Image imgMark;
        private Image imgHgorbhg;
        private Image imgTime;
        private Image imgXmp;

        public ExamProcedure()
        {
            m_userId = -1;
            m_kch = -1;
            m_thirdPassiveHandle = -1;
            m_fourthPassiveHandle = -1;
            font = new Font("宋体", 16, FontStyle.Regular);
            brush = new SolidBrush(Color.FromArgb(255, 255, 255));

            imgTbk = Image.FromFile(BaseDefine.IMG_PATH_TBK);
            imgMark = Image.FromFile(BaseDefine.IMG_PATH_MARK);
            imgHgorbhg = Image.FromFile(BaseDefine.IMG_PATH_HGORBHG);
            imgTime = Image.FromFile(BaseDefine.IMG_PATH_TIME);
            imgXmp = Image.FromFile(BaseDefine.IMG_PATH_XMP);
        }

        ~ExamProcedure()
        { }

        public bool Init(int userId, int kch, int thirdPH, int fourthPH)
        {
            m_userId = userId;
            m_kch = kch;
            m_thirdPassiveHandle = thirdPH;
            m_fourthPassiveHandle = fourthPH;

            bmThirdPic = new Bitmap(imgTbk);
            bmFourthPic = new Bitmap(imgMark);
            gThirdPic = Graphics.FromImage(bmThirdPic);
            gFourthPic = Graphics.FromImage(bmFourthPic);

            string strInit = string.Format(BaseDefine.STRING_INIT_CAR, m_kch);
            gThirdPic.DrawString(strInit, font, brush, new Rectangle(0, 6, 350, 34));
            


            //临时
            SavePngFile(thirdPH);

            return true;
        }

        private bool SavePngFile(int lh)
        {
            bmThirdPic.Save(@"D:\image\thirdPic.png");
            bmFourthPic.Save(@"D:\image\fourthPic.png");

            BaseMethod.MakeAviFile(@"D:\image\third.avi", bmThirdPic);
            BaseMethod.MakeAviFile(@"D:\image\fourth.avi", bmFourthPic);

            //string exePath = @"D:\Users\Hqw\Documents\Work\北科舟宇\project\code\HMQService\HMQService\bin\Debug\mencoder.exe";
            //string strCmd = string.Format(" -ovc x264 -x264encopts bitrate=256 -vf scale=352:288 \"D:\\image\\third.avi\" -o \"D:\\image\third.yuv\"");
            //var process = Process.Start(exePath, strCmd);
            //process.WaitForExit(1000);

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Arguments = string.Format("-ovc x264 -x264encopts bitrate=256 -vf scale=352:288 \"{0}\" -o \"{1}\"",
                @"D:\image\third.avi",
                @"D:\image\third.yuv");
            startInfo.CreateNoWindow = true;
            startInfo.FileName = @".\mencoder.exe";
            var process = Process.Start(startInfo);
            process.WaitForExit(5000);

            //CHCNetSDK.NET_DVR_MatrixSendData()

            try
            {
                while(true)
                {
                    Byte[] bytes = File.ReadAllBytes(@"D:\image\third.yuv");
                    int len = bytes.Length;
                    Log.GetLogger().DebugFormat("len : {0}", len);
                    //将读取到的文件数据发送给解码器 
                    IntPtr pBuffer = Marshal.AllocHGlobal((Int32)len);
                    Marshal.Copy(bytes, 0, pBuffer, len);
                    if (!CHCNetSDK.NET_DVR_MatrixSendData((int)lh, pBuffer, (uint)len))
                    {
                        //发送失败 Failed to send data to the decoder
                        //数据发送失败，可以循环重新发送，避免数据丢失导致卡顿
                    }
                    Marshal.FreeHGlobal(pBuffer);
                }

            }
            catch(Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
            }

            return true;
        }

    }
}
