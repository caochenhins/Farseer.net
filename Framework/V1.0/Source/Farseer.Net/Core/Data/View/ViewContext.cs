using System;
using System.ComponentModel;
using FS.Configs;
using FS.Core.Data.Table;
using FS.Mapping.Table;

namespace FS.Core.Data.View
{
    public class ViewContext : BaseContext, IDisposable
    {
        /// <summary>
        /// 使用DB特性设置数据库信息
        /// </summary>
        protected ViewContext()
        {
            InstanceProperty("ViewSet`1");
        }

        /// <summary>
        /// 通过数据库配置，连接数据库
        /// </summary>
        /// <param name="dbIndex">数据库选项</param>
        protected ViewContext(int dbIndex) : base(dbIndex) { InstanceProperty("ViewSet`1"); }

        /// <summary>
        /// 通过自定义数据链接符，连接数据库
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="commandTimeout">SQL执行超时时间</param>
        protected ViewContext(string connectionString, DataBaseType dbType = DataBaseType.SqlServer, int commandTimeout = 30) : base(connectionString, dbType, commandTimeout) { InstanceProperty("TableSet`1"); }


        /// <summary>
        /// 实例化子类中，所有Set属性
        /// </summary>
        protected override sealed void InstanceProperty(string propertyName)
        {
            QueueManger = new ViewQueueManger(DataBase);
            base.InstanceProperty(propertyName);
        }

        /// <summary> 
        /// 队列管理
        /// </summary>
        protected internal ViewQueueManger QueueManger { get; private set; }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing)
            {
                QueueManger.DataBase.Dispose();
                QueueManger.DataBase = null;
            }
        }
    }
}