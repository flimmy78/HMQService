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
        private Image imgMap;
        private Image imgCar;
        private AutoResetEvent autoEventThird;  //自动重置事件，考生信息刷新线程
        private AutoResetEvent autoEventFourthInfo; //自动重置事件，考试实时信息刷新线程
        private AutoResetEvent autoEventFourthMap; //自动重置事件，地图刷新线程
        private Thread m_thirdPicThread;
        private Thread m_fourthPicThread;

        private static readonly object m_lockThird = new object();    //考生信息界面线程同步锁
        private static readonly object m_lockFourth = new object();   //考试实时信息界面线程同步锁
        private static readonly object m_lockCommon = new object(); //三四画面通用线程同步锁

        private int m_kskm; //考试科目
        private string m_strCurrentState;   //考试阶段文字描述
        private uint m_CurrentXmFlag;     //标识当前所处项目，用于绘制项目牌
        private DateTime m_startTime;   //考试开始时间
        private DateTime m_endTime; //考试结束时间
        private int m_CurrentScore; //考试成绩
        private Dictionary<int, string[]> m_dicErrorInfo;  //扣分信息
        private StudentInfo m_studentInfo;  //考生信息
        private bool m_bFinish; //考试结束标识 
        private bool m_bPass;   //考试是否合格
        private GPSData m_gpsData;  //车载GPS实时数据
        private bool m_bDrawMap;    //是否绘制地图
        private bool m_bDrawCar;    //是否绘制车模型
        private int m_mapWidth;    
        private int m_mapHeight;
        private int m_carX; //考车所在位置
        private int m_carY; //考车所在位置
        private double m_mapX;
        private double m_mapY;
        private double m_zoomIn;
        private int m_mapPy;    //地图飘移

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

            m_kskm = 0;
            m_strCurrentState = string.Empty;
            m_CurrentXmFlag = 0;
            m_CurrentScore = BaseDefine.CONFIG_VALUE_TOTAL_SCORE;
            m_dicErrorInfo = new Dictionary<int, string[]>();
            m_studentInfo = new StudentInfo();
            m_bFinish = false;
            m_bPass = false;
            m_gpsData = new GPSData();
            m_bDrawMap = false;
            m_bDrawCar = false;
            m_mapHeight = 0;
            m_mapWidth = 0;
            m_mapX = 0.0;
            m_mapY = 0.0;
            m_zoomIn = 0.0;
            m_mapPy = 0;
            m_carX = 0;
            m_carY = 0;
        }

        ~ExamProcedure()
        { }

        public bool Init(int userId, int kch, int thirdPH, int fourthPH)
        {
            m_userId = userId;
            m_kch = kch;
            m_thirdPassiveHandle = thirdPH;
            m_fourthPassiveHandle = fourthPH;

            m_kskm = BaseMethod.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH_ENV, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_KSKM, BaseDefine.CONFIG_VALUE_KSKM_2);

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
                lock(m_lockFourth)
                {
                    m_strCurrentState = "考试开始";
                    m_CurrentXmFlag = 0;
                    m_CurrentScore = BaseDefine.CONFIG_VALUE_TOTAL_SCORE;
                    m_dicErrorInfo.Clear();
                    m_startTime = DateTime.Now;
                    m_bFinish = false;
                    m_bPass = false;
                }
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
            }

            //更新考生信息画面
            try
            {
                lock(m_lockThird)
                {
                    autoEventThird.Reset();

                    m_studentInfo = studentInfo;
                }
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
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
                lock(m_lockFourth)
                {
                    m_strCurrentState = xmlx;

                    //科目三地图模式需要实时展示项目状态图片，这里将其它状态清空，仅保留当前项目状态
                    if (m_bDrawMap && (BaseDefine.CONFIG_VALUE_KSKM_3 == m_kskm))  
                    {
                        m_CurrentXmFlag = 0;
                    }

                    uint xmFlag = GetXmFlag(xmCode, true);
                    m_CurrentXmFlag |= xmFlag;
                }
            }
            catch(Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}, xmlx = {1}", e.Message, xmlx);
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

            lock(m_lockFourth)
            {
                //只存放3条扣分信息，超过3条时覆盖第3条
                int nIndex = m_dicErrorInfo.Count;
                if (nIndex > 2)
                {
                    nIndex = 2;
                }
                m_dicErrorInfo[nIndex] = errorInfo;

                //扣除当前得分
                m_CurrentScore -= kcfs;
                if (m_CurrentScore <= BaseDefine.CONFIG_VALUE_ZERO_SCORE)
                {
                    m_CurrentScore = BaseDefine.CONFIG_VALUE_ZERO_SCORE;
                }
            }

            return true;
        }

        public bool HandleGPS(GPSData gpsData)
        {
            //考试实时信息
            try
            {
                lock(m_lockFourth)
                {
                    m_gpsData = gpsData;

                    if (m_bDrawMap)
                    {
                        int tempx = 0;
                        int tempy = 0;

                        if (1 == m_mapPy)
                        {
                            tempx = Math.Abs((int)((m_gpsData.Longitude - m_mapX) * m_zoomIn)) - 176;
                            tempy = Math.Abs((int)((m_gpsData.Latitude - m_mapY) * m_zoomIn)) - 144;
                        }
                        else
                        {
                            tempx = Math.Abs((int)((m_gpsData.Longitude - m_mapX) * m_zoomIn));
                            tempy = Math.Abs((int)((m_gpsData.Latitude - m_mapY) * m_zoomIn));
                        }

                        if (tempx < 0 || tempx > m_mapWidth || tempy < 0 || tempy > m_mapHeight)
                        {
                            Log.GetLogger().ErrorFormat("GPS数据存在异常，longitude={0}, latitude={1}, zoomin={2}", m_gpsData.Longitude,
                                m_gpsData.Latitude, m_zoomIn);
                        }
                        else
                        {
                            m_carX = tempx;
                            m_carY = tempy;
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
                return false;
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

                lock(m_lockFourth)
                {
                    m_strCurrentState = xmlx;

                    uint xmFlag = GetXmFlag(xmCode, false);

                    m_CurrentXmFlag |= xmFlag;
                }
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}, xmlx = {1}", e.Message, xmlx);
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
                lock(m_lockFourth)
                {
                    m_bFinish = true;   //考试结束
                    m_endTime = DateTime.Now;

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
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}, bPass = {1}", e.Message, bPass);
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
            int nLoadMap = BaseMethod.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH_ENV, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_LOADMAP, 0);
            if (1 == nLoadMap)
            {
                m_bDrawMap = true;
            }

            if (m_bDrawMap) //地图版本
            {
                LoadMapConfig();

                m_fourthPicThread = new Thread(new ThreadStart(FourthPicMapThread));
                m_fourthPicThread.Start();
            }
            else  //项目牌版本
            {
                m_fourthPicThread = new Thread(new ThreadStart(FourthPicInfoThread));
                m_fourthPicThread.Start();
            }

            return true;
        }

        private void LoadMapConfig()
        {
            lock(m_lockFourth)
            {
                imgMap = Image.FromFile(BaseDefine.IMG_PATH_MAPN);
                m_mapWidth = imgMap.Width;
                m_mapHeight = imgMap.Height;

                int xc = BaseMethod.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH_MAP, BaseDefine.CONFIG_SECTION_MAPCONFIG,
                    BaseDefine.CONFIG_KEY_XC, 0);
                int yc = BaseMethod.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH_MAP, BaseDefine.CONFIG_SECTION_MAPCONFIG,
                    BaseDefine.CONFIG_KEY_YC, 0);

                string keyX = string.Empty;
                if (1 == xc)
                {
                    keyX = BaseDefine.CONFIG_KEY_MINX;
                }
                else
                {
                    keyX = BaseDefine.CONFIG_KEY_MAXX;
                }
                string keyY = string.Empty;
                if (1 == yc)
                {
                    keyY = BaseDefine.CONFIG_KEY_MINY;
                }
                else
                {
                    keyY = BaseDefine.CONFIG_KEY_MAXY;
                }

                m_mapX = BaseMethod.INIGetDoubleValue(BaseDefine.CONFIG_FILE_PATH_MAP, BaseDefine.CONFIG_SECTION_MAPCONFIG,
                    keyX, 0.0);
                m_mapY = BaseMethod.INIGetDoubleValue(BaseDefine.CONFIG_FILE_PATH_MAP, BaseDefine.CONFIG_SECTION_MAPCONFIG,
                    keyY, 0.0);

                m_zoomIn = BaseMethod.INIGetDoubleValue(BaseDefine.CONFIG_FILE_PATH_MAP, BaseDefine.CONFIG_SECTION_MAPCONFIG,
                    BaseDefine.CONFIG_KEY_ZOOMIN, 0.0);

                int nDrawCar = BaseMethod.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH_ENV, BaseDefine.CONFIG_SECTION_CONFIG,
                    BaseDefine.CONFIG_KEY_DRAWCAR, 0);
                if (1 == nDrawCar)
                {
                    m_bDrawCar = true;

                    string carSkinPath = string.Empty;
                    int skinNo = BaseMethod.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH_CONFIG, BaseDefine.CONFIG_SECTION_CARSKIN,
                        m_kch.ToString(), 0);
                    if (0 == skinNo)
                    {
                        carSkinPath = string.Format(BaseDefine.IMG_PATH_SINGLE_CAR);
                    }
                    else
                    {
                        carSkinPath = string.Format(BaseDefine.IMG_PATH_MULTI_CAR, skinNo);
                    }

                    imgCar = Image.FromFile(carSkinPath);
                }

                m_mapPy = BaseMethod.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH_CONFIG, BaseDefine.CONFIG_SECTION_CONFIG,
                    BaseDefine.CONFIG_KEY_DITUPY, 1);
            }  
        }

        private void ThirdPicKeepThread()
        {
            autoEventThird = new AutoResetEvent(true);  //自动重置事件，默认为已触发
            while (true)
            {
                autoEventThird.WaitOne(Timeout.Infinite);

                try
                {
                    lock(m_lockThird)
                    {
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

                            if (null != m_studentInfo.ArrayZp)
                            {
                                Stream streamZp = new MemoryStream(m_studentInfo.ArrayZp);
                                Image imgZp = Image.FromStream(streamZp);
                                graphics.DrawImage(imgZp, new Rectangle(242, 10, 100, 126));
                            }
                            
                            if (null != m_studentInfo.ArrayMjzp)
                            {
                                Stream streamMjzp = new MemoryStream(m_studentInfo.ArrayMjzp);
                                Image imgMjzp = Image.FromStream(streamMjzp);
                                graphics.DrawImage(imgMjzp, new Rectangle(272, 140, 80, 100));
                            }
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
                }
                catch (Exception e)
                {
                    Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
                }
                
                System.Threading.Thread.Sleep(1000);

                autoEventThird.Set();   //触发事件
            }
        }

        private void FourthPicInfoThread()
        {
            autoEventFourthInfo = new AutoResetEvent(true);  //自动重置事件，默认为已触发
            while (true)
            {
                autoEventFourthInfo.WaitOne(Timeout.Infinite);

                try
                {
                    lock(m_lockFourth)
                    {
                        //重新初始化画板
                        Bitmap bm = new Bitmap(imgMark);
                        Graphics graphics = Graphics.FromImage(bm);

                        //绘制项目牌列表
                        if (BaseDefine.CONFIG_VALUE_KSKM_2 == m_kskm)
                        {
                            graphics.DrawImage(imgXmp, new Rectangle(264, 36, 88, 252), 0, 0, 88, 252, GraphicsUnit.Pixel);
                        }
                        else
                        {
                            graphics.DrawImage(imgXmp, new Rectangle(264, 0, 88, 288), 0, 0, 88, 288, GraphicsUnit.Pixel);
                        }

                        //绘制项目状态
                        DrawXmStateInfo(ref graphics);

                        //绘制实时状态信息
                        if (!string.IsNullOrEmpty(m_strCurrentState))
                        {
                            TimeSpan ts;
                            if (m_bFinish)
                            {
                                ts = m_endTime - m_startTime;
                            }
                            else
                            {
                                ts = DateTime.Now - m_startTime;
                            }
                            string strTotalTime = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                            string strScore = string.Format(BaseDefine.STRING_EXAM_TIME_AND_SCORE, strTotalTime, m_CurrentScore);
                            string strSpeed = string.Format(BaseDefine.STRING_CAR_SPEED, m_gpsData.Speed);
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
                    
                }
                catch (Exception e)
                {
                    Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
                }

                System.Threading.Thread.Sleep(1000);

                autoEventFourthInfo.Set();   //触发事件
            }

        }

        private void FourthPicMapThread()
        {
            autoEventFourthMap = new AutoResetEvent(true);  //自动重置事件，默认为已触发
            while (true)
            {
                autoEventFourthMap.WaitOne(Timeout.Infinite);

                try
                {
                    lock(m_lockFourth)
                    {
                        Font font = new Font("宋体", 10, FontStyle.Regular);

                        //重新初始化画板
                        //Bitmap bm = new Bitmap(352, 288);
                        Bitmap bm = new Bitmap(imgMap, 352, 288);
                        Graphics graphics = Graphics.FromImage(bm);
                        graphics.DrawImage(imgMap, new Rectangle(0, 0, 352, 288), m_carX, m_carY, 352, 288, GraphicsUnit.Pixel);

                        //绘制考车
                        if (m_bDrawCar)
                        {
                            graphics.TranslateTransform(176, 144);
                            graphics.RotateTransform(m_gpsData.DirectionAngle);
                            graphics.TranslateTransform(-176, -144);

                            graphics.DrawImage(imgCar, new Rectangle(0, 0, 352, 288));  //车模型

                            graphics.ResetTransform();
                        }

                        graphics.DrawImage(imgMark, new Rectangle(0, 0, 352, 288)); //遮罩

                        int nKskm = BaseMethod.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH_ENV, BaseDefine.CONFIG_SECTION_CONFIG,
                            BaseDefine.CONFIG_KEY_KSKM, 0);    //考试科目

                        //绘制实时状态信息
                        if (!string.IsNullOrEmpty(m_strCurrentState))
                        {
                            TimeSpan ts;
                            if (m_bFinish)
                            {
                                ts = m_endTime - m_startTime;
                            }
                            else
                            {
                                ts = DateTime.Now - m_startTime;
                            }

                            string speed = string.Format("{0} km/h", m_gpsData.Speed);
                            string mileage = string.Format("{0} m", m_gpsData.Mileage);
                            string score = string.Format("成绩:{0}", m_CurrentScore);
                            string time = string.Format("时长:{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);

                            graphics.DrawString(m_strCurrentState, font, brush, new Rectangle(72, 8, 348, 30));
                            graphics.DrawString(speed, font, brush, new Rectangle(0, 240, 98, 262));
                            graphics.DrawString(mileage, font, brush, new Rectangle(0, 265, 98, 288));
                            graphics.DrawString(score, font, brush, new Rectangle(263, 240, 350, 262));
                            graphics.DrawString(time, font, brush, new Rectangle(263, 265, 350, 288));
                        }

                        //绘制科目三考试项目牌
                        if (BaseDefine.CONFIG_VALUE_KSKM_3 == m_kskm)
                        {
                            DrawXmStateMap(ref graphics);
                        }

                        //绘制扣分信息
                        foreach (int index in m_dicErrorInfo.Keys)
                        {
                            string[] errorInfo = m_dicErrorInfo[index];
                            if (null == errorInfo)
                            {
                                continue;
                            }

                            string errorMsg = string.Format("{0} {1}", errorInfo[0], errorInfo[1]);

                            if (0 == index)
                            {
                                graphics.DrawString(errorMsg, font, brushBlack, new Rectangle(6, 216, 346, 236));
                            }
                            else if (1 == index)
                            {
                                graphics.DrawString(errorMsg, font, brushBlack, new Rectangle(6, 196, 346, 216));
                            }
                            else if (2 == index)
                            {
                                graphics.DrawString(errorMsg, font, brushBlack, new Rectangle(6, 176, 346, 196));
                            }
                        }

                        //发送画面到合码器
                        SendBitMapToHMQ(bm, m_kch, m_fourthPassiveHandle);
                    }
                }
                catch (Exception e)
                {
                    Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
                }

                System.Threading.Thread.Sleep(1000);

                autoEventFourthMap.Set();   //触发事件
            }

        }

        /// <summary>
        /// 绘制项目牌实时状态(项目牌模式)
        /// </summary>
        /// <param name="g"></param>
        private void DrawXmStateInfo(ref Graphics g)
        {
            #region 科目二
            if (BaseDefine.CONFIG_VALUE_KSKM_2 == m_kskm)
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
            #endregion

            #region 科目三
            if (BaseDefine.CONFIG_VALUE_KSKM_3 == m_kskm)
            {
                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_SC) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 0, 44, 36), 176, 0, 44, 36, GraphicsUnit.Pixel); //结束上车准备
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_SC) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 0, 44, 36), 88, 0, 44, 36, GraphicsUnit.Pixel); //开始上车准备
                }

                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_QB) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(308, 0, 44, 36), 220, 0, 44, 36, GraphicsUnit.Pixel); //结束起步
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_QB) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(308, 0, 44, 36), 132, 0, 44, 36, GraphicsUnit.Pixel); //开始起步
                }

                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_ZHIXIAN) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 36, 44, 36), 176, 36, 44, 36, GraphicsUnit.Pixel); //结束直线
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_ZHIXIAN) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 36, 44, 36), 88, 36, 44, 36, GraphicsUnit.Pixel); //开始直线
                }

                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_JJ) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(308, 36, 44, 36), 220, 36, 44, 36, GraphicsUnit.Pixel); //结束加减档
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_JJ) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(308, 36, 44, 36), 132, 36, 44, 36, GraphicsUnit.Pixel); //开始加减档
                }

                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_BG) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 72, 44, 36), 176, 72, 44, 36, GraphicsUnit.Pixel); //结束变更
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_BG) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 72, 44, 36), 88, 72, 44, 36, GraphicsUnit.Pixel); //开始变更
                }

                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_KB) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(308, 72, 44, 36), 220, 72, 44, 36, GraphicsUnit.Pixel); //结束靠边
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_KB) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(308, 72, 44, 36), 132, 72, 44, 36, GraphicsUnit.Pixel); //开始靠边
                }

                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_ZHIXING) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 108, 44, 36), 176, 108, 44, 36, GraphicsUnit.Pixel); //结束直行
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_ZHIXING) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 108, 44, 36), 88, 108, 44, 36, GraphicsUnit.Pixel); //开始直行
                }

                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_ZZ) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(308, 108, 44, 36), 220, 108, 44, 36, GraphicsUnit.Pixel); //结束左转
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_ZZ) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(308, 108, 44, 36), 132, 108, 44, 36, GraphicsUnit.Pixel); //开始左转
                }

                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_YZ) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 144, 44, 36), 176, 144, 44, 36, GraphicsUnit.Pixel); //结束右转
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_YZ) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 144, 44, 36), 88, 144, 44, 36, GraphicsUnit.Pixel); //开始右转
                }

                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_RX) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(308, 144, 44, 36), 220, 144, 44, 36, GraphicsUnit.Pixel); //结束人行
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_RX) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(308, 144, 44, 36), 132, 144, 44, 36, GraphicsUnit.Pixel); //开始人行
                }

                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_XX) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 180, 44, 36), 176, 180, 44, 36, GraphicsUnit.Pixel); //结束学校
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_XX) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 180, 44, 36), 88, 180, 44, 36, GraphicsUnit.Pixel); //开始学校
                }

                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_CZ) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(308, 180, 44, 36), 220, 180, 44, 36, GraphicsUnit.Pixel); //结束车站
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_CZ) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(308, 180, 44, 36), 132, 180, 44, 36, GraphicsUnit.Pixel); //开始车站
                }

                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_HC) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 216, 44, 36), 176, 216, 44, 36, GraphicsUnit.Pixel); //结束会车
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_HC) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 216, 44, 36), 88, 216, 44, 36, GraphicsUnit.Pixel); //开始会车
                }

                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_CC) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(308, 216, 44, 36), 220, 216, 44, 36, GraphicsUnit.Pixel); //结束超车
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_CC) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(308, 216, 44, 36), 132, 216, 44, 36, GraphicsUnit.Pixel); //开始超车
                }

                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_DT) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 252, 44, 36), 176, 252, 44, 36, GraphicsUnit.Pixel); //结束掉头
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_DT) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(264, 252, 44, 36), 88, 252, 44, 36, GraphicsUnit.Pixel); //开始掉头
                }

                if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_END_YJ) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(308, 252, 44, 36), 220, 252, 44, 36, GraphicsUnit.Pixel); //结束夜间
                }
                else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_YJ) > 0)
                {
                    g.DrawImage(imgXmp, new Rectangle(308, 252, 44, 36), 132, 252, 44, 36, GraphicsUnit.Pixel); //开始夜间
                }
            }
            #endregion
        }

        /// <summary>
        /// 绘制项目牌实时状态(地图模式)
        /// </summary>
        /// <param name="g"></param>
        private void DrawXmStateMap(ref Graphics g)
        {
            if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_SC) > 0)
            {
                g.DrawImage(imgXmp, new Rectangle(0, 0, 72, 72), 0, 72, 72, 72, GraphicsUnit.Pixel); //开始上车准备
            }

            else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_QB) > 0)
            {
                g.DrawImage(imgXmp, new Rectangle(0, 0, 72, 72), 0, 144, 72, 72, GraphicsUnit.Pixel); //开始起步
            }

            else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_ZHIXIAN) > 0)
            {
                g.DrawImage(imgXmp, new Rectangle(0, 0, 72, 72), 0, 216, 72, 72, GraphicsUnit.Pixel); //开始直线
            }

            else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_JJ) > 0)
            {
                g.DrawImage(imgXmp, new Rectangle(0, 0, 72, 72), 0, 288, 72, 72, GraphicsUnit.Pixel); //开始加减
            }

            else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_BG) > 0)
            {
                g.DrawImage(imgXmp, new Rectangle(0, 0, 72, 72), 0, 360, 72, 72, GraphicsUnit.Pixel); //开始变更
            }

            else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_KB) > 0)
            {
                g.DrawImage(imgXmp, new Rectangle(0, 0, 72, 72), 0, 432, 72, 72, GraphicsUnit.Pixel); //开始靠边
            }

            else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_ZHIXING) > 0)
            {
                g.DrawImage(imgXmp, new Rectangle(0, 0, 72, 72), 0, 504, 72, 72, GraphicsUnit.Pixel); //开始直行
            }

            else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_ZZ) > 0)
            {
                g.DrawImage(imgXmp, new Rectangle(0, 0, 72, 72), 0, 576, 72, 72, GraphicsUnit.Pixel); //开始左转
            }

            else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_YZ) > 0)
            {
                g.DrawImage(imgXmp, new Rectangle(0, 0, 72, 72), 0, 648, 72, 72, GraphicsUnit.Pixel); //开始右转
            }

            else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_RX) > 0)
            {
                g.DrawImage(imgXmp, new Rectangle(0, 0, 72, 72), 0, 720, 72, 72, GraphicsUnit.Pixel); //开始人行
            }

            else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_XX) > 0)
            {
                g.DrawImage(imgXmp, new Rectangle(0, 0, 72, 72), 0, 792, 72, 72, GraphicsUnit.Pixel); //开始学校
            }

            else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_CZ) > 0)
            {
                g.DrawImage(imgXmp, new Rectangle(0, 0, 72, 72), 0, 864, 72, 72, GraphicsUnit.Pixel); //开始车站
            }

            else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_HC) > 0)
            {
                g.DrawImage(imgXmp, new Rectangle(0, 0, 72, 72), 0, 936, 72, 72, GraphicsUnit.Pixel); //开始会车
            }

            else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_CC) > 0)
            {
                g.DrawImage(imgXmp, new Rectangle(0, 0, 72, 72), 0, 1008, 72, 72, GraphicsUnit.Pixel); //开始超车
            }

            else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_DT) > 0)
            {
                g.DrawImage(imgXmp, new Rectangle(0, 0, 72, 72), 0, 1080, 72, 72, GraphicsUnit.Pixel); //开始掉头
            }

            else if ((m_CurrentXmFlag & BaseDefine.EXAM_STATE_START_YJ) > 0)
            {
                g.DrawImage(imgXmp, new Rectangle(0, 0, 72, 72), 0, 1152, 72, 72, GraphicsUnit.Pixel); //开始夜间
            }
        }

        private bool SendBitMapToHMQ(Bitmap bm, int kch, int passiveHandle)
        {
            bool bRet = false;

            string videoPath = @".\video";
            Random rd = new Random();
            int nRand = rd.Next(0, 100);
            if (!Directory.Exists(videoPath))
            {
                Directory.CreateDirectory(videoPath);
            }
            string aviFilePath = string.Format(@"{0}\{1}_{2}_{3}.avi", videoPath, kch, passiveHandle, nRand);
            string yuvFilePath = string.Format(@"{0}\{1}_{2}_{3}.yuv", videoPath, kch, passiveHandle, nRand);

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
            startInfo.FileName = BaseDefine.STRING_FILE_PATH_MENCODER;
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
        private uint GetXmFlag(int xmCode, bool bStart)
        {
            uint nRet = 0;

            #region 科目二
            if (BaseDefine.CONFIG_VALUE_KSKM_2 == m_kskm)   //科目二
            {
                //当前科目二的项目编号为6位数字，如 201510，前3位201表示“倒车入库项目”，后3位表示不同的车库
                //只需要前3位即可判断具体的项目
                xmCode = xmCode / 1000;

                if (bStart)
                {
                    if (BaseDefine.XMBH_201 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_DCRK;
                    }
                    else if (BaseDefine.XMBH_204 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_CFTC;
                    }
                    else if (BaseDefine.XMBH_203 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_DDPQ;
                    }
                    else if (BaseDefine.XMBH_206 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_QXXS;
                    }
                    else if (BaseDefine.XMBH_207 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_ZJZW;
                    }
                    else if (BaseDefine.XMBH_214 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_MNSD;
                    }
                    else if (BaseDefine.XMBH_215 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_YWSH;
                    }
                    else if (BaseDefine.XMBH_216 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_YWSH;
                    }
                }
                else
                {
                    if (BaseDefine.XMBH_201 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_DCRK;
                    }
                    else if (BaseDefine.XMBH_204 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_CFTC;
                    }
                    else if (BaseDefine.XMBH_203 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_DDPQ;
                    }
                    else if (BaseDefine.XMBH_206 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_QXXS;
                    }
                    else if (BaseDefine.XMBH_207 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_ZJZW;
                    }
                    else if (BaseDefine.XMBH_214 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_MNSD;
                    }
                    else if (BaseDefine.XMBH_215 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_YWSH;
                    }
                    else if (BaseDefine.XMBH_216 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_YWSH;
                    }
                }
            }
            #endregion

            #region 科目三
            if (BaseDefine.CONFIG_VALUE_KSKM_3 == m_kskm)   //科目三
            {
                if (bStart)
                {
                    if (BaseDefine.XMBH_201 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_SC;
                    }
                    else if (BaseDefine.XMBH_202 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_QB;
                    }
                    else if (BaseDefine.XMBH_203 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_ZHIXIAN;
                    }
                    else if (BaseDefine.XMBH_204 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_BG;
                    }
                    else if (BaseDefine.XMBH_206 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_RX;
                    }
                    else if (BaseDefine.XMBH_207 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_XX;
                    }
                    else if (BaseDefine.XMBH_208 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_CZ;
                    }
                    else if (BaseDefine.XMBH_209 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_HC;
                    }
                    else if (BaseDefine.XMBH_210 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_CC;
                    }
                    else if (BaseDefine.XMBH_211 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_KB;
                    }
                    else if (BaseDefine.XMBH_212 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_DT;
                    }
                    else if (BaseDefine.XMBH_213 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_YJ;
                    }
                    else if (BaseDefine.XMBH_214 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_ZZ;
                    }
                    else if (BaseDefine.XMBH_215 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_YZ;
                    }
                    else if (BaseDefine.XMBH_216 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_ZHIXING;
                    }
                    else if (BaseDefine.XMBH_217 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_START_JJ;
                    }
                }
                else
                {
                    if (BaseDefine.XMBH_201 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_SC;
                    }
                    else if (BaseDefine.XMBH_202 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_QB;
                    }
                    else if (BaseDefine.XMBH_203 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_ZHIXIAN;
                    }
                    else if (BaseDefine.XMBH_204 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_BG;
                    }
                    else if (BaseDefine.XMBH_206 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_RX;
                    }
                    else if (BaseDefine.XMBH_207 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_XX;
                    }
                    else if (BaseDefine.XMBH_208 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_CZ;
                    }
                    else if (BaseDefine.XMBH_209 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_HC;
                    }
                    else if (BaseDefine.XMBH_210 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_CC;
                    }
                    else if (BaseDefine.XMBH_211 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_KB;
                    }
                    else if (BaseDefine.XMBH_212 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_DT;
                    }
                    else if (BaseDefine.XMBH_213 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_YJ;
                    }
                    else if (BaseDefine.XMBH_214 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_ZZ;
                    }
                    else if (BaseDefine.XMBH_215 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_YZ;
                    }
                    else if (BaseDefine.XMBH_216 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_ZHIXING;
                    }
                    else if (BaseDefine.XMBH_217 == xmCode)
                    {
                        nRet = BaseDefine.EXAM_STATE_END_JJ;
                    }
                }
            }
            #endregion

            return nRet;
        }

    }
}
