using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace BekUtils.Database
{
    internal class SqlDataProvider : IDataProvider
    {
        private SqlConnection sqlConnection;
        private SqlCommand sqlCommand;
        private string connectionString;

        public SqlDataProvider() : this(null)
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        public SqlDataProvider(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return;
            }
            else
            {
                this.connectionString = connectionString;
            }
        }

        /// <summary>
        /// SqlServer 连接字符串 "Data Source=myServerAddress;Initial Catalog=myDataBase;User Id=myUsername;Password=myPassword;"    
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return this.connectionString;
            }
            set
            {
                this.connectionString = value;
            }
        }

        /// <summary>
        /// 返回一个带有连接字符串的SQLSERVER Connection.
        /// </summary>
        /// <returns>OracleConnection</returns>
        private SqlConnection GetSqlServerConnection()
        {
            try
            {
                return new SqlConnection(this.connectionString);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 对于 UPDATE、INSERT 和 DELETE 语句，返回值为该命令所影响的行数。对于其他所有类型的语句，返回值为 -1
        /// </summary>
        /// <param name="Sql">UPDATE、INSERT 和 DELETE 语句</param>
        public int ExecuteNonQuery(string sql)
        {
            using (sqlConnection = this.GetSqlServerConnection())
            {
                if (sqlConnection == null)
                    return -1;
                int rv = -1;

                //SqlTransaction sqlTransaction = null;
                try
                {
                    if (System.Data.ConnectionState.Closed == sqlConnection.State)
                    {
                        sqlConnection.Open();
                    }

                    //sqlCommand = new SqlCommand(sql, sqlConnection);
                    //sqlTransaction = sqlConnection.BeginTransaction();
                    //sqlCommand.Transaction = sqlTransaction;
                    //rv = sqlCommand.ExecuteNonQuery();
                    //sqlTransaction.Commit();

                    sqlCommand = new SqlCommand();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.CommandText = sql;
                    rv = sqlCommand.ExecuteNonQuery();
                }
                catch(Exception e)
                {
                    //sqlTransaction.Rollback();
                    rv = -1;
                }

                return rv;
            }
        }

        /// <summary>
        ///  执行单Sql语句查询，并将查询返回的结果作为一个数据集返回
        /// </summary>
        /// <param name="selectSql">SELECT 语句</param>
        /// <returns>数据集 DataSet</returns>
        public DataSet RetriveDataSet(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return null;
            }
            using (sqlConnection = this.GetSqlServerConnection())
            {
                if (sqlConnection == null)
                    return null;
                if (System.Data.ConnectionState.Closed == sqlConnection.State)
                {
                    sqlConnection.Open();
                }
                using (SqlDataAdapter da = new SqlDataAdapter(sql, sqlConnection))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds);
                    }
                    catch (Exception e)
                    {
                    }
                    return ds;
                }
            }
        }

        public void Dispose()
        {
            this.connectionString = null;
            //this.sqlCommand.Dispose();
            this.sqlConnection.Dispose();
        }

    }
}
