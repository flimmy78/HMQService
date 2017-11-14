using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BekUtils.Util;
using log4net;
using System.Runtime.InteropServices;

namespace HikUtils
{
    public class HikUtils
    {
        /// <summary>
        /// 登录海康设备
        /// </summary>
        /// <param name="ip">设备IP</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="port">端口</param>
        /// <param name="userId">输出参数，登录id</param>
        /// <returns></returns>
        public static bool LoginHikDevice(string ip, string username, string password, string port, out int userId)
        {
            userId = -1;
            uint errorCode = 0;
            if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(port))
            {
                Log.GetLogger().ErrorFormat("LoginHikDevice fail，传入参数为空值");
                return false;
            }

            try
            {
                int nPort = string.IsNullOrEmpty(port) ? 8000 : int.Parse(port);
                CHCNetSDK.NET_DVR_DEVICEINFO_V30 m_struDeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();
                userId = CHCNetSDK.NET_DVR_Login_V30(ip, nPort, username, password, ref m_struDeviceInfo);
                if (-1 == userId)
                {
                    errorCode = CHCNetSDK.NET_DVR_GetLastError();
                    Log.GetLogger().ErrorFormat("NET_DVR_Login_V30 failed, error code = {0}", errorCode);
                    return false;
                }
            }
            catch(Exception e)
            {
                Log.GetLogger().ErrorFormat("catch an error : {0}", e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取设备能力集
        /// </summary>
        /// <param name="userId">登录id</param>
        /// <param name="struDecAbility">设备能力集</param>
        /// <returns></returns>
        public static bool GetDeviceAbility(int userId, ref CHCNetSDK.NET_DVR_MATRIX_ABILITY_V41 struDecAbility)
        {
            uint errorCode = 0;

            try
            {
                //获取设备能力集
                int nSize = Marshal.SizeOf(struDecAbility);
                IntPtr ptrDecAbility = Marshal.AllocHGlobal(nSize);
                Marshal.StructureToPtr(struDecAbility, ptrDecAbility, false);
                if (!CHCNetSDK.NET_DVR_GetDeviceAbility(userId, CHCNetSDK.MATRIXDECODER_ABILITY_V41, IntPtr.Zero, 0, ptrDecAbility, (uint)nSize))
                {
                    errorCode = CHCNetSDK.NET_DVR_GetLastError();
                    Log.GetLogger().ErrorFormat("Get MATRIXDECODER_ABILITY_V41 failed, error code = {0}", errorCode);
                    return false;
                }

                struDecAbility = (CHCNetSDK.NET_DVR_MATRIX_ABILITY_V41)Marshal.PtrToStructure(ptrDecAbility,
                    typeof(CHCNetSDK.NET_DVR_MATRIX_ABILITY_V41));
            }
            catch (Exception e)
            {
                Log.GetLogger().InfoFormat("catch an error : {0}", e.Message);
                return false;
            }

            return true;
        }
    }
}
