using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using FS.Configs;
using FS.Core.Data;

namespace FS.Core.Infrastructure
{
    public abstract class BaseContext : IDisposable
    {
        /// <summary>
        /// 使用DB特性设置数据库信息
        /// </summary>
        protected BaseContext()
        {
            var map = CacheManger.GetTableMap(this.GetType());
            DataBase = new DbExecutor(map.ClassInfo.ConnStr, map.ClassInfo.DataType, map.ClassInfo.CommandTimeout);
        }

        /// <summary>
        /// 通过数据库配置，连接数据库
        /// </summary>
        /// <param name="dbIndex">数据库选项</param>
        protected BaseContext(int dbIndex) : this(CacheManger.CreateConnString(dbIndex), DbConfigs.ConfigInfo.DbList[dbIndex].DataType, DbConfigs.ConfigInfo.DbList[dbIndex].CommandTimeout) { }

        /// <summary>
        /// 通过自定义数据链接符，连接数据库
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="commandTimeout">SQL执行超时时间</param>
        protected BaseContext(string connectionString, DataBaseType dbType = DataBaseType.SqlServer, int commandTimeout = 30)
        {
            DataBase = new DbExecutor(connectionString, dbType, commandTimeout);
        }

        /// <summary>
        /// 实例化子类中，所有Set属性
        /// </summary>
        protected void InstanceProperty(object entity, string propertyName)
        {
            var types = this.GetType().GetProperties();
            var map = CacheManger.GetTableMap(this.GetType());
            foreach (var type in types)
            {
                if (!type.CanWrite || type.PropertyType.Name != propertyName) { continue; }
                //var set = CacheManger.GetInstance(type.PropertyType, entity, map.GetFieldName(type));
                var set = Activator.CreateInstance(type.PropertyType, entity, map.GetFieldName(type));
                type.SetValue(entity, set, null);
            }
        }

        protected DbExecutor DataBase { get; private set; }
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
