using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMQService.Common
{
    public class BaseDefine
    {
        //TCP服务监听端口
        public static readonly int HMQ_SERVICE_DEFAULT_PORT = 6708;

        //数据格式
        public const int  PACK_TYPE_M17C51 = 1;
        public const int PACK_TYPE_M17C52 = 2;
        public const int PACK_TYPE_M17C53 = 3;
        public const int PACK_TYPE_M17C54 = 4;
        public const int PACK_TYPE_M17C55 = 5;
        public const int PACK_TYPE_M17C56 = 6;

        //项目编号
        public static readonly int XMBH_300= 300;
        public static readonly int XMBH_400 = 400;
        public static readonly int XMBH_500 = 500;
        public static readonly int XMBH_600 = 600;
        public static readonly int XMBH_700 = 700;
        public static readonly int XMBH_800 = 800;
        public static readonly int XMBH_15010 = 15010;
        public static readonly int XMBH_15020 = 15020;
        public static readonly int XMBH_15030 = 15030;
        public static readonly int XMBH_201510 = 201510;
        public static readonly int XMBH_204510 = 204510;
        public static readonly int XMBH_203510 = 203510;
        public static readonly int XMBH_206510 = 206510;
        public static readonly int XMBH_207510 = 207510;

        //接口字符串分隔符
        public static readonly char SPLIT_CHAR_ASTERISK = '*';
        public static readonly char SPLIT_CHAR_AMPERSAND = '&';
        public static readonly char SPLIT_CHAR_HASH_SIGN = '#';
        public static readonly char SPLIT_CHAR_SPACE = ' ';
        public static readonly char SPLIT_CHAR_COMMA = ',';
        public static readonly int INTERFACE_FIELD_COUNT_KM2 = 9;   //接口字段数量（科目二），如果后续接口调整，这里需要修改
        public static readonly int INTERFACE_FIELD_COUNT_KM3 = 10;  //接口字段数量（科目三），如果后续接口调整，这里需要修改

        //配置文件
        public static readonly string CONFIG_FILE_PATH = @".\config.ini";
        public static readonly string ZZIPChannel_FILE_PATH = @".\ZZIPChannel.dat";
        public static readonly string CONFIG_SECTION_CONFIG = "CONFIG";
        public static readonly string CONFIG_SECTION_SQLLINK = "SQLLINK";
        public static readonly string CONFIG_SECTION_JMQ = "JMQ";
        public static readonly string CONFIG_SECTION_Q = "Q";
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
        public static readonly string CONFIG_KEY_NUM = "NUM";
        public static readonly string CONFIG_KEY_EVEN= "EVEN";
        public static readonly string CONFIG_KEY_WND2 = "WND2";
        public static readonly string CONFIG_KEY_TIME = "Time";

        //数据库
        public static readonly string DB_TABLE_TBKVIDEO = "TBKVideo";
        public static readonly string DB_TABLE_ERRORDATA = "ErrorData";
        public static readonly string DB_TABLE_STUDENTINFO = "StudentInfo";
        public static readonly string DB_FIELD_BH = "编号";
        public static readonly string DB_FIELD_SBIP = "设备IP";
        public static readonly string DB_FIELD_YHM = "用户名";
        public static readonly string DB_FIELD_MM = "密码";
        public static readonly string DB_FIELD_DKH = "端口号";
        public static readonly string DB_FIELD_TDH = "通道号";
        public static readonly string DB_FIELD_TRANSMODE = "TransMode";
        public static readonly string DB_FIELD_MEDIAIP = "MediaIP";
        public static readonly string DB_FIELD_NID = "Nid";
        public static readonly string DB_FIELD_CWBH = "错误编号";
        public static readonly string DB_FIELD_KFLX = "扣分类型";
        public static readonly string DB_FIELD_KCFS = "扣除分数";
        public static readonly string DB_FIELD_KSCS = "考试次数";
        public static readonly string DB_FIELD_DRCS = "当日次数";
        public static readonly string DB_FIELD_ZKZMBH = "准考证明编号";

        //windows message
        public static readonly int MSG_WM_USER = 0x400;
        public static readonly int MSG_UM_JGPTDATA = MSG_WM_USER + 256;
    }
}
