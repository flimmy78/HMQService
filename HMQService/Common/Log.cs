using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace HMQService.Common
{
    public class Log
    {
        public static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ILog GetLogger()
        {
            return logger;
        }
    }
}
