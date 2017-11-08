using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using AviFile;

namespace HMQService.Common
{
    public class BaseMethod
    {
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

        //从INI配置文件取值，返回double类型
        public static double INIGetDoubleValue(string iniFile, string section, string key, double defaultValue)
        {
            double dRet = defaultValue;

            try
            {
                string retStr = INIOperator.INIGetStringValue(iniFile, section, key, string.Empty);

                dRet = string.IsNullOrEmpty(retStr) ? defaultValue : double.Parse(retStr);
            }
            catch (Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}，inifile = {1}, section = {2}, key = {3}", e.Message, iniFile, section, key);
            }

            return dRet;
        }

        /// <summary>
        /// 检测指定文件是否存在,如果存在则返回true。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static bool IsExistFile(string filePath)
        {
            return File.Exists(filePath);
        }

        /// <summary>
        /// 删除指定文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        /// <summary>
        /// 将内存中的BitMap存为avi文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="bm"></param>
        /// <returns></returns>
        public static bool MakeAviFile(string filePath, Bitmap bm)
        {
            try
            {
                int frameRate = BaseDefine.VIDEO_FRAME_RATE;

                AviManager aviMgr = new AviManager(filePath, false);
                VideoStream aviStream = aviMgr.AddVideoStream(false, frameRate, bm);

                for (int i = 1; i < frameRate; i++)
                {
                    aviStream.AddFrame(bm);
                }

                aviMgr.Close();
            }
            catch(Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
                return false;
            }

            if (!IsExistFile(filePath))
            {
                Log.GetLogger().ErrorFormat("MakeAviFile 没有生成 avi 文件，filepath={0}", filePath);
                return false;
            }

            //Log.GetLogger().DebugFormat("MakeAviFile end, filepath = {0}", filePath);
            return true;
        }
    }
}
