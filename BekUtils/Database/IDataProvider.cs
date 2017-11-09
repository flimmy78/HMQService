using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BekUtils.Database
{
    /// <summary>
    /// 对数据库访问的通用接口
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// 执行 UPDATE、INSERT 和 DELETE 语句，返回值为该命令所影响的行数
        /// </summary>  
        int ExecuteNonQuery(string sql);

        /// <summary>
        ///  执行单Sql语句查询，并将查询返回的结果作为一个数据集返回
        /// </summary>  
        System.Data.DataSet RetriveDataSet(string sql);

        /// <summary>
        /// 执行Dispose
        /// </summary>
        void Dispose();
    }
}
