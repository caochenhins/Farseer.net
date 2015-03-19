using System;
using System.Data;
using FS.Configs;
using FS.Core.Data;
using FS.Core.Infrastructure;

namespace FS.Core.Context
{
    /// <summary>
    /// 表上下文
    /// </summary>
    public class DbContext : IDisposable
    {
        /// <summary>
        /// 通过数据库配置，连接数据库
        /// </summary>
        /// <param name="dbIndex">数据库选项</param>
        /// <param name="tableName">表名/视图名/存储过程名</param>
        protected internal DbContext(int dbIndex = 0, string tableName = null) : this(DbFactory.CreateConnString(dbIndex), DbConfigs.ConfigInfo.DbList[dbIndex].DataType, DbConfigs.ConfigInfo.DbList[dbIndex].CommandTimeout, tableName) { }

        /// <summary>
        /// 通过自定义数据链接符，连接数据库
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="commandTimeout">SQL执行超时时间</param>
        /// <param name="tableName">表名/视图名/存储过程名</param>
        protected internal DbContext(string connectionString, DataBaseType dbType = DataBaseType.SqlServer, int commandTimeout = 30, string tableName = null) : this(new DbExecutor(connectionString, dbType, commandTimeout), tableName) { }

        /// <summary>
        /// 事务
        /// </summary>
        /// <param name="database">数据库执行</param>
        /// <param name="tableName">表名/视图名/存储过程名</param>
        protected internal DbContext(DbExecutor database, string tableName = null)
        {
            Database = database;
            TableName = tableName;
            Query = DbFactory.CreateQuery(this);
        }

        /// <summary>
        /// 数据库
        /// </summary>
        internal protected DbExecutor Database { get; private set; }

        /// <summary>
        /// 数据库查询支持
        /// </summary>
        internal protected IQuery Query { get; set; }

        /// <summary>
        /// 表名/视图名/存储过程名
        /// </summary>
        internal protected string TableName { get; protected set; }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Database.Dispose();
            Database = null;
        }
    }
}