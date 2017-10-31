using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using HMQService.Common;
using HMQService.Model;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;


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
        private AutoResetEvent autoEventThird;  //自动重置事件
        private AutoResetEvent autoEventFourth; //自动重置事件
        private Thread m_thirdPicThread;
        private Thread m_fourthPicThread;
        private Byte[] m_thirdPicVideoBytes;
        private Byte[] m_fourthPicVideoBytes;
        private int m_thirdBytesLen;
        private int m_fourthBytesLen;

        public ExamProcedure()
        {
            m_userId = -1;
            m_kch = -1;
            m_thirdPassiveHandle = -1;
            m_fourthPassiveHandle = -1;
            font = new Font("宋体", 14, FontStyle.Regular);
            brush = new SolidBrush(Color.FromArgb(255, 255, 255));
            m_thirdPicVideoBytes = null;
            m_fourthPicVideoBytes = null;
            m_thirdBytesLen = 0;
            m_fourthBytesLen = 0;

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

            //初始化 ThirdPic
            string strInit = string.Format(BaseDefine.STRING_INIT_CAR, m_kch);
            try
            {
                Monitor.Enter(bmThirdPic);

                gThirdPic = Graphics.FromImage(bmThirdPic);
                gThirdPic.DrawString(strInit, font, brush, new Rectangle(0, 6, 350, 34));
            }
            catch(Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
            }
            finally
            {
                Monitor.Exit(bmThirdPic);
            }

            //初始化 FourthPic
            try
            {
                Monitor.Enter(bmFourthPic);

                gFourthPic = Graphics.FromImage(bmFourthPic);
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
            }
            finally
            {
                Monitor.Exit(bmFourthPic);
            }

            //开启 ThirdPic 刷新线程
            InitThirdPic(); 

            //开启 FourthPic 刷新线程
            InitFourthPic();

            return true;
        }

        public bool Handle17C52(StudentInfo studentInfo)
        {
            //更新考生信息画面
            try
            {
                //Monitor.Enter(bmThirdPic);
                autoEventThird.Reset();

                bmThirdPic = new Bitmap(imgTbk);
                gThirdPic = Graphics.FromImage(bmThirdPic);

                string carType = studentInfo.Kch + "-" + studentInfo.Bz + "-" + studentInfo.Kscx;
                string examReason = studentInfo.Ksy1 + "  " + studentInfo.KsyyDes;
                gThirdPic.DrawString(carType, font, brush, new Rectangle(0, 8, 350, 38));
                gThirdPic.DrawString(studentInfo.Xingming, font, brush, new Rectangle(58, 45, 350, 75));
                gThirdPic.DrawString(studentInfo.Xb, font, brush, new Rectangle(58, 80, 350, 110));
                gThirdPic.DrawString(studentInfo.Date, font, brush, new Rectangle(90, 115, 350, 145));
                gThirdPic.DrawString(studentInfo.Lsh, font, brush, new Rectangle(90, 150, 350, 180));
                gThirdPic.DrawString(studentInfo.Sfzmbh, font, brush, new Rectangle(90, 185, 350, 215));
                gThirdPic.DrawString(studentInfo.Jxmc, font, brush, new Rectangle(90, 220, 350, 250));
                gThirdPic.DrawString(examReason, font, brush, new Rectangle(90, 255, 350, 285));

                Stream streamZp = new MemoryStream(studentInfo.ArrayZp);
                Stream streamMjzp = new MemoryStream(studentInfo.ArrayMjzp);
                Image imgZp = Image.FromStream(streamZp);
                Image imgMjzp = Image.FromStream(streamMjzp);
                gThirdPic.DrawImage(imgZp, new Rectangle(242, 10, 100, 126));
                gThirdPic.DrawImage(imgMjzp, new Rectangle(272, 140, 80, 100));

                SendBitMapToHMQ(bmThirdPic, m_kch, m_thirdPassiveHandle, ref m_thirdPicVideoBytes, ref m_thirdBytesLen);
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
            }
            finally
            {
                //Monitor.Exit(bmThirdPic);
                autoEventThird.Set();
            }

            return true;
        }

        private bool InitThirdPic()
        {
            SendBitMapToHMQ(bmThirdPic, m_kch, m_thirdPassiveHandle, ref m_thirdPicVideoBytes, ref m_thirdBytesLen);

            m_thirdPicThread = new Thread(new ThreadStart(ThirdPicKeepThread));
            m_thirdPicThread.Start();

            return true;
        }

        private bool InitFourthPic()
        {
            SendBitMapToHMQ(bmFourthPic, m_kch, m_fourthPassiveHandle, ref m_fourthPicVideoBytes, ref m_fourthBytesLen);

            m_fourthPicThread = new Thread(new ThreadStart(FourthPicKeepThread));
            m_fourthPicThread.Start();

            return true;
        }

        private void ThirdPicKeepThread()
        {
            autoEventThird = new AutoResetEvent(true);  //自动重置事件，默认为已触发
            while (true)
            {
                autoEventThird.WaitOne(Timeout.Infinite);

                try
                {
                    Monitor.Enter(bmThirdPic);

                    MatrixSendData(m_thirdPicVideoBytes, m_thirdBytesLen, m_thirdPassiveHandle);
                }
                catch (Exception e)
                {
                    Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
                }
                finally
                {
                    Monitor.Exit(bmThirdPic);
                }
                
                System.Threading.Thread.Sleep(1000);

                autoEventThird.Set();   //触发事件
            }
        }

        private void FourthPicKeepThread()
        {
            autoEventFourth = new AutoResetEvent(true);  //自动重置事件，默认为已触发
            while (true)
            {
                autoEventFourth.WaitOne(Timeout.Infinite);

                try
                {
                    Monitor.Enter(m_fourthPicVideoBytes);

                    MatrixSendData(m_fourthPicVideoBytes, m_fourthBytesLen, m_fourthPassiveHandle);
                }
                catch (Exception e)
                {
                    Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
                }
                finally
                {
                    Monitor.Exit(m_fourthPicVideoBytes);
                }

                System.Threading.Thread.Sleep(1000);

                autoEventFourth.Set();   //触发事件
            }

        }

        private bool SendBitMapToHMQ(Bitmap bm, int kch, int passiveHandle, ref Byte[] videoBytes, ref int bytesLen)
        {
            bool bRet = false;

            string videoPath = @".\video";
            if (!Directory.Exists(videoPath))
            {
                Directory.CreateDirectory(videoPath);
            }
            string aviFilePath = string.Format(@"{0}\{1}_{2}.avi", videoPath, kch, passiveHandle);
            string yuvFilePath = string.Format(@"{0}\{1}_{2}.yuv", videoPath, kch, passiveHandle);

            if (BaseMethod.IsExistFile(aviFilePath))
            {
                BaseMethod.DeleteFile(aviFilePath);
            }
            if (BaseMethod.IsExistFile(yuvFilePath))
            {
                BaseMethod.DeleteFile(yuvFilePath);
            }

            //BitMap 转 AVI
            bRet = MakeAviFile(aviFilePath, bm);
            if (!bRet)
            {
                return bRet;
            }

            //AVI 转 264 码流
            bRet = MakeYuvFile(aviFilePath, yuvFilePath, ref videoBytes, ref bytesLen);
            if (!bRet)
            {
                return bRet;
            }

            //发送 H264 码流到合码器
            bRet = MatrixSendData(videoBytes, bytesLen, passiveHandle);

            //删除临时文件
            if (BaseMethod.IsExistFile(aviFilePath))
            {
                BaseMethod.DeleteFile(aviFilePath);
            }
            if (BaseMethod.IsExistFile(yuvFilePath))
            {
                BaseMethod.DeleteFile(yuvFilePath);
            }

            return bRet;
        }

        private bool MakeAviFile(string aviFilePath, Bitmap bm)
        {
            bool bRet = false;

            try
            {
                Monitor.Enter(bm);

                bRet = BaseMethod.MakeAviFile(aviFilePath, bm);
            }
            catch(Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
            }
            finally
            {
                Monitor.Exit(bm);
            }

            return bRet;
        }

        private bool MakeYuvFile(string aviFilePath, string yuvFilePath, ref Byte[] videoBytes, ref int len)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Arguments = string.Format("-ovc x264 -x264encopts bitrate=256 -vf scale=352:288 \"{0}\" -o \"{1}\"",
                aviFilePath,
                yuvFilePath);
            startInfo.CreateNoWindow = true;
            startInfo.FileName = @".\mencoder.exe";
            var process = Process.Start(startInfo);
            process.WaitForExit(5000);

            if (!BaseMethod.IsExistFile(yuvFilePath))
            {
                Log.GetLogger().ErrorFormat("生成yuv格式文件失败，fileName={0}", yuvFilePath);
                return false;
            }

            try
            {
                videoBytes = File.ReadAllBytes(yuvFilePath);
                len = videoBytes.Length;
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error :{0}", e.Message);
            }

            return true;
        }

        private bool MatrixSendData(Byte[] videoBytes, int bytesLen, int passiveHandle)
        {
            bool bRet = false;

            for (int i = 0; i < 1; i++)
            {
                try
                {
                    //将读取到的文件数据发送给解码器 
                    IntPtr pBuffer = Marshal.AllocHGlobal((Int32)bytesLen);
                    Marshal.Copy(videoBytes, 0, pBuffer, bytesLen);

                    bRet = CHCNetSDK.NET_DVR_MatrixSendData((int)passiveHandle, pBuffer, (uint)bytesLen);
                    if (bRet)
                    {
                        Marshal.FreeHGlobal(pBuffer);
                        break;
                    }

                    uint errorCode = CHCNetSDK.NET_DVR_GetLastError();
                    Log.GetLogger().ErrorFormat("NET_DVR_MatrixSendData failed, errorCode = {0}, 尝试次数 i = {1}", errorCode, i);
                    Marshal.FreeHGlobal(pBuffer);
                }
                catch (Exception e)
                {
                    Log.GetLogger().ErrorFormat("SendDataToHMQ catch an error : {0}, 尝试次数 i = {1}", e.Message, i);
                }
            }

            return bRet;
        }

        private bool SavePngFile(int lh)
        {
            bmThirdPic.Save(@"D:\image\thirdPic.png");
            bmFourthPic.Save(@"D:\image\fourthPic.png");

            //BaseMethod.MakeAviFile(@"D:\image\third.avi", bmThirdPic);
            //BaseMethod.MakeAviFile(@"D:\image\fourth.avi", bmFourthPic);

            ////string exePath = @"D:\Users\Hqw\Documents\Work\北科舟宇\project\code\HMQService\HMQService\bin\Debug\mencoder.exe";
            ////string strCmd = string.Format(" -ovc x264 -x264encopts bitrate=256 -vf scale=352:288 \"D:\\image\\third.avi\" -o \"D:\\image\third.yuv\"");
            ////var process = Process.Start(exePath, strCmd);
            ////process.WaitForExit(1000);

            //ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.Arguments = string.Format("-ovc x264 -x264encopts bitrate=256 -vf scale=352:288 \"{0}\" -o \"{1}\"",
            //    @"D:\image\third.avi",
            //    @"D:\image\third.yuv");
            //startInfo.CreateNoWindow = true;
            //startInfo.FileName = @".\mencoder.exe";
            //var process = Process.Start(startInfo);
            //process.WaitForExit(5000);

            ////CHCNetSDK.NET_DVR_MatrixSendData()

            //try
            //{
            //    while(true)
            //    {
            //        Byte[] bytes = File.ReadAllBytes(@"D:\image\third.yuv");
            //        int len = bytes.Length;
            //        Log.GetLogger().DebugFormat("len : {0}", len);
            //        //将读取到的文件数据发送给解码器 
            //        IntPtr pBuffer = Marshal.AllocHGlobal((Int32)len);
            //        Marshal.Copy(bytes, 0, pBuffer, len);
            //        if (!CHCNetSDK.NET_DVR_MatrixSendData((int)lh, pBuffer, (uint)len))
            //        {
            //            //发送失败 Failed to send data to the decoder
            //            //数据发送失败，可以循环重新发送，避免数据丢失导致卡顿
            //        }
            //        Marshal.FreeHGlobal(pBuffer);
            //    }

            //}
            //catch(Exception e)
            //{
            //    Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
            //}

            return true;
        }

    }
}
