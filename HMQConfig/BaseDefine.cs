using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMQConfig
{
    public class BaseDefine
    {
        public static readonly string DATABASE_CONN_FORMAT = @"Data Source={0};Initial Catalog = {1};User ID = {2};PWD = {3}";
        public static readonly string DATABASE_NAME_MASTER = @"master";

        public static readonly string CONFIG_FILE_PATH_ENV = @".\conf\HS_CONF_ENV.ini";
        public static readonly string CONFIG_FILE_PATH_DB = @".\conf\HS_CONF_DB.ini";
        public static readonly string CONFIG_SECTION_CONFIG = @"CONFIG";
        public static readonly string CONFIG_KEY_SQLORACLE = @"SQLORACLE";
        public static readonly string CONFIG_KEY_USERNAME = @"USERNAME";
        public static readonly string CONFIG_KEY_PASSWORD = @"PASSWORD";
        public static readonly string CONFIG_KEY_DBADDRESS = @"DBADDRESS";
        public static readonly string CONFIG_KEY_INSTANCE = @"INSTANCE";

        public static readonly string EXCEL_SHEET_NAME_CAR = @"考车配置";
        public static readonly string EXCEL_SHEET_NAME_CAMERA = @"摄像头配置";
    }
}
