using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMQService.Common
{
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

    public class BaseDefine
    {
        //合码器服务监听端口
        public static readonly int HMQ_SERVICE_DEFAULT_PORT = 6708;

        //配置文件
        public static readonly string CONFIG_FILE_PATH = @".\config.ini";
        public static readonly string CONFIG_SECTION_CONFIG = "CONFIG";
        public static readonly string CONFIG_SECTION_SQLLINK = "SQLLINK";
        public static readonly string CONFIG_SECTION_JMQ = "JMQ";
        public static readonly string CONFIG_KEY_SQLORACLE = "SQLORACLE";
        public static readonly string CONFIG_KEY_LOADMAP = "LOADMAP";
        public static readonly string CONFIG_KEY_SQLORTCP = "SQLORTCP";
        public static readonly string CONFIG_KEY_DEMONS = "Demons";
        public static readonly string CONFIG_KEY_VIDEOWND = "VIDEOWND";
        public static readonly string CONFIG_KEY_KSKM = "KSKM";
        public static readonly string CONFIG_KEY_DISPLAY = "DISPLAY";
        public static readonly string CONFIG_KEY_ServerPZ = "ServerPZ";
        public static readonly string CONFIG_KEY_N = "N";
        public static readonly string CONFIG_KEY_BNC = "BNC";

        //接口字符串分隔符
        public static readonly char SPLIT_CHAR_ASTERISK = '*';
        public static readonly char SPLIT_CHAR_AMPERSAND = '&';
        public static readonly char SPLIT_CHAR_HASH_SIGN = '#';
        public static readonly char SPLIT_CHAR_SPACE = ' ';
        public static readonly int INTERFACE_FIELD_COUNT_KM2 = 9;   //接口字段数量（科目二），如果后续接口调整，这里需要修改
        public static readonly int INTERFACE_FIELD_COUNT_KM3 = 10;  //接口字段数量（科目三），如果后续接口调整，这里需要修改
    }
}
