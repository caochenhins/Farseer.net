﻿using System;

namespace FS.Mapping.Table.Attribute
{
    /// <summary>
    /// 设置字段在数据库中的映射关系
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ColumnAttribute : System.Attribute
    {
        /// <summary>
        /// 设置字段在数据库中的映射关系
        /// </summary>
        public ColumnAttribute()
        {
            IsMap = true;
            IsPrimaryKey = false;
        }

        /// <summary>
        /// 数据库字段名称（映射）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否为数据库主键（自动标识）
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 是否映射到数据库字段中(默认为true)
        /// </summary>
        public bool IsMap { get; set; }
    }
}
