using System;
using FS.Configs;

namespace FS.Core.Data.View
{
    public class ViewContext : IDisposable
    {
        /// <summary>
        /// 通过数据库配置，连接数据库
        /// </summary>
        /// <param name="dbIndex">数据库选项</param>
        protected ViewContext(int dbIndex = 0) : this(DbFactory.CreateConnString(dbIndex), DbConfigs.ConfigInfo.DbList[dbIndex].DataType, DbConfigs.ConfigInfo.DbList[dbIndex].CommandTimeout) { }

        /// <summary>
        /// 通过自定义数据链接符，连接数据库
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="commandTimeout">SQL执行超时时间</param>
        protected ViewContext(string connectionString, DataBaseType dbType = DataBaseType.SqlServer, int commandTimeout = 30) : this(new DbExecutor(connectionString, dbType, commandTimeout)) { }

        /// <summary>
        /// 数据库
        /// </summary>
        /// <param name="database">数据库执行</param>
        protected ViewContext(DbExecutor database)
        {
            QueueManger = new ViewQueueManger(database);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing)
            {
                QueueManger.DataBase.Dispose();
                QueueManger.DataBase = null;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary> 
        /// 队列管理
        /// </summary>
        protected internal ViewQueueManger QueueManger { get; private set; }
    }
}