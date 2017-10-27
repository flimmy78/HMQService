using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HMQService.Common;
using System.Threading;
using HMQService.Decode;
using HMQService.Database;
using HMQService.Model;
using System.Data;

namespace HMQService.Server
{
    public class DataHandler
    {
        private string m_data = string.Empty;
        private Thread m_dataHandlerThread = null;
        private Dictionary<int, CarManager> m_dicCars = new Dictionary<int, CarManager>();
        private Dictionary<string, CameraConf> m_dicCameras = new Dictionary<string, CameraConf>();
        private Dictionary<string, JudgementRule> m_dicJudgeRules = new Dictionary<string, JudgementRule>();
        private IDataProvider m_sqlDataProvider = null;

        public DataHandler(Byte[] data, int nSize, Dictionary<int, CarManager> dicCars, Dictionary<string, CameraConf> dicCameras,
            Dictionary<string, JudgementRule> dicRules, IDataProvider sqlDataProvider)
        {
            m_data = Encoding.ASCII.GetString(data, 0, nSize);
            m_dicCars = dicCars;
            m_dicCameras = dicCameras;
            m_dicJudgeRules = dicRules;
            m_sqlDataProvider = sqlDataProvider;

            Log.GetLogger().InfoFormat("接收到车载数据：{0}", m_data);
        }

        ~DataHandler()
        {
            StopHandle();
        }

        public void StartHandle()
        {
            if (!string.IsNullOrEmpty(m_data))
            {
                m_dataHandlerThread =
                    new Thread(new ThreadStart(DataHandlerThreadProc));

                m_dataHandlerThread.Start();
            }
        }

        public void StopHandle()
        {
            // Wait for one second for the the thread to stop.
            m_dataHandlerThread.Join(1000);

            // If still alive; Get rid of the thread.
            if (m_dataHandlerThread.IsAlive)
            {
                m_dataHandlerThread.Abort();
            }
            m_dataHandlerThread = null;
        }

        private void DataHandlerThreadProc()
        {
            string errorMsg = string.Empty;

            //解析车载数据
            string[] retArray = BaseMethod.SplitString(m_data, BaseDefine.SPLIT_CHAR_ASTERISK, out errorMsg);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                goto END;
            }

            int nLength = retArray.Length;
            if (nLength != BaseDefine.INTERFACE_FIELD_COUNT_KM2
                && nLength != BaseDefine.INTERFACE_FIELD_COUNT_KM3)
            {
                errorMsg = string.Format("车载数据格式不正确，{0} 数量不对", BaseDefine.SPLIT_CHAR_ASTERISK);
                goto END;
            }

            string strKch = retArray[1];
            int nKch = string.IsNullOrEmpty(strKch) ? 0 : int.Parse(strKch);    //考车号
            string strType = retArray[2];
            int nType = string.IsNullOrEmpty(strType) ? 0 : int.Parse(strType); //类型
            string strXmbh = retArray[5];   //项目编号
            string strZkzh = retArray[6];   //准考证号
            string strTime = retArray[7];   //时间
            string score = retArray[8]; //得分

            if (!m_dicCars.ContainsKey(nKch))
            {
                Log.GetLogger().ErrorFormat("找不到考车{0}，请检查配置", nKch);
                return;
            }
            Log.GetLogger().InfoFormat(
                "接收到车载接口信息，考车号={0}, 类型={1}, 项目编号={2}, 准考证号={3}, 时间={4}, 得分={5}",
                nKch, nType, strXmbh, strZkzh, strTime, score);

            switch (nType)
            {
                case BaseDefine.PACK_TYPE_M17C51:   //考试开始
                    {
                        HandleM17C51(nKch, strZkzh);
                    }
                    break;
                case BaseDefine.PACK_TYPE_M17C52:   //项目开始
                    {
                        HandleM17C52(nKch, strZkzh, strXmbh);
                    }
                    break;
                case BaseDefine.PACK_TYPE_M17C53:   //扣分
                    {
                        HandleM17C53(nKch, strXmbh, strTime);
                    }
                    break;
                case BaseDefine.PACK_TYPE_M17C54:
                    {
                        //项目抓拍照片，这里不需要处理
                    }
                    break;
                case BaseDefine.PACK_TYPE_M17C55:   //项目完成
                    {
                        HandleM17C55(nKch, strZkzh, strXmbh);
                    }
                    break;
                case BaseDefine.PACK_TYPE_M17C56:   //考试完成
                    {
                        string strKscj = retArray[5];  //考试完成时该字段表示考试成绩
                        int kscj = string.IsNullOrEmpty(strKscj) ? 0 : int.Parse(strKscj);
                        if (0 == kscj)
                        {
                            errorMsg = string.Format("车载接口传的考试成绩为 {0}", strKscj);
                            goto END;
                        }

                        HandleM17C56(nKch, kscj);
                    }
                    break;
                default:
                    break;
            }

        END:
            {
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    Log.GetLogger().ErrorFormat("处理车载数据(DataHandlerThreadProc)时产生错误，{0}", errorMsg);
                }
                else
                {
                    Log.GetLogger().InfoFormat("DataHandlerThreadProc 执行结束");
                }
            }

            return;
        }

        /// <summary>
        /// 考试开始？
        /// </summary>
        /// <param name="kch">考车号</param>
        /// <param name="zkzmbh">准考证明编号</param>
        /// <returns></returns>
        private bool HandleM17C51(int kch, string zkzmbh)
        {
            int kscs = 0;   //考试次数
            int drcs = 0;   //当日次数
            if (!GetExamCount(zkzmbh, ref kscs, ref drcs))
            {
                return false;
            }

            try
            {
                BaseMethod.TF17C51(kch, zkzmbh, kscs, drcs);
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("TF17C51 catch an error : {0}, kch = {1}, zkzmbh = {2}, kscs = {3}, drcs = {4}",
                    e.Message, kch, zkzmbh, kscs, drcs);
                return false;
            }

            Log.GetLogger().InfoFormat("HandleM17C51 end, kch={0}, zkzmbh={1}, kscs={2}, drcs={3}", kch, zkzmbh, kscs, drcs);
            return true;
        }

        /// <summary>
        /// 项目开始
        /// </summary>
        /// <param name="kch">考车号</param>
        /// <param name="zkzmbh">准考证明编号</param>
        /// <param name="xmbh">项目编号</param>
        /// <returns></returns>
        private bool HandleM17C52(int kch, string zkzmbh, string xmbh)
        {
            int xmCode = string.IsNullOrEmpty(xmbh) ? 0 : int.Parse(xmbh);
            int nWnd2 = BaseMethod.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_WND2, 0);    //画面二状态
            Log.GetLogger().DebugFormat("nWnd2 = {0}", nWnd2);

            if (nWnd2 != 1)
            {
                string key = string.Format("{0}_1", xmbh);
                Log.GetLogger().DebugFormat("camera key = {0}", key);
                if (!m_dicCameras.ContainsKey(key))
                {
                    Log.GetLogger().ErrorFormat("摄像头 {0} 未配置，请检查配置文件。", key);
                    return false;
                }

                CameraConf camera = m_dicCameras[key];
                m_dicCars[kch].StartDynamicDecode(camera, 1);   //第二画面进项目

                //处理定点
                if ((BaseDefine.XMBH_15010 == xmCode) || (BaseDefine.XMBH_15020 == xmCode) || (BaseDefine.XMBH_15030 == xmCode))
                {
                    Log.GetLogger().InfoFormat("定点：{0}", xmCode);

                    if (BaseMethod.IsExistFile(BaseDefine.ZZIPChannel_FILE_PATH))
                    {
                        XmInfo xmInfo = new XmInfo(kch, xmCode);

                        Thread QHThread = new Thread(new ParameterizedThreadStart(QHThreadProc));
                        QHThread.Start(xmInfo);
                    }

                    return true;
                }
            }

            //项目编号转换，科目二专用，数据库升级后可以不需要这段代码
            int xmCodeNew = GetKM2NewXmBh(xmCode);
            Log.GetLogger().DebugFormat("xmCodeNew = {0}", xmCodeNew);

            //获取扣分类型
            if (!m_dicJudgeRules.ContainsKey(xmbh))
            {
                Log.GetLogger().ErrorFormat("扣分类型 {0} 未配置，请检查配置", xmbh);
                return false;
            }
            string kflx = m_dicJudgeRules[xmbh].JudgementType;

            //获取考生照片
            Byte[] arrayZp = null; //身份证照片
            Byte[] arrayMjzp = null;   //签到照片
            if (!GetStudentPhoto(zkzmbh, ref arrayZp, ref arrayMjzp))
            {
                return false;
            }

            

            try
            {
                //使用 C++ dll 进行绘制
                Log.GetLogger().DebugFormat("kch={0}, zkzmbh={1}, xmCode={2}, kflx={3}", kch, zkzmbh, xmCodeNew, kflx);
                BaseMethod.TF17C52(kch, zkzmbh, xmCodeNew, kflx);
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("TF17C52 catch an error : {0}, kch = {1}, zkzmbh = {2}, xmCodeNew = {3}, kflx = {4}",
                    e.Message, kch, zkzmbh, xmCodeNew, kflx);
                return false;
            }

            Log.GetLogger().InfoFormat("HandleM17C52 end, kch={0}, zkzmbh={1}", kch, zkzmbh);
            return true;
        }

        private bool HandleM17C52(int kch, int bh)
        {
            string key = string.Format("{0}_{1}", kch, bh);
            if (!m_dicCameras.ContainsKey(key))
            {
                Log.GetLogger().ErrorFormat("摄像头配置 {0} 不存在，请检查配置", key);
                return false;
            }

            CameraConf camera = m_dicCameras[key];
            if (!m_dicCars[kch].StartDynamicDecode(camera, 1))
            {
                Log.GetLogger().ErrorFormat("HandleM17C52 画面切换失败，{0}", key);
                return false;
            }

            Log.GetLogger().InfoFormat("HandleM17C52 画面切换，{0}", key);
            return true;
        }

        private void QHThreadProc(object obj)
        {
            XmInfo xmInfo = (XmInfo)obj;

            int kch = xmInfo.Kch;
            int xmCode = xmInfo.XmCode;

            string section = string.Format("{0}{1}", BaseDefine.CONFIG_SECTION_Q, xmCode);
            int sleepTime = BaseMethod.INIGetIntValue(BaseDefine.ZZIPChannel_FILE_PATH, section,
                BaseDefine.CONFIG_KEY_TIME, 2000);

            System.Threading.Thread.Sleep(sleepTime);

            HandleM17C52(kch, 1);

        }

        /// <summary>
        /// 项目扣分
        /// </summary>
        /// <param name="kch">考车号</param>
        /// <param name="xmbh">项目编号，包含项目编号和错误编号，用@分隔</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        private bool HandleM17C53(int kch, string xmbh, string time)
        {
            string errorMsg = string.Empty;
            string[] strArray = BaseMethod.SplitString(xmbh, BaseDefine.SPLIT_CHAR_AT, out errorMsg);
            if (!string.IsNullOrEmpty(errorMsg) || strArray.Length != 2)
            {
                Log.GetLogger().ErrorFormat("17C53 接口存在错误，{0}", errorMsg);
                return false;
            }
            string strXmCode = strArray[0];
            string strErrrorCode = strArray[1];
            int xmCode = string.IsNullOrEmpty(strXmCode) ? 0 : int.Parse(strXmCode);

            int kskm = BaseMethod.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_KSKM, 0); //考试科目
            string xmName = string.Empty;   //项目名称
            if (BaseDefine.CONFIG_VALUE_KSKM_3 == kskm) //科目三
            {
                xmName = GetKM3Name(xmCode);
            }
            else  //科目二
            {
                xmName = GetKM2Name(xmCode);
            }

            //扣分类型、扣除分数
            if (!m_dicJudgeRules.ContainsKey(strErrrorCode))
            {
                Log.GetLogger().ErrorFormat("数据库扣分规则表中不存在错误编号为 {0} 的记录，请检查配置。", strErrrorCode);
                return false;
            }
            string kflx = m_dicJudgeRules[strErrrorCode].JudgementType;
            int kcfs = m_dicJudgeRules[strErrrorCode].Points;

            try
            {
                //参数：考车号，项目名称，扣分类型，扣除分数
                BaseMethod.TF17C53(kch, xmName, kflx, kcfs);
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("TF17C53 catch an error : {0}, kch={1}, xmName={2}, kflx={3}, kcfs={4}", e.Message,
                    kch, xmName, kflx, kcfs);
                return false;
            }

            Log.GetLogger().InfoFormat("HandleM17C53 end, kch={0}, xmName={1}, kflx={2}, kcfs={3}", kch, xmName, kflx, kcfs);
            return true;
        }

        //根据项目编号，获取科目三项目名称
        private string GetKM3Name(int xmCode)
        {
            string xmName = string.Empty;
            switch (xmCode)
            {
                case BaseDefine.XMBH_201:
                    xmName = BaseDefine.XMMC_SCZB;  //上车准备
                    break;
                case BaseDefine.XMBH_202:
                    xmName = BaseDefine.XMMC_QB;  //起步
                    break;
                case BaseDefine.XMBH_203:
                    xmName = BaseDefine.XMMC_ZHIXIAN;   //直线
                    break;
                case BaseDefine.XMBH_204:
                    xmName = BaseDefine.XMMC_BG;    //变更
                    break;
                case BaseDefine.XMBH_205:
                    xmName = BaseDefine.XMMC_TGLK;  //通过路口
                    break;
                case BaseDefine.XMBH_206:
                    xmName = BaseDefine.XMMC_RX;    //人行
                    break;
                case BaseDefine.XMBH_207:
                    xmName = BaseDefine.XMMC_XX;    //学校
                    break;
                case BaseDefine.XMBH_208:
                    xmName = BaseDefine.XMMC_CC;    //车站
                    break;
                case BaseDefine.XMBH_209:
                    xmName = BaseDefine.XMMC_HC;    //会车
                    break;
                case BaseDefine.XMBH_210:
                    xmName = BaseDefine.XMMC_CC;    //超车
                    break;
                case BaseDefine.XMBH_211:
                    xmName = BaseDefine.XMMC_KB;    //靠边
                    break;
                case BaseDefine.XMBH_212:
                    xmName = BaseDefine.XMMC_DT;    //掉头
                    break;
                case BaseDefine.XMBH_213:
                    xmName = BaseDefine.XMMC_YJ;    //夜间
                    break;
                case BaseDefine.XMBH_214:
                    xmName = BaseDefine.XMMC_ZZ;    //左转
                    break;
                case BaseDefine.XMBH_215:
                    xmName = BaseDefine.XMMC_YZ;    //右转
                    break;
                case BaseDefine.XMBH_216:
                    xmName = BaseDefine.XMMC_ZHIXING;    //直行
                    break;
                case BaseDefine.XMBH_217:
                    xmName = BaseDefine.XMMC_JJ;    //加减
                    break;
                default:
                    xmName = BaseDefine.XMMC_ZH;    //综合
                    break;
            }


            return xmName;
        }

        //根据项目编号，获取科目二项目名称
        private string GetKM2Name(int xmCode)
        {
            string xmName = string.Empty;

            if ((xmCode > BaseDefine.XMBH_201509) && (xmCode < BaseDefine.XMBH_201700))
            {
                xmName = BaseDefine.XMMC_DCRK;  //倒车入库
            }
            else if ((xmCode > BaseDefine.XMBH_204509) && (xmCode < BaseDefine.XMBH_204700))
            {
                xmName = BaseDefine.XMMC_CFTC;  //侧方停车
            }
            else if ((xmCode > BaseDefine.XMBH_203509) && (xmCode < BaseDefine.XMBH_203700))
            {
                xmName = BaseDefine.XMMC_DDPQ;  //定点坡起
            }
            else if ((xmCode > BaseDefine.XMBH_206509) && (xmCode < BaseDefine.XMBH_206700))
            {
                xmName = BaseDefine.XMMC_QXXS;  //曲线行驶
            }
            else if ((xmCode > BaseDefine.XMBH_207509) && (xmCode < BaseDefine.XMBH_207700))
            {
                xmName = BaseDefine.XMMC_ZJZW;  //直角转弯
            }
            else if (BaseDefine.XMBH_249 == xmCode)
            {
                xmName = BaseDefine.XMMC_MNSD;  //模拟遂道
            }
            else if (BaseDefine.XMBH_259 == xmCode)
            {
                xmName = BaseDefine.XMMC_YWSH;  //雨雾湿滑
            }
            else
            {
                xmName = BaseDefine.XMMC_ZHPP;  //综合评判
            }

            return xmName;
        }

        /// <summary>
        /// 项目完成
        /// </summary>
        /// <param name="kch">考车号</param>
        /// <param name="strZkzmbh">准考证明</param>
        /// <param name="strXmbh">项目编号</param>
        /// <returns></returns>
        private bool HandleM17C55(int kch, string strZkzmbh, string strXmbh)
        {
            //项目开始编号与项目完成编号不一样，车载没有把完成编号传过来，这里需要根据开始编号进行转换
            int xmBeginCode = string.IsNullOrEmpty(strXmbh) ? 0 : int.Parse(strXmbh);
            int xmEndCode = 0;

            int kskm = BaseMethod.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_KSKM, 0); //考试科目
            if (BaseDefine.CONFIG_VALUE_KSKM_3 == kskm) //科目三
            {
                string key = string.Format("考车{0}_2", kch);
                if (!m_dicCameras.ContainsKey(key))
                {
                    Log.GetLogger().ErrorFormat("找不到 {0} 摄像头配置，请检查配置", key);
                    //return false;
                }
                else
                {
                    CameraConf camera = m_dicCameras[key];
                    m_dicCars[kch].StartDynamicDecode(camera, 1);   //车载视频动态，第二画面车外
                }

                //科目三的项目完成编号为，开始编号+700
                //218 --> 918
                if (xmBeginCode < BaseDefine.XMBH_700)
                {
                    xmEndCode = xmBeginCode + BaseDefine.XMBH_700;
                }
                else
                {
                    xmEndCode = xmBeginCode;
                }
            }
            else  //科目二
            {
                //项目编号转换，科目二专用，数据库升级后可以不需要这段代码
                xmBeginCode = GetKM2NewXmBh(xmBeginCode);

                //e.g. 201500 --> 201990
                // 201500 先除以 1000，得到 201。再乘以 1000，得到 201000。再加上 990，得到 201990。
                xmEndCode = (xmBeginCode / 1000) * 1000 + 990;
            }

            //获取扣分类型
            if (!m_dicJudgeRules.ContainsKey(xmEndCode.ToString()))
            {
                Log.GetLogger().ErrorFormat("ErrorData 表不存在 {0} 记录，请检查配置", xmEndCode);
                return false;
            }
            string kflx = m_dicJudgeRules[xmEndCode.ToString()].JudgementType;

            try
            {
                BaseMethod.TF17C55(kch, xmBeginCode, kflx);
            }
            catch(Exception e)
            {
                Log.GetLogger().ErrorFormat("TF17C55 catch an error : {0}, kch={1}, xmBeginCode={2}, kflx={3}", e.Message,
                    kch, xmBeginCode, kflx);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 考试完成
        /// </summary>
        /// <param name="kch">考车号</param>
        /// <param name="kscj">考试成绩</param>
        /// <returns></returns>
        private bool HandleM17C56(int kch, int kscj)
        {
            int kshgfs = 0; //考试合格分数

            int kskm = BaseMethod.INIGetIntValue(BaseDefine.CONFIG_FILE_PATH, BaseDefine.CONFIG_SECTION_CONFIG,
                BaseDefine.CONFIG_KEY_KSKM, 0); //考试科目
            if (BaseDefine.CONFIG_VALUE_KSKM_3 == kskm) //科目三
            {
                kshgfs = BaseDefine.CONFIG_VALUE_KSHGFS_3;
            }
            else  //科目二
            {
                kshgfs = BaseDefine.CONFIG_VALUE_KSHGFS_2;
            }

            try
            {
                if (kscj >= kshgfs) //考试合格
                {
                    BaseMethod.TF17C56(kch, 1, kscj);
                }
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("TF17C56 catch an error : {0}, 考车号={1}, 科目{2}, 考试成绩={3}", e.Message,
                    kch, kskm, kscj);
                return false;
            }

            Log.GetLogger().InfoFormat("TF17C56 end, 考车号={0}, 科目{1}, 考试成绩={2}", kch, kskm, kscj);
            return true;
        }

        /// <summary>
        /// 从数据库查询考生的考试次数和当日次数
        /// </summary>
        /// <param name="zkzmbh">准考证明编号</param>
        /// <param name="kscs">考试次数</param>
        /// <param name="drcs">当日次数</param>
        /// <returns></returns>
        private bool GetExamCount(string zkzmbh, ref int kscs, ref int drcs)
        {
            kscs = -1;
            drcs = -1;

            try
            {
                string sql = string.Format("select {0},{1} from {2} where {3}='{4}';",
                    BaseDefine.DB_FIELD_KSCS, BaseDefine.DB_FIELD_DRCS, BaseDefine.DB_TABLE_STUDENTINFO,
                    BaseDefine.DB_FIELD_ZKZMBH, zkzmbh);
                DataSet ds = m_sqlDataProvider.RetriveDataSet(sql);
                if (null != ds)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string strKscs = (null == ds.Tables[0].Rows[i][0]) ? string.Empty : ds.Tables[0].Rows[i][0].ToString();
                        string strDrcs = (null == ds.Tables[0].Rows[i][1]) ? string.Empty : ds.Tables[0].Rows[i][1].ToString();
                        kscs = string.IsNullOrEmpty(strKscs) ? 0 : int.Parse(strKscs);
                        drcs = string.IsNullOrEmpty(strDrcs) ? 0 : int.Parse(strDrcs);

                        break;
                    }
                }
            }
            catch(Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
                return false;
            }

            if (-1==kscs || -1==drcs)
            {
                Log.GetLogger().ErrorFormat("从数据库 studentInfo 表获取准考证为 {0} 的考生信息失败", zkzmbh);
                return false;
            }

            Log.GetLogger().InfoFormat("考生 {0} 的考试考试次数为 {1}，当日次数为 {2}", zkzmbh, kscs, drcs);
            return true;
        }

        //科目二的项目编号有更新，这里将旧的编号转换为新的编号
        //如果现场数据库已升级到最新，则不需要调用该函数
        private int GetKM2NewXmBh(int xmCode)
        {
            int xmCodeNew = 0;
            if (xmCode > BaseDefine.XMBH_300 && xmCode < BaseDefine.XMBH_400)
            {
                xmCodeNew = BaseDefine.XMBH_201510;
            }
            else if (xmCode > BaseDefine.XMBH_400 && xmCode < BaseDefine.XMBH_500)
            {
                xmCodeNew = BaseDefine.XMBH_204510;
            }
            else if (xmCode > BaseDefine.XMBH_500 && xmCode < BaseDefine.XMBH_600)
            {
                xmCodeNew = BaseDefine.XMBH_203510;
            }
            else if (xmCode > BaseDefine.XMBH_600 && xmCode < BaseDefine.XMBH_700)
            {
                xmCodeNew = BaseDefine.XMBH_206510;
            }
            else if (xmCode > BaseDefine.XMBH_700 && xmCode < BaseDefine.XMBH_800)
            {
                xmCodeNew = BaseDefine.XMBH_207510;
            }
            else
            {
                xmCodeNew = xmCode;
            }

            return xmCodeNew;
        }

        /// <summary>
        /// 根据准考证明从数据库获取考生照片和门禁照片(现场抓拍，签到照片)
        /// </summary>
        /// <param name="zkzmbh">准考证明</param>
        /// <param name="arrayZp">身份证照片信息</param>
        /// <param name="arrayMjzp">签到照片信息</param>
        /// <returns></returns>
        private bool GetStudentPhoto(string zkzmbh, ref Byte[] arrayZp, ref Byte[] arrayMjzp)
        {
            string sql = string.Format("select {0},{1} from {2} where {3}='{4}';",
                BaseDefine.DB_FIELD_ZP,
                BaseDefine.DB_FIELD_MJZP,
                BaseDefine.DB_TABLE_STUDENTPHOTO,
                BaseDefine.DB_FIELD_ZKZMBH,
                zkzmbh);

            try
            {
                DataSet ds = m_sqlDataProvider.RetriveDataSet(sql);
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows != null)
                {
                    arrayZp = (Byte[])ds.Tables[0].Rows[0][0];
                    arrayMjzp = (Byte[])ds.Tables[0].Rows[0][1];
                }
            }
            catch(Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}, zkzmbh={1}", e.Message, zkzmbh);
                return false;
            }

            if (null==arrayZp || null==arrayMjzp || 0==arrayZp.Length || 0==arrayMjzp.Length)
            {
                Log.GetLogger().ErrorFormat("StudentPhoto表查询不到数据，zkzmbh={0}", zkzmbh);
                return false;
            }

            Log.GetLogger().DebugFormat("GetStudentPhoto success, zkzmbh={0}", zkzmbh);
            return true;
        }
    }
}
