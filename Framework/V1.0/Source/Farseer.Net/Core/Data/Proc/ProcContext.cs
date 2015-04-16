using System;
using System.Data;
using FS.Configs;

namespace FS.Core.Data.Proc
{
    public class ProcContext : IDisposable
    {
        /// <summary>
        /// 通过数据库配置，连接数据库
        /// </summary>
        /// <param name="dbIndex">数据库选项</param>
        protected ProcContext(int dbIndex = 0) : this(DbFactory.CreateConnString(dbIndex), DbConfigs.ConfigInfo.DbList[dbIndex].DataType, DbConfigs.ConfigInfo.DbList[dbIndex].CommandTimeout) { }

        /// <summary>
        /// 通过自定义数据链接符，连接数据库
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="commandTimeout">SQL执行超时时间</param>
        protected ProcContext(string connectionString, DataBaseType dbType = DataBaseType.SqlServer, int commandTimeout = 30) : this(new DbExecutor(connectionString, dbType, commandTimeout)) { }

        /// <summary>
        /// 数据库
        /// </summary>
        /// <param name="database">数据库执行</param>
        protected ProcContext(DbExecutor database)
        {
            IsMergeCommand = true;
            QueueManger = new ProcQueueManger(database);
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
        protected internal ProcQueueManger QueueManger { get; private set; }

        /// <summary>
        /// true:启用合并执行命令、并延迟加载
        /// </summary>
        protected bool IsMergeCommand { get; set; }

        /// <summary>
        /// 保存修改
        /// IsMergeCommand=true时：只提交一次SQL到数据库
        /// </summary>
        /// <param name="isOlation">默认启用事务操作</param>
        public int SaveChanges(bool isOlation = true)
        {
            // 开启或关闭事务
            if (isOlation) { QueueManger.DataBase.OpenTran(IsolationLevel.Serializable); }
            else { QueueManger.DataBase.CloseTran(); }

            var result = QueueManger.Commit();
            // 如果开启了事务，则关闭
            if (isOlation)
            {
                QueueManger.DataBase.Commit();
                QueueManger.DataBase.CloseTran();
            }
            return result;
        }
    }
}
