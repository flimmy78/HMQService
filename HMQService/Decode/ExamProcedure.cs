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
        private Font font;
        private Brush brush;
        private Brush brushBlack;
        private Image imgTbk;
        private Image imgMark;
        private Image imgHgorbhg;
        private Image imgTime;
        private Image imgXmp;
        private AutoResetEvent autoEventThird;  //自动重置事件
        private AutoResetEvent autoEventFourth; //自动重置事件
        private Thread m_thirdPicThread;
        private Thread m_fourthPicThread;

        private int m_lockThird;    //考生信息界面线程同步锁
        private int m_lockFourth;   //考试实时信息界面线程同步锁

        private string m_strCurrentState;   //考试阶段文字描述
        private int m_CurrentXmFlag;     //标识当前所处项目，用于绘制项目牌
        private DateTime m_startTime;   //考试开始时间
        private int m_CurrentScore; //考试成绩
        private Dictionary<int, string[]> m_dicErrorInfo;  //扣分信息
        private StudentInfo m_studentInfo;  //考生信息
        private bool m_bFinish; //考试结束标识 
        private bool m_bPass;   //考试是否合格

        public ExamProcedure()
        {
            m_userId = -1;
            m_kch = -1;
            m_thirdPassiveHandle = -1;
            m_fourthPassiveHandle = -1;
            font = new Font("宋体", 13, FontStyle.Regular);
            brush = new SolidBrush(Color.FromArgb(255, 255, 255));
            brushBlack = new SolidBrush(Color.FromArgb(0, 0, 0));

            imgTbk = Image.FromFile(BaseDefine.IMG_PATH_TBK);
            imgMark = Image.FromFile(BaseDefine.IMG_PATH_MARK);
            imgHgorbhg = Image.FromFile(BaseDefine.IMG_PATH_HGORBHG);
            imgTime = Image.FromFile(BaseDefine.IMG_PATH_TIME);
            imgXmp = Image.FromFile(BaseDefine.IMG_PATH_XMP);

            m_lockThird = 0;
            m_lockFourth = 0;
            m_strCurrentState = string.Empty;
            m_CurrentXmFlag = 0;
            m_CurrentScore = BaseDefine.CONFIG_VALUE_TOTAL_SCORE;
            m_dicErrorInfo = new Dictionary<int, string[]>();
            m_studentInfo = new StudentInfo();
            m_bFinish = false;
            m_bPass = false;
        }

        ~ExamProcedure()
        { }

        public bool Init(int userId, int kch, int thirdPH, int fourthPH)
        {
            m_userId = userId;
            m_kch = kch;
            m_thirdPassiveHandle = thirdPH;
            m_fourthPassiveHandle = fourthPH;

            //开启 ThirdPic 刷新线程
            InitThirdPic(); 

            //开启 FourthPic 刷新线程
            InitFourthPic();

            return true;
        }

        public bool Handle17C51(StudentInfo studentInfo)
        {
            //考试实时信息
            try
            {
                //Monitor.Enter(m_lockFourth);

                m_strCurrentState = "考试开始";
                m_CurrentXmFlag = 0;
                m_CurrentScore = BaseDefine.CONFIG_VALUE_TOTAL_SCORE;
                m_dicErrorInfo.Clear();
                m_startTime = DateTime.Now;
                m_bFinish = false;
                m_bPass = false;
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
            }
            finally
            {
                //Monitor.Exit(m_lockFourth);
            }

            //更新考生信息画面
            try
            {
                //Monitor.Enter(bmThirdPic);
                autoEventThird.Reset();

                m_studentInfo = studentInfo;
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

        /// <summary>
        /// 项目开始，绘制考生信息画面和实时信息画面
        /// </summary>
        /// <param name="xmCode">项目编号</param>
        /// <param name="xmlx">项目名称</param>
        /// <returns></returns>
        public bool Handle17C52(int xmCode, string xmlx)
        {
            //考试实时信息
            try
            {
                //Monitor.Enter(m_lockFourth);

                m_strCurrentState = xmlx;

                int xmFlag = GetXmFlag(xmCode, true);
                m_CurrentXmFlag |= xmFlag;
            }
            catch(Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}, xmlx = {1}", e.Message, xmlx);
            }
            finally
            {
                //Monitor.Exit(m_lockFourth);
            }
            
            return true;
        }

        /// <summary>
        /// 处理扣分 
        /// </summary>
        /// <param name="xmName">项目名称</param>
        /// <param name="kflx">扣分类型</param>
        /// <param name="kcfs">扣除分数</param>
        /// <returns></returns>
        public bool Handle17C53(string xmName, string kflx, int kcfs)
        {
            string[] errorInfo = new string[2];
            errorInfo[0] = string.Format("{0} 扣{1}分", xmName, kcfs);
            errorInfo[1] = kflx;

            //只存放3条扣分信息，超过3条时覆盖第3条
            int nIndex = m_dicErrorInfo.Count;
            if (nIndex > 2)
            {
                nIndex = 2;
            }
            m_dicErrorInfo[nIndex] = errorInfo;

            //扣除当前得分
            m_CurrentScore -= kcfs;
            if (BaseDefine.CONFIG_VALUE_ZERO_SCORE == m_CurrentScore)
            {
                m_CurrentScore = BaseDefine.CONFIG_VALUE_ZERO_SCORE;
            }

            return true;
        }

        /// <summary>
        /// 项目结束
        /// </summary>
        /// <param name="xmCode">项目编号</param>
        /// <param name="xmlx">项目类型</param>
        /// <returns></returns>
        public bool Handle17C55(int xmCode, string xmlx)
        {
            //考试实时信息
            try
            {
                //Monitor.Enter(m_lockFourth);

                m_strCurrentState = xmlx;

                Log.TempDebugFormat(string.Format("Handle17C55 in"));

                int xmFlag = GetXmFlag(xmCode, false);


                Log.TempDebugFormat(string.Format("Handle17C55 in，xmCode={0} ,xmFlag = {0}", xmCode, xmFlag));

                m_CurrentXmFlag |= xmFlag;
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}, xmlx = {1}", e.Message, xmlx);
            }
            finally
            {
                //Monitor.Exit(m_lockFourth);
            }

            return true;
        }

        /// <summary>
        /// 处理 17C56 信号，在考生信息界面绘制考试是否合格的标识
        /// </summary>
        /// <param name="bPass">考试是否合格，true-合格，false-不合格</param>
        /// <returns></returns>
        public bool Handle17C56(bool bPass)
        {
            //考试实时信息
            try
            {
                //Monitor.Enter(m_lockFourth);

                m_bFinish = true;   //考试结束

                if (bPass)
                {
                    m_strCurrentState = "考试合格";
                    m_bPass = true;
                }
                else
                {
                    m_strCurrentState = "考试不合格";
                    m_bPass = false;
                }
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}, bPass = {1}", e.Message, bPass);
            }
            finally
            {
                //Monitor.Exit(m_lockFourth);
            }

            //更新考生信息画面
            try
            {
                //Monitor.Enter(bmThirdPic);
                autoEventThird.Reset();

                m_bFinish = true;   //考试结束

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
            m_thirdPicThread = new Thread(new ThreadStart(ThirdPicKeepThread));
            m_thirdPicThread.Start();

            return true;
        }

        private bool InitFourthPic()
        {
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
                    //Monitor.Enter(bmThirdPic);

                    //重新初始化画板
                    Bitmap bm = new Bitmap(imgTbk);
                    Graphics graphics = Graphics.FromImage(bm);

                    //绘制考生信息
                    if (!string.IsNullOrEmpty(m_studentInfo.Sfzmbh))
                    {
                        string carType = m_studentInfo.Kch + "-" + m_studentInfo.Bz + "-" + m_studentInfo.Kscx;   //考车号-车牌号-驾照类型
                        string examReason = m_studentInfo.Ksy1 + " " + m_studentInfo.KsyyDes;  //考试员-考试原因
                        string sexAndCount = m_studentInfo.Xb + " 次数: " + m_studentInfo.Drcs;   //性别-考试次数
                        graphics.DrawString(carType, font, brush, new Rectangle(0, 8, 350, 38));
                        graphics.DrawString(m_studentInfo.Xingming, font, brush, new Rectangle(58, 45, 350, 75));
                        graphics.DrawString(sexAndCount, font, brush, new Rectangle(58, 80, 350, 110));
                        graphics.DrawString(m_studentInfo.Date, font, brush, new Rectangle(90, 115, 350, 145));
                        graphics.DrawString(m_studentInfo.Lsh, font, brush, new Rectangle(90, 150, 350, 180));
                        graphics.DrawString(m_studentInfo.Sfzmbh, font, brush, new Rectangle(90, 185, 350, 215));
                        graphics.DrawString(m_studentInfo.Jxmc, font, brush, new Rectangle(90, 220, 350, 250));
                        graphics.DrawString(examReason, font, brush, new Rectangle(90, 255, 350, 285));

                        Stream streamZp = new MemoryStream(m_studentInfo.ArrayZp);
                        Stream streamMjzp = new MemoryStream(m_studentInfo.ArrayMjzp);
                        Image imgZp = Image.FromStream(streamZp);
                        Image imgMjzp = Image.FromStream(streamMjzp);
                        graphics.DrawImage(imgZp, new Rectangle(242, 10, 100, 126));
                        graphics.DrawImage(imgMjzp, new Rectangle(272, 140, 80, 100));
                    }

                    if (m_bFinish)
                    {
                        //合格标识和不合格标识放在同一张图片里，这里需要对图片进行切割
                        Image imgResult = null;
                        Rectangle rect;
                        Bitmap originBitmap = new Bitmap(Image.FromFile(BaseDefine.IMG_PATH_HGORBHG));
                        if (m_bPass)
                        {
                            rect = new Rectangle(0, 0, originBitmap.Width / 2, originBitmap.Height);
                        }
                        else
                        {
                            rect = new Rectangle(originBitmap.Width / 2, 0, originBitmap.Width / 2, originBitmap.Height);
                        }
                        Bitmap bmp = new Bitmap(rect.Width, rect.Height);
                        using (Graphics gph = Graphics.FromImage(bmp))
                        {
                            gph.DrawImage(originBitmap, new Rectangle(0, 0, bmp.Width, bmp.Height), rect, GraphicsUnit.Pixel);
                        }
                        imgResult = (Image)bmp;

                        //绘制合格/不合格标识
                        graphics.DrawImage(imgResult, new Rectangle(100, 50, 135, 100));
                    }

                    //发送画面到合码器
                    SendBitMapToHMQ(bm, m_kch, m_thirdPassiveHandle);

                }
                catch (Exception e)
                {
                    Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
                }
                finally
                {
                    //Monitor.Exit(bmThirdPic);
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
                    //Monitor.Enter(m_lockFourth);
                    
                    //重新初始化画板
                    Bitmap bm = new Bitmap(imgMark);
                    Graphics graphics = Graphics.FromImage(bm);

                    //绘制项目牌列表
                    int nKskm = BaseMethod.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH, BaseDefine.CONFIG_SECTION_CONFIG,
                        BaseDefine.CONFIG_KEY_KSKM, 0);    //考试科目
                    if (BaseDefine.CONFIG_VALUE_KSKM_2 == nKskm)
                    {
                        graphics.DrawImage(imgXmp, new Rectangle(264, 36, 88, 252), 0, 0, 88, 252, GraphicsUnit.Pixel);
                    }
                    else
                    {
                        graphics.DrawImage(imgXmp, new Rectangle(264, 0, 88, 288), 0, 0, 88, 288, GraphicsUnit.Pixel);
                    }

                    //绘制项目状态
                    DrawXmState(nKskm, ref graphics);

                    //绘制实时状态信息
                    if (!string.IsNullOrEmpty(m_strCurrentState))
                    {
                        TimeSpan ts = DateTime.Now - m_startTime;
                        string strTotalTime = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        string strScore = string.Format(BaseDefine.STRING_EXAM_TIME_AND_SCORE, strTotalTime, m_CurrentScore);
                        string strSpeed = string.Format(BaseDefine.STRING_CAR_SPEED, 0.0);
                        string strStartTime = string.Format(BaseDefine.STRING_EXAM_START_TIME, m_startTime.ToString(BaseDefine.STRING_TIME_FORMAT));

                        graphics.DrawString(m_strCurrentState, font, brush, new Rectangle(4, 10, 348, 40));
                        graphics.DrawString(strScore, font, brush, new Rectangle(4, 40, 263, 65));
                        graphics.DrawString(strSpeed, font, brush, new Rectangle(4, 65, 263, 90));
                        graphics.DrawString(strStartTime, font, brush, new Rectangle(4, 90, 263, 115));
                    }

                    //绘制扣分信息
                    foreach (int index in m_dicErrorInfo.Keys)
                    {
                        string[] errorInfo = m_dicErrorInfo[index];
                        if (null == errorInfo)
                        {
                            continue;
                        }

                        if (0 == index)
                        {
                            graphics.DrawString(errorInfo[0], font, brushBlack, new Rectangle(2, 120, 260, 145));
                            graphics.DrawString(errorInfo[1], font, brushBlack, new Rectangle(2, 145, 260, 170));
                        }
                        else if (1 == index)
                        {
                            graphics.DrawString(errorInfo[0], font, brushBlack, new Rectangle(2, 180, 260, 205));
                            graphics.DrawString(errorInfo[1], font, brushBlack, new Rectangle(2, 205, 260, 230));
                        }
                        else if (2 == index)
                        {
                            graphics.DrawString(errorInfo[0], font, brushBlack, new Rectangle(2, 240, 260, 265));
                            graphics.DrawString(errorInfo[1], font, brushBlack, new Rectangle(2, 265, 260, 288));
                        }
                    }

                    //发送画面到合码器
                    SendBitMapToHMQ(bm, m_kch, m_fourthPassiveHandle);
                }
                catch (Exception e)
                {
                    Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
                }
                finally
                {
                    //Monitor.Exit(m_lockFourth);
                }

                System.Threading.Thread.Sleep(1000);

                autoEventFourth.Set();   //触发事件
            }

        }

        /// <summary>
        /// 绘制项目牌实时状态
        /// </summary>
        /// <param name="nKskm">考试科目，2--科目2，3--科目3</param>
        /// <param name="g"></param>
        private void DrawXmState(int nKskm, ref Graphics g)
        {
            if (BaseDefine.CONFIG_VALUE_KSKM_2 == nKskm)
            {
                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_DCRK) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 36, 88, 36), 176, 0, 88, 36, GraphicsUnit.Pixel); //结束倒车入库
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_DCRK) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 36, 88, 36), 88, 0, 88, 36, GraphicsUnit.Pixel); //进入倒车入库
                }

                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_CFTC) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 72, 88, 36), 176, 36, 88, 36, GraphicsUnit.Pixel); //结束侧方停车
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_CFTC) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 72, 88, 36), 88, 36, 88, 36, GraphicsUnit.Pixel); //进入侧方停车
                }

                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_DDPQ) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 108, 88, 36), 176, 72, 88, 36, GraphicsUnit.Pixel); //结束定点坡起
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_DDPQ) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 108, 88, 36), 88, 72, 88, 36, GraphicsUnit.Pixel); //开始定点坡起
                }

                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_QXXS) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 144, 88, 36), 176, 108, 88, 36, GraphicsUnit.Pixel); //结束曲线行驶
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_QXXS) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 144, 88, 36), 88, 108, 88, 36, GraphicsUnit.Pixel); //开始曲线行驶
                }

                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_ZJZW) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 180, 88, 36), 176, 144, 88, 36, GraphicsUnit.Pixel); //结束直角转弯
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_ZJZW) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 180, 88, 36), 88, 144, 88, 36, GraphicsUnit.Pixel); //开始直角转弯
                }

                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_YWSH) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 216, 88, 36), 176, 180, 88, 36, GraphicsUnit.Pixel); //结束雨雾湿滑
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_YWSH) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 216, 88, 36), 88, 180, 88, 36, GraphicsUnit.Pixel); //开始雨雾湿滑
                }

                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_MNSD) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 252, 88, 36), 176, 216, 88, 36, GraphicsUnit.Pixel); //结束雨雾湿滑
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_MNSD) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 252, 88, 36), 88, 216, 88, 36, GraphicsUnit.Pixel); //开始雨雾湿滑
                }
            }
            else
            {

            }
        }

        private bool SendBitMapToHMQ(Bitmap bm, int kch, int passiveHandle)
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
            Byte[] videoBytes = null;
            int bytesLen = 0;
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
            //bmThirdPic.Save(@"D:\image\thirdPic.png");
            //bmFourthPic.Save(@"D:\image\fourthPic.png");

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

        /// <summary>
        /// 获取当前项目阶段对应的标识，用于绘制项目牌
        /// </summary>
        /// <param name="xmCode">项目编号</param>
        /// <param name="bStart">true--项目开始，false--项目结束</param>
        /// <returns></returns>
        private int GetXmFlag(int xmCode, bool bStart)
        {
            int nRet = 0;

            //当前科目二的项目编号为6位数字，如 201510，前3位201表示“倒车入库项目”，后3位表示不同的车库
            //只需要前3位即可判断具体的项目
            int xmType = xmCode / 1000;

            if (bStart)
            {
                if (BaseDefine.XMBH_201 == xmType)
                {
                    nRet = BaseDefine.EXAM_STATE_START_DCRK;
                }
                else if (BaseDefine.XMBH_204 == xmType)
                {
                    nRet = BaseDefine.EXAM_STATE_START_CFTC;
                }
                else if (BaseDefine.XMBH_203 == xmType)
                {
                    nRet = BaseDefine.EXAM_STATE_START_DDPQ;
                }
                else if (BaseDefine.XMBH_206 == xmType)
                {
                    nRet = BaseDefine.EXAM_STATE_START_QXXS;
                }
                else if (BaseDefine.XMBH_207 == xmType)
                {
                    nRet = BaseDefine.EXAM_STATE_START_ZJZW;
                }
                else if (BaseDefine.XMBH_214 == xmType)
                {
                    nRet = BaseDefine.EXAM_STATE_START_MNSD;
                }
                else if (BaseDefine.XMBH_215 == xmType)
                {
                    nRet = BaseDefine.EXAM_STATE_START_YWSH;
                }
                else if (BaseDefine.XMBH_216 == xmType)
                {
                    nRet = BaseDefine.EXAM_STATE_START_YWSH;
                }
            }
            else
            {
                if (BaseDefine.XMBH_201 == xmType)
                {
                    nRet = BaseDefine.EXAM_STATE_END_DCRK;
                }
                else if (BaseDefine.XMBH_204 == xmType)
                {
                    nRet = BaseDefine.EXAM_STATE_END_CFTC;
                }
                else if (BaseDefine.XMBH_203 == xmType)
                {
                    nRet = BaseDefine.EXAM_STATE_END_DDPQ;
                }
                else if (BaseDefine.XMBH_206 == xmType)
                {
                    nRet = BaseDefine.EXAM_STATE_END_QXXS;
                }
                else if (BaseDefine.XMBH_207 == xmType)
                {
                    nRet = BaseDefine.EXAM_STATE_END_ZJZW;
                }
                else if (BaseDefine.XMBH_214 == xmType)
                {
                    nRet = BaseDefine.EXAM_STATE_END_MNSD;
                }
                else if (BaseDefine.XMBH_215 == xmType)
                {
                    nRet = BaseDefine.EXAM_STATE_END_YWSH;
                }
                else if (BaseDefine.XMBH_216 == xmType)
                {
                    nRet = BaseDefine.EXAM_STATE_END_YWSH;
                }
            }

            return nRet;
        }

    }
}
