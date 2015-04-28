using System;
using System.ComponentModel.DataAnnotations;
using FS.Mapping.Table.Attribute;

namespace FS.Mapping.Table
{
    /// <summary>
    /// 保存字段映射的信息
    /// </summary>
    public class FieldMapState
    {
        /// <summary>
        ///     数据类型
        /// </summary>
        public DataTypeAttribute DataType { get; set; }

        /// <summary>
        ///     字段映射
        /// </summary>
        public ColumnAttribute Column { get; set; }

        /// <summary>
        ///     扩展类型
        /// </summary>
        public eumPropertyExtend PropertyExtend { get; set; }

        /// <summary>
        ///     指示字段是否为存储过程中输入的参数
        /// </summary>
        public bool IsInParam { get; set; }

        /// <summary>
        ///     指示字段是否为存储过程中输出的参数
        /// </summary>
        public bool IsOutParam { get; set; }
    }

    /// <summary>
    ///     属性类型，自定义扩展属性
    /// </summary>
    public enum eumPropertyExtend
    {
        /// <summary>
        ///     Xml属性
        /// </summary>
        Attribute,

        /// <summary>
        ///     Xml节点
        /// </summary>
        Element,
    }
}