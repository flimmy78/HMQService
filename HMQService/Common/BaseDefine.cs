using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMQService.Common
{
    public class BaseDefine
    {
        public static readonly string LISTENING_ADDRESS = "0.0.0.0";    //监听地址
        public static readonly int LISTENING_PORT_TCP = 6708;   //TCP 监听端口号
        public static readonly int LISTENING_PORT_UDP = 6709;   //UDP 监听端口号
        public static readonly int VIDEO_FRAME_RATE = 1;    // BitMap 转成 视频码流的每秒帧数

        public static readonly string STRING_KM2_PUBLIC_VIDEO = @"10086_1"; //科目二场地远景摄像头编号
        public static readonly string STRING_TIME_FORMAT = "HH:mm:ss";
        public static readonly string STRING_INIT_CAR = "正在初始化考车 {0} ...";
        public static readonly string STRING_EXAM_TIME_AND_SCORE = "时长:{0} 成绩:{1}";
        public static readonly string STRING_CAR_SPEED = "速度: {0} km/h";
        public static readonly string STRING_EXAM_START_TIME = "开始时间：{0}";
        public static readonly string STRING_FILE_PATH_MENCODER = @".\3rd\mencoder.exe";

        //考试所处阶段
        //科目二
        public const uint EXAM_STATE_START_DCRK = 0x0001;    //开始倒车入库
        public const uint EXAM_STATE_END_DCRK = 0x0002;  //结束倒车入库
        public const uint EXAM_STATE_START_CFTC = 0x0004;  //开始侧方停车
        public const uint EXAM_STATE_END_CFTC = 0x0008;  //结束侧方停车
        public const uint EXAM_STATE_START_DDPQ = 0x0010;  //开始定点坡起
        public const uint EXAM_STATE_END_DDPQ = 0x0020;  //结束定点坡起
        public const uint EXAM_STATE_START_QXXS = 0x0040;  //开始曲线行驶
        public const uint EXAM_STATE_END_QXXS = 0x0080;  //结束曲线行驶
        public const uint EXAM_STATE_START_ZJZW = 0x0100;  //开始直角转弯
        public const uint EXAM_STATE_END_ZJZW = 0x0200;  //结束直角转弯
        public const uint EXAM_STATE_START_YWSH = 0x0400;  //开始雨雾湿滑
        public const uint EXAM_STATE_END_YWSH = 0x0800;  //结束雨雾湿滑
        public const uint EXAM_STATE_START_MNSD = 0x1000;  //开始模拟隧道
        public const uint EXAM_STATE_END_MNSD = 0x2000;  //结束模拟隧道
        //科目三
        public const uint EXAM_STATE_START_SC = 0x0001;    //开始上车
        public const uint EXAM_STATE_END_SC = 0x0002;  //结束上车
        public const uint EXAM_STATE_START_QB = 0x0004;  //开始起步
        public const uint EXAM_STATE_END_QB = 0x0008;  //结束起步
        public const uint EXAM_STATE_START_ZHIXIAN = 0x0010;  //开始直线
        public const uint EXAM_STATE_END_ZHIXIAN = 0x0020;  //结束直线
        public const uint EXAM_STATE_START_JJ= 0x0040;  //开始加减
        public const uint EXAM_STATE_END_JJ = 0x0080;  //结束加减
        public const uint EXAM_STATE_START_BG = 0x0100;  //开始变更
        public const uint EXAM_STATE_END_BG = 0x0200;  //结束变更
        public const uint EXAM_STATE_START_KB = 0x0400;  //开始靠边
        public const uint EXAM_STATE_END_KB = 0x0800;  //结束靠边
        public const uint EXAM_STATE_START_ZHIXING = 0x1000;  //开始直行
        public const uint EXAM_STATE_END_ZHIXING = 0x2000;  //结束直行
        public const uint EXAM_STATE_START_ZZ = 0x4000;  //开始左转
        public const uint EXAM_STATE_END_ZZ = 0x8000;  //结束左转
        public const uint EXAM_STATE_START_YZ = 0x00010000;    //开始右转
        public const uint EXAM_STATE_END_YZ = 0x00020000;  //结束右转
        public const uint EXAM_STATE_START_RX = 0x00040000;    //开始人行
        public const uint EXAM_STATE_END_RX = 0x00080000;  //结束人行
        public const uint EXAM_STATE_START_XX = 0x00100000;    //开始学校
        public const uint EXAM_STATE_END_XX = 0x00200000;  //结束学校
        public const uint EXAM_STATE_START_CZ = 0x00400000;    //开始车站
        public const uint EXAM_STATE_END_CZ = 0x00800000;  //结束车站
        public const uint EXAM_STATE_START_HC = 0x01000000;    //开始会车
        public const uint EXAM_STATE_END_HC = 0x02000000;  //结束会车
        public const uint EXAM_STATE_START_CC = 0x04000000;    //开始超车
        public const uint EXAM_STATE_END_CC = 0x08000000;  //结束超车
        public const uint EXAM_STATE_START_DT = 0x10000000;    //开始掉头
        public const uint EXAM_STATE_END_DT = 0x20000000;  //结束掉头
        public const uint EXAM_STATE_START_YJ = 0x40000000;    //开始夜间
        public const uint EXAM_STATE_END_YJ = 0x80000000;  //结束夜间

        //数据格式
        public const int PACK_TYPE_M17C51 = 1;
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
        public static readonly string CONFIG_FILE_PATH_ENV = @".\conf\HS_CONF_ENV.ini";
        public static readonly string CONFIG_FILE_PATH_DB = @".\conf\HS_CONF_DB.ini";
        public static readonly string CONFIG_FILE_PATH_DISPLAY = @".\conf\HS_CONF_DISPLAY.ini";
        public static readonly string CONFIG_FILE_PATH_CAR = @".\conf\HS_CONF_CAR.ini";
        public static readonly string CONFIG_FILE_PATH_CONFIG = @".\config.ini";
        public static readonly string CONFIG_FILE_PATH_ZZIPChannel = @".\ZZIPChannel.dat";
        public static readonly string CONFIG_FILE_PATH_MAP = @".\MAP.cfg";
        public static readonly string CONFIG_SECTION_CONFIG = "CONFIG";
        public static readonly string CONFIG_SECTION_SQLLINK = "SQLLINK";  
        public static readonly string CONFIG_SECTION_JMQ = "JMQ";
        public static readonly string CONFIG_SECTION_Q = "Q";
        public static readonly string CONFIG_SECTION_MAPCONFIG = "MAPCONFIG";
        public static readonly string CONFIG_SECTION_CARSKIN = "CARSKIN";
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
        public static readonly string CONFIG_KEY_MAXX = "MAXX";
        public static readonly string CONFIG_KEY_MAXY = "MAXY";
        public static readonly string CONFIG_KEY_MINX = "MINX";
        public static readonly string CONFIG_KEY_MINY = "MINY";
        public static readonly string CONFIG_KEY_ZOOMIN = "ZoomIn";
        public static readonly string CONFIG_KEY_XC = "XC";
        public static readonly string CONFIG_KEY_YC = "YC";
        public static readonly string CONFIG_KEY_DRAWCAR = "DrawCar";
        public static readonly string CONFIG_KEY_DITUPY = "DITUPY";
        public static readonly string CONFIG_KEY_HMQ = "HMQ";

        public static readonly int CONFIG_VALUE_KSKM_2 = 2;     //考试科目为科目2
        public static readonly int CONFIG_VALUE_KSKM_3 = 3;     //考试科目为科目3
        public static readonly int CONFIG_VALUE_KSHGFS_2 = 80;  //科目2合格分数
        public static readonly int CONFIG_VALUE_KSHGFS_3 = 90;  //科目3合格分数
        public static readonly int CONFIG_VALUE_TOTAL_SCORE = 100;  //满分
        public static readonly int CONFIG_VALUE_ZERO_SCORE = 0;  //零分

        //数据库
        public static readonly string DB_TABLE_TBKVIDEO = "TBKVideo";
        public static readonly string DB_TABLE_ERRORDATA = "ErrorData";
        public static readonly string DB_TABLE_STUDENTINFO = "StudentInfo";
        public static readonly string DB_TABLE_STUDENTPHOTO = "StudentPhoto";
        public static readonly string DB_TABLE_SCHOOLINFO = "SchoolInfo";
        public static readonly string DB_TABLE_SYSCFG = "SysCfg";
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
        public static readonly string DB_FIELD_ZP = "照片";
        public static readonly string DB_FIELD_MJZP = "门禁照片";
        public static readonly string DB_FIELD_KCH = "考车号";
        public static readonly string DB_FIELD_BZ = "备注";
        public static readonly string DB_FIELD_KSCX = "考试车型";
        public static readonly string DB_FIELD_XINGMING = "姓名";
        public static readonly string DB_FIELD_XB = "性别";
        public static readonly string DB_FIELD_LSH = "流水号";
        public static readonly string DB_FIELD_SFZMBH = "身份证明编号";
        public static readonly string DB_FIELD_JXMC = "驾校名称";
        public static readonly string DB_FIELD_KSY1 = "考试员1";
        public static readonly string DB_FIELD_KSYY = "考试原因";
        public static readonly string DB_FIELD_DLR = "代理人";
        public static readonly string DB_FIELD_JXBH = "驾校编号";
        public static readonly string DB_FIELD_XIANGMU = "项目";
        public static readonly string DB_VALUE_A= "A";
        public static readonly string DB_VALUE_B = "B";
        public static readonly string DB_VALUE_D = "D";
        public static readonly string DB_VALUE_F = "F";
        public static readonly string DB_VALUE_CK = "初考";
        public static readonly string DB_VALUE_ZJ = "增驾";
        public static readonly string DB_VALUE_MFXX = "满分学习";
        public static readonly string DB_VALUE_BK = "补考";
        public static readonly string DB_VALUE_KSYYWZ = "考试原因：未知";

        //背景图片
        public static readonly string IMG_PATH_TBK = @".\res\tbk.skin";
        public static readonly string IMG_PATH_HGORBHG = @".\res\hgorbhg.skin";
        public static readonly string IMG_PATH_PASS = @".\res\pass.skin";
        public static readonly string IMG_PATH_NOTPASS = @".\res\notpass.skin";
        public static readonly string IMG_PATH_MARK = @".\res\mark.skin";
        public static readonly string IMG_PATH_TIME= @".\res\time.skin";
        public static readonly string IMG_PATH_XMP = @".\res\xmp.skin";
        public static readonly string IMG_PATH_MAPN = @".\res\MAPN.skin";
        public static readonly string IMG_PATH_SINGLE_CAR = @".\res\Car.skin";
        public static readonly string IMG_PATH_MULTI_CAR = @".\res\Car{0}.skin";

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
