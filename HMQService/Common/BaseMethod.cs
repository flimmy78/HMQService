using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMQService.Common
{
    public class BaseMethod
    {
        public static string[] SplitString(string data, char cOperator, out string errorMsg)
        {
            errorMsg = string.Empty;
            string[] retArray = null;

            if (string.IsNullOrEmpty(data))
            {
                errorMsg = string.Format("data参数为空");
                return null;
            }

            try
            {
                retArray = data.Split(cOperator);
            }
            catch(Exception e)
            {
                errorMsg = string.Format("分割字符串时产生异常：{0}", e.Message);
            }

            if (null == retArray || 0 == retArray.Length)
            {
                errorMsg = string.Format("Split方法返回为空");
                return null;
            }

            return retArray;
        }

        public static int INIGetIntValue(string iniFile, string section, string key, int defaultValue)
        {
            int nRet = defaultValue;

            try
            {
                string retStr = INIOperator.INIGetStringValue(iniFile, section, key, string.Empty);

                nRet = string.IsNullOrEmpty(retStr) ? defaultValue : int.Parse(retStr);
            }
            catch(Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}，inifile = {1}, section = {2}, key = {3}", e.Message, iniFile, section, key);
            }

            return nRet;
        }
    }
}
