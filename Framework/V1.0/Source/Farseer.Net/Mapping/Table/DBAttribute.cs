using System;
using FS.Configs;
using FS.Core;
using FS.Core.Data;

namespace FS.Mapping.Table
{
    /// <summary>
    ///     实体类的属性标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    // ReSharper disable once InconsistentNaming
    public sealed class DBAttribute : Attribute
    {
        /// <summary>
        ///     默认第一个数据库配置
        /// </summary>
        /// <param name="name">表名/视图名/存储过程名/</param>
        /// <param name="dbIndex">数据库选项</param>
        public DBAttribute(string name, int dbIndex = 0) : this(name, DbFactory.CreateConnString(dbIndex), DbConfigs.ConfigInfo.DbList[dbIndex].DataType, DbConfigs.ConfigInfo.DbList[dbIndex].DataVer, DbConfigs.ConfigInfo.DbList[dbIndex].CommandTimeout) { }

        /// <summary>
        ///     默认第一个数据库配置
        /// </summary>
        /// <param name="name">表名/视图名/存储过程名/</param>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="dataVer">数据库版本</param>
        /// <param name="commandTimeout">SQL执行超时时间</param>
        public DBAttribute(string name, string connectionString, DataBaseType dbType = DataBaseType.SqlServer, string dataVer = "2008", int commandTimeout = 30)
        {
            Name = name;
            ConnStr = connectionString;
            DataType = dbType;
            DataVer = dataVer;
            CommandTimeout = commandTimeout;
        }

        /// <summary>
        ///     表名
        /// </summary>
        public string Name { get; set; }

        ///// <summary>
        /////     设置数据库连接配置(Dbconfig)的索引项
        ///// </summary>
        //public int DbIndex { get; set; }

        /// <summary>
        ///     设置数据库连接字符串
        /// </summary>
        public string ConnStr { get; set; }

        /// <summary>
        ///     设置数据库类型
        /// </summary>
        public DataBaseType DataType { get; set; }

        /// <summary>
        ///     设置数据库版本
        /// </summary>
        public string DataVer { get; set; }

        /// <summary>
        ///     设置数据库执行T-SQL时间，单位秒默认是30秒
        /// </summary>
        public int CommandTimeout { get; set; }
    }
}