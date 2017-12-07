using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMQConfig
{
    public class BaseDefine
    {
        public static readonly string DB_CONN_FORMAT_SQL = @"Data Source={0};Initial Catalog = {1};User ID = {2};PWD = {3}";
        public static readonly string DB_CONN_FORMAT_ORACLE = @"user id = {0}; password={1};Data Source = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST ={2})(PORT=1521))(CONNECT_DATA=(SERVICE_NAME={3})));";
        public static readonly string DB_NAME_MASTER = @"master";
        public static readonly string DB_TABLE_TBKVIDEO = @"TBKVideo";
        public static readonly string DB_FIELD_BH = "编号";
        public static readonly string DB_FIELD_SBIP = "设备IP";
        public static readonly string DB_FIELD_YHM = "用户名";
        public static readonly string DB_FIELD_MM = "密码";
        public static readonly string DB_FIELD_DKH = "端口号";
        public static readonly string DB_FIELD_TDH = "通道号";
        public static readonly string DB_FIELD_BZ = "备注";
        public static readonly string DB_FIELD_TRANSMODE = "TransMode";
        public static readonly string DB_FIELD_MEDIAIP = "MediaIP";
        public static readonly string DB_FIELD_NID = "Nid";

        public static readonly string CONFIG_FILE_PATH_ENV = @"conf\HS_CONF_ENV.ini";
        public static readonly string CONFIG_FILE_PATH_DB = @"conf\HS_CONF_DB.ini";
        public static readonly string CONFIG_FILE_PATH_DISPLAY = @"conf\HS_CONF_DISPLAY.ini";
        public static readonly string CONFIG_FILE_PATH_CAR = @"conf\HS_CONF_CAR.ini";
        public static readonly string CONFIG_FILE_PATH_MAP = @"conf\HS_CONF_MAP.ini";
        public static readonly string CONFIG_SECTION_CONFIG = @"CONFIG";
        public static readonly string CONFIG_SECTION_JMQ = @"JMQ";
        public static readonly string CONFIG_KEY_SQLORACLE = @"SQLORACLE";
        public static readonly string CONFIG_KEY_USERNAME = @"USERNAME";
        public static readonly string CONFIG_KEY_PASSWORD = @"PASSWORD";
        public static readonly string CONFIG_KEY_DBADDRESS = @"DBADDRESS";
        public static readonly string CONFIG_KEY_INSTANCE = @"INSTANCE";
        public static readonly string CONFIG_KEY_NUM = @"NUM";
        public static readonly string CONFIG_KEY_BNC = @"BNC";
        public static readonly string CONFIG_KEY_EVEN = "EVEN";
        public static readonly string CONFIG_KEY_DISPLAY = "DISPLAY";
        public static readonly string CONFIG_KEY_DISPLAY1 = "DISPLAY1";     //车内视频
        public static readonly string CONFIG_KEY_DISPLAY2 = "DISPLAY2";     //车外视频
        public static readonly string CONFIG_KEY_DISPLAY3 = "DISPLAY3";     //考生信息
        public static readonly string CONFIG_KEY_DISPLAY4 = "DISPLAY4";     //实时信息
        public static readonly string CONFIG_KEY_VIDEOWND = "VIDEOWND";     //音频窗口
        public static readonly string CONFIG_KEY_WND2 = "WND2";
        public static readonly string CONFIG_KEY_HMQ = "HMQ";
        public static readonly string CONFIG_KEY_SLEEP_TIME= "SLEEPTIME";
        public static readonly string CONFIG_KEY_MAXX = "MAXX";
        public static readonly string CONFIG_KEY_MINX = "MINX";
        public static readonly string CONFIG_KEY_MAXY = "MAXY";
        public static readonly string CONFIG_KEY_MINY= "MINY";
        public static readonly string CONFIG_KEY_ZOOMIN = "ZOOMIN";

        public static readonly string EXCEL_SHEET_NAME_CONF_TRANS = @"通道配置";
        public static readonly string EXCEL_SHEET_NAME_CONF_CAMERA_CAR = @"车载摄像头";
        public static readonly string EXCEL_SHEET_NAME_CONF_CAMERA_XM = @"项目摄像头";

        public static readonly string STRING_EXCEL_TEMPLATE = @"template.xlsx";
        public static readonly string STRING_BITSTREAM_MASTER = @"主码流";
        public static readonly string STRING_BITSTREAM_SUB = @"子码流";
        public static readonly string STRING_EVEN_YES = @"是";
        public static readonly string STRING_EVEN_NO = @"否";
        public static readonly string STRING_WND2_YES = @"是";
        public static readonly string STRING_WND2_NO = @"否";
        public static readonly string STRING_WND_LEFT_TOP = @"左上角";
        public static readonly string STRING_WND_RIGHT_TOP = @"右上角";
        public static readonly string STRING_WND_LEFT_BOTTOM = @"左下角";
        public static readonly string STRING_WND_RIGHT_BOTTOM = @"右下角";

        public static readonly int MAP_DEFAULT_ZOOMIN = 20; //默认地图放大倍数
        //public static readonly int MAP_SPLIT_WIDTH = 500; //地图分割尺寸
        //public static readonly int MAP_SPLIT_HEIGHT = 410; //地图分割尺寸
        public static readonly int MAP_SPLIT_WIDTH = 1024; //地图分割尺寸
        public static readonly int MAP_SPLIT_HEIGHT = 838; //地图分割尺寸
        public static readonly string MAP_IMAGE_PATH = @".\res\map";   //地图图片路径
    }
}
