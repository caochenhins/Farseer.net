using System;
using FS.Core.Data;

namespace FS.Mapping.Table
{
    /// <summary>
    ///     实体类的属性标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DBAttribute : Attribute
    {
        /// <summary>
        ///     默认第一个数据库配置
        /// </summary>
        /// <param name="name">表名/视图名/存储过程名/</param>
        /// <param name="dbIndex">数据库选项</param>
        public DBAttribute(int dbIndex, string name = null)
        {
            DbIndex = dbIndex;
            Name = name;
        }

        /// <summary>
        ///     默认第一个数据库配置
        /// </summary>
        /// <param name="name">表名/视图名/存储过程名/</param>
        public DBAttribute(string name = null) : this(0, name) { }

        /// <summary>
        ///     表名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     设置数据库连接配置(Dbconfig)的索引项
        /// </summary>
        public int DbIndex { get; set; }

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