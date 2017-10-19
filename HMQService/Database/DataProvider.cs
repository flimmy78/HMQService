using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMQService.Database
{
    /// <summary>
    /// 提供对数据库访问的通用类。
    /// </summary>
    public class DataProvider
    {
        /// <summary>
        /// 数据库枚举类型
        /// </summary>
        public enum DataProviderType
        {
            OdbcDataProvider = 0,
            OleDbDataProvider = 1,
            OracleDataProvider = 2,
            SqlDataProvider = 3
        }

        /// <summary>
        /// 建立访问数据库的实例
        /// </summary>
        /// <param name="DataProviderType">数据库枚举类型</param>
        /// <returns></returns>
        public static IDataProvider CreateDataProvider(DataProviderType dataProviderType)
        {
            switch (dataProviderType)
            {
                case DataProviderType.OdbcDataProvider:
                    //return new OdbcDataProvider();
                    return null;
                case DataProviderType.OleDbDataProvider:
                    //return new OleDbDataProvider();
                    return null;
                case DataProviderType.OracleDataProvider:
                    return new OracleDataProvider();
                case DataProviderType.SqlDataProvider:
                    return new SqlDataProvider();
                default:
                    return null;
            }
        }
    }
}
