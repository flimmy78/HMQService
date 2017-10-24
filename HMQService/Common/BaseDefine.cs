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

        //接口字符串分隔符
        public static readonly char SPLIT_CHAR_ASTERISK = '*';
        public static readonly char SPLIT_CHAR_AMPERSAND = '&';
        public static readonly char SPLIT_CHAR_HASH_SIGN = '#';
        public static readonly char SPLIT_CHAR_SPACE = ' ';
        public static readonly char SPLIT_CHAR_COMMA = ',';
        public static readonly char SPLIT_CHAR_AT = '@';
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
        public static readonly int CONFIG_VALUE_KSKM_2 = 2;     //考试科目为科目2
        public static readonly int CONFIG_VALUE_KSKM_3 = 3;     //考试科目为科目3
        public static readonly int CONFIG_VALUE_KSHGFS_2 = 80;  //科目2合格分数
        public static readonly int CONFIG_VALUE_KSHGFS_3 = 90;  //科目3合格分数

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

        //项目编号
        public const int XMBH_201 = 201;
        public const int XMBH_202 = 202;
        public const int XMBH_203 = 203;
        public const int XMBH_204 = 204;
        public const int XMBH_205 = 205;
        public const int XMBH_206 = 206;
        public const int XMBH_207 = 207;
        public const int XMBH_208 = 208;
        public const int XMBH_209 = 209;
        public const int XMBH_210 = 210;
        public const int XMBH_211 = 211;
        public const int XMBH_212 = 212;
        public const int XMBH_213 = 213;
        public const int XMBH_214 = 214;
        public const int XMBH_215 = 215;
        public const int XMBH_216 = 216;
        public const int XMBH_217 = 217;
        public const int XMBH_249 = 249;
        public const int XMBH_259 = 259;
        public const int XMBH_300 = 300;
        public const int XMBH_400 = 400;
        public const int XMBH_500 = 500;
        public const int XMBH_600 = 600;
        public const int XMBH_700 = 700;
        public const int XMBH_800 = 800;
        public const int XMBH_15010 = 15010;
        public const int XMBH_15020 = 15020;
        public const int XMBH_15030 = 15030;
        public const int XMBH_201509 = 201509;
        public const int XMBH_201510 = 201510;
        public const int XMBH_201700 = 201700;
        public const int XMBH_203509 = 203509;
        public const int XMBH_203510 = 203510;
        public const int XMBH_203700 = 203700;
        public const int XMBH_204509 = 204509;
        public const int XMBH_204510 = 204510;
        public const int XMBH_204700 = 204700;
        public const int XMBH_206509 = 206509;
        public const int XMBH_206510 = 206510;
        public const int XMBH_206700 = 206700;
        public const int XMBH_207509 = 207509;
        public const int XMBH_207510 = 207510;
        public const int XMBH_207700 = 207700;

        //项目名称
        public static readonly string XMMC_SCZB = "上车准备";   //科目3
        public static readonly string XMMC_QB = "起步";
        public static readonly string XMMC_ZHIXIAN = "直线";
        public static readonly string XMMC_BG = "变更";
        public static readonly string XMMC_TGLK = "通过路口";
        public static readonly string XMMC_RX = "人行";
        public static readonly string XMMC_XX = "学校";
        public static readonly string XMMC_CZ = "车站";
        public static readonly string XMMC_HC = "会车";
        public static readonly string XMMC_CC = "超车";
        public static readonly string XMMC_KB = "靠边";
        public static readonly string XMMC_DT = "掉头";
        public static readonly string XMMC_YJ = "夜间";
        public static readonly string XMMC_ZZ = "左转";
        public static readonly string XMMC_YZ = "右转";
        public static readonly string XMMC_ZHIXING = "直行";
        public static readonly string XMMC_JJ = "加减";
        public static readonly string XMMC_ZH = "综合";
        public static readonly string XMMC_DCRK = "倒车入库";   //科目2
        public static readonly string XMMC_CFTC = "侧方停车";
        public static readonly string XMMC_DDPQ = "定点坡起";
        public static readonly string XMMC_QXXS = "曲线行驶";
        public static readonly string XMMC_ZJZW = "直角转弯";
        public static readonly string XMMC_YWSH = "雨雾湿滑";
        public static readonly string XMMC_MNSD = "模拟遂道";
        public static readonly string XMMC_ZHPP = "综合评判";

        //windows message
        public static readonly int MSG_WM_USER = 0x400;
        public static readonly int MSG_UM_JGPTDATA = MSG_WM_USER + 256;
    }
}
