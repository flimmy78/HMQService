using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HMQService.Common
{
    public class BaseMethod
    {
        [DllImport("CYuvToH264T.dll", CharSet = CharSet.Auto)]
        public static extern void TFInit(int ikch, int str);
        [DllImport("CYuvToH264T.dll", CharSet = CharSet.Auto)]
        public static extern void TFPassiveHandle(int lPassHandle, int ikch, int itf);
        [DllImport("CYuvToH264T.dll", CharSet = CharSet.Auto)]
        public static extern void TF17C51(int ikch, string zkzm, int ikscs, int idrcs);
        [DllImport("CYuvToH264T.dll", CharSet = CharSet.Auto)]
        public static extern void TF17C52(int ikch, string zkzm, int ic, string msgz);
        [DllImport("CYuvToH264T.dll", CharSet = CharSet.Auto)]
        public static extern void TF17C53(int ikch, string timestr, string msgz, int ikcfs);
        [DllImport("CYuvToH264T.dll", CharSet = CharSet.Auto)]
        public static extern void TF17C54(int ikch, IntPtr msgz);
        [DllImport("CYuvToH264T.dll", CharSet = CharSet.Auto)]
        public static extern void TF17C55(int ikch, int ic, string msgz);
        [DllImport("CYuvToH264T.dll", CharSet = CharSet.Auto)]
        public static extern void TF17C56(int ikch, int itype, int ikscj);

        //字符串分割
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

        //从INI配置文件取值，返回int类型
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

        /// <summary>
        /// 检测指定文件是否存在,如果存在则返回true。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static bool IsExistFile(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
