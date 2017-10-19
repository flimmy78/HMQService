using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HMQService.Common;
using System.Threading;

namespace HMQService.Server
{
    public class DataHandler
    {
        private string m_data = string.Empty;
        private Thread m_dataHandlerThread = null;

        public DataHandler(Byte[] data, int nSize)
        {
            m_data = Encoding.ASCII.GetString(data, 0 ,nSize);

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

            foreach(string str in retArray)
            {
                Log.GetLogger().InfoFormat("分割数据 {0}", str);
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
    }
}
