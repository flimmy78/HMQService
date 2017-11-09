using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace BekUtils.Util
{
    public class Log
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ILog GetLogger()
        {
            return logger;
        }

        public static void TempDebugFormat(string str)
        {
            logger.DebugFormat("HQW DEBUG TEMP : {0}", str);
        }
    }
}
