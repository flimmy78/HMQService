using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMQConfig
{
    public class BaseDefine
    {
        public static readonly string DB_CONN_FORMAT = @"Data Source={0};Initial Catalog = {1};User ID = {2};PWD = {3}";
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

        public static readonly string CONFIG_FILE_PATH_ENV = @".\conf\HS_CONF_ENV.ini";
        public static readonly string CONFIG_FILE_PATH_DB = @".\conf\HS_CONF_DB.ini";
        public static readonly string CONFIG_FILE_PATH_DISPLAY = @".\conf\HS_CONF_DISPLAY.ini";
        public static readonly string CONFIG_FILE_PATH_CAR = @".\conf\HS_CONF_CAR.ini";
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
        public static readonly string CONFIG_KEY_DISPLAY1 = "DISPLAY1";
        public static readonly string CONFIG_KEY_DISPLAY2 = "DISPLAY2";
        public static readonly string CONFIG_KEY_DISPLAY3 = "DISPLAY3";
        public static readonly string CONFIG_KEY_DISPLAY4 = "DISPLAY4";
        public static readonly string CONFIG_KEY_VIDEOWND = "VIDEOWND";

        public static readonly string EXCEL_SHEET_NAME_CONF_TRANS = @"通道配置";
        public static readonly string EXCEL_SHEET_NAME_CONF_CAMERA_CAR = @"车载摄像头";
        public static readonly string EXCEL_SHEET_NAME_CONF_CAMERA_XM = @"项目摄像头";

        public static readonly string STRING_BITSTREAM_MASTER = @"主码流";
        public static readonly string STRING_BITSTREAM_SUB = @"子码流";
        public static readonly string STRING_EVEN_YES = @"是";
        public static readonly string STRING_EVEN_NO = @"否";
    }
}
