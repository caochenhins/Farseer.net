using System;
using System.Collections.Generic;
using System.ComponentModel;
using FS.Configs;
using FS.Core.Data;
using FS.Mapping.Context;

namespace FS.Core.Infrastructure
{
    public abstract class BaseContext : IDisposable
    {
        /// <summary>
        /// 使用DB特性设置数据库信息
        /// </summary>
        protected BaseContext()
        {
            ContextMap = CacheManger.GetContextMap(this.GetType());
            DataBase = new DbExecutor(ContextMap.ContextProperty.ConnStr, ContextMap.ContextProperty.DataType, ContextMap.ContextProperty.CommandTimeout);
        }

        /// <summary>
        /// 通过数据库配置，连接数据库
        /// </summary>
        /// <param name="dbIndex">数据库选项</param>
        protected BaseContext(int dbIndex) : this(CacheManger.CreateConnString(dbIndex), DbConfigs.ConfigEntity.DbList[dbIndex].DataType, DbConfigs.ConfigEntity.DbList[dbIndex].CommandTimeout) { }

        /// <summary>
        /// 通过自定义数据链接符，连接数据库
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="commandTimeout">SQL执行超时时间</param>
        protected BaseContext(string connectionString, DataBaseType dbType = DataBaseType.SqlServer, int commandTimeout = 30)
        {
            DataBase = new DbExecutor(connectionString, dbType, commandTimeout);
            ContextMap = CacheManger.GetContextMap(this.GetType());
        }

        /// <summary>
        /// 实例化子类中，所有Set属性
        /// </summary>
        protected void InstanceProperty(object context, string propertyName)
        {
            var lstPropertyInfo = this.GetType().GetProperties();
            foreach (var propertyInfo in lstPropertyInfo)
            {
                if (!propertyInfo.CanWrite || propertyInfo.PropertyType.Name != propertyName) { continue; }
                // 动态实例化属性
                var set = Activator.CreateInstance(propertyInfo.PropertyType, context);
                propertyInfo.SetValue(context, set, null);
            }
        }

        /// <summary>
        /// 数据库操作
        /// </summary>
        protected DbExecutor DataBase { get; private set; }

        /// <summary>
        /// TableContext、ProcContext、ViewContext 映射关系
        /// </summary>
        public ContextMap ContextMap { get; private set; } 

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
        protected abstract void Dispose(bool disposing);

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
    }
}
