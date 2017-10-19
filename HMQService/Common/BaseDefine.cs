using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMQService.Common
{
    public class BaseDefine
    {
        //合码器服务监听端口
        public static readonly int HMQ_SERVICE_DEFAULT_PORT = 6708;

        //接口字符串分隔符
        public static readonly char SPLIT_CHAR_ASTERISK = '*';
        public static readonly char SPLIT_CHAR_AMPERSAND = '&';
        public static readonly char SPLIT_CHAR_HASH_SIGN = '#';
        public static readonly char SPLIT_CHAR_SPACE = ' ';
        public static readonly int INTERFACE_FIELD_COUNT_KM2 = 9;   //接口字段数量（科目二）
        public static readonly int INTERFACE_FIELD_COUNT_KM3 = 10;  //接口字段数量（科目三）
    }

    public enum PackType
    {
        SOCKZREEOR = 0,
        GNSSDATA,
        JGPTDATA,
        JMQStart,
        QueryStart,
        AllCarNum,
        M17C51,
        M17C52,
        M17C53,
        M17C54,
        M17C55,
        M17C56
    }
}
