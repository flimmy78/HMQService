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
            m_data = Encoding.ASCII.GetString(data, 0 ,nSize);
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
                case BaseDefine.PACK_TYPE_M17C51:
                    {
                        HandleM17C51(nKch, strZkzh);
                    }
                    break;
                case BaseDefine.PACK_TYPE_M17C52:
                    {
                        HandleM17C52(nKch, strZkzh, strXmbh);
                    }
                    break;
                case BaseDefine.PACK_TYPE_M17C53:
                    {

                    }
                    break;
                case BaseDefine.PACK_TYPE_M17C54:
                    {
                        //项目抓拍照片，这里不需要处理
                    }
                    break;
                case BaseDefine.PACK_TYPE_M17C55:
                    {

                    }
                    break;
                case BaseDefine.PACK_TYPE_M17C56:
                    {

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
                    Log.GetLogger().InfoFormat("DataHandlerThreadProc 执行成功");
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
            } 

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

            if (nWnd2 != 1)
            {
                string key = string.Format("{0}_1", xmbh);
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

            //项目编号转换，数据库升级后可以不需要这段代码
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

            //获取扣分类型
            if (!m_dicJudgeRules.ContainsKey(xmbh))
            {
                Log.GetLogger().ErrorFormat("扣分类型 {0} 未配置，请检查配置", xmbh);
                return false;
            }
            string kflx = m_dicJudgeRules[xmbh].JudgementType;

            try
            {
                BaseMethod.TF17C52(kch, zkzmbh, xmCodeNew, kflx);
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("TF17C52 catch an error : {0}, kch = {1}, zkzmbh = {2}, xmCodeNew = {3}, kflx = {4}",
                    e.Message, kch, zkzmbh, xmCodeNew, kflx);
            }

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
    }
}
