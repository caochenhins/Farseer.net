using System;
using System.ComponentModel;
using FS.Configs;
using FS.Mapping.Table;

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
            InstanceProperty();
        }

        /// <summary>
        /// 实例化子类中，所有Set属性
        /// </summary>
        protected virtual void InstanceProperty()
        {
            var types = this.GetType().GetProperties();
            foreach (var type in types)
            {
                if (type.PropertyType.Name != "ViewSet`1") { continue; }
                var map = TableMapCache.GetMap(this.GetType());
                var user = Activator.CreateInstance(type.PropertyType, this, map.GetFieldName(type));
                type.SetValue(this, user, null);
            }
        }

        #region 禁用智能提示
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Type GetType()
        {
            return base.GetType();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
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
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary> 
        /// 队列管理
        /// </summary>
        protected internal ViewQueueManger QueueManger { get; private set; }
    }
}