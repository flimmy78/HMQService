using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BekUtils.Util
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
            catch (Exception e)
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
    }
}
